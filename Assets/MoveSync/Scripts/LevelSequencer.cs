using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    [Serializable]
    struct SongInfo
    {
        public float bpm;
        public float offset;
    }

    [RequireComponent(typeof(AudioSource))]
    public class LevelSequencer : Singleton<LevelSequencer>
    {
        [Header("Song Settings")] [SerializeField]
        private SongInfo _songInfo;

        private AudioSource _audioSource;

        [Header("Gameplay settings")] [SerializeField]
        private float _restartTime = 1.0f;

        [Header("Events")] [SerializeField] private UnityEvent _onRestartFinished;
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
            foreach (var beatObject in FindObjectsOfType<BeatObject>())
            {
                Destroy(beatObject.gameObject);
            }

            foreach (var projectile in FindObjectsOfType<BaseProjectile>())
            {
                Destroy(projectile.gameObject);
            }

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
            _audioSource.time = Mathf.Min(songTime + songOffset, _audioSource.clip.length - epsilon);
        }

        public AudioSource audioSource => _audioSource;
        public float timeBPM => time * toBPM;
        public float bpm => _songInfo.bpm;
        public float toBPM => _songInfo.bpm / 60.0f;
        public float toTime => 60.0f / _songInfo.bpm;
        public float time => _audioSource.time - songOffset;
        public float songOffset => _songInfo.offset;
    }
}