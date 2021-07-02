using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    [RequireComponent(typeof(AudioSource))]
    public class LevelSequencer : Singleton<LevelSequencer>
    {
        [Header("Events")] 
        [SerializeField] private UnityEvent _onRestartFinished;
        [SerializeField] private UnityEvent _onRestart;

        private SongParams _songParams;
        private AudioSource _audioSource;
        private float _restartTime = 1.0f;
        private float _toBPM;
        private float _toTime;
        // sequencing with song
        private float _invFrequency;
        

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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

        public void Restart()
        {
            StartCoroutine(RestartCoroutine());

            _onRestart.Invoke();
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
        
        void RestartFinish()
        {
            audioSource.pitch = 1.0f;
            audioSource.Play();

            PlayerBehaviour.instance.Restart();
            _onRestartFinished.Invoke();
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
    }
}