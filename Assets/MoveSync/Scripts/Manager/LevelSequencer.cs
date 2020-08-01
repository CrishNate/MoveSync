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
        [HideInInspector] public SongInfo songInfo;

        private AudioSource _audioSource;
        private float _restartTime = 1.0f;

        [Header("Events")] 
        [SerializeField] private UnityEvent _onRestartFinished;
        [SerializeField] private UnityEvent _onRestart;

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

        void RestartFinish()
        {
            audioSource.pitch = 1.0f;
            audioSource.Play();

            PlayerBehaviour.instance.Restart();
            _onRestartFinished.Invoke();
        }

        public void SetSongTime(float songTime)
        {
            const float epsilon = 0.1f;

            // epsiion prevent this error
            // ERROR: Error executing result (An invalid seek position was passed to this function. )
            _audioSource.time = Mathf.Max(0, Mathf.Min(songTime + songOffset, _audioSource.clip.length - epsilon));
        }
        

        public AudioSource audioSource => _audioSource;
        public float timeBPM => time * toBPM;
        public float bpm => songInfo.bpm;
        public float toBPM => songInfo.bpm / 60.0f;
        public float toTime => 60.0f / songInfo.bpm;
        public float time => _audioSource.time - songOffset;
        public float songOffset => songInfo.offset;
        public bool songPlaying => _audioSource.isPlaying;
    }
}