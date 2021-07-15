using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [RequireComponent(typeof(AudioSource))]
    public class LevelSequencer : Singleton<LevelSequencer>
    {
        public UnityEvent onRestartFinished;
        public UnityEvent onRestart;
        public UnityEvent onLevelFinished;
        
        private bool _pendingLevelFinished;
        private List<BeatUpdate> _beatObjectInstances = new List<BeatUpdate>();

        private SongParams _songParams;
        private AudioSource _audioSource;
        private static float _restartTime = 1.0f;
        private static float _volumeTime = 1.0f;
        
        private float _toBPM;
        private float _toTime;
        
        // sequencing with song
        private float _invFrequency;
        private float _songLengthBPM;
        
        private float _lastBeatObjectTimeBPM;
        
        // debug stuff
        public bool blockLevelFinishing;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            LevelDataManager.instance.onLoadedSong.AddListener(RecalculateLastBeatObjectTime);
        }

        void Update()
        {
            // Loop update all beat objects
            for (int i = 0; i < beatObjectInstances.Count; i++)
            {
                beatObjectInstances[i].InnerUpdate();
            }
            
            // Song End
            if (!songPlaying || blockLevelFinishing) 
                return;

            if (timeBPM > _lastBeatObjectTimeBPM && !_pendingLevelFinished)
            {
                SongFinished();
            }
        }
        
        public void BeginRestart()
        {
            StartCoroutine(RestartCoroutine());

            onRestart.Invoke();
        }
        
        IEnumerator RestartCoroutine()
        {
            while (audioSource.pitch > 0)
            {
                audioSource.pitch -= Time.deltaTime / _restartTime;
                yield return null;
            }

            audioSource.pitch = 0;

            yield return new WaitForSeconds(1.0f);

            RestartFinish();
        }

        void RestartFinish()
        {
            audioSource.pitch = 1.0f;
            audioSource.Play();

            PlayerBehaviour.instance.Restart();
            onRestartFinished.Invoke();
        }
        
        public void Play()
        {
            audioSource.Play();
        }

        public void Pause()
        {
            audioSource.Pause();
        }
        
        public void SetSongTime(float songTime)
        {
            const float epsilon = 0.1f;

            // epsiion prevent this error
            // ERROR: Error executing result (An invalid seek position was passed to this function. )
            _audioSource.time = Mathf.Clamp(songTime + songOffset, 0, _audioSource.clip.length - epsilon);
        }
                
        public void SetSongOffset(float inSongOffset)
        {
            _songParams.offset = inSongOffset;
        }
        
        public void StartSong()
        {
            SetSongTime(0);

            Play();
            _pendingLevelFinished = false;

            StartCoroutine(FadeInVolume());
        }

        public void SongFadeOut()
        {
            StartCoroutine(FadeOutVolume());
        }
        
        void SongFinished()
        {
            onLevelFinished.Invoke();
            _pendingLevelFinished = true;

            SongFadeOut();
        }
        
        // Volume Coroutine
        IEnumerator FadeOutVolume()
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime / _volumeTime;
                yield return null;
            }

            audioSource.volume = 0;
            audioSource.Stop();
        }
        
        IEnumerator FadeInVolume()
        {
            audioSource.volume = 0;
            
            while (audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime / _volumeTime;
                yield return null;
            }

            audioSource.volume = 1;
        }

        void RecalculateLastBeatObjectTime()
        {
            if (LevelDataManager.instance.levelInfo.beatObjectDatas.Count == 0) 
                return;
            
            _lastBeatObjectTimeBPM = LevelDataManager.instance.levelInfo.beatObjectDatas.Last().durationTime;
        }
        
        public SongParams songParams
        {
            get => _songParams;
            set
            {
                _songParams = value;
                _toBPM = value.bpm / 60.0f;
                _toTime = 60.0f / value.bpm;
                _invFrequency = 1.0f / audioSource.clip.frequency;
            }
        }
        
        public AudioSource audioSource => _audioSource;
        public float timeBPM => time * toBPM;
        public float bpm => songParams.bpm;
        public float toBPM => _toBPM;
        public float toTime => _toTime;
        public float time => audioSource.timeSamples * _invFrequency - songOffset;
        public float songOffset => songParams.offset;
        public bool songPlaying => _audioSource.isPlaying;
        public List<BeatUpdate> beatObjectInstances => _beatObjectInstances;
    }
}