using System;
using UnityEngine;

namespace MoveSync
{
    [RequireComponent(typeof(AudioSource))]
    public class Metronome : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _tempoClip;
        [SerializeField] private AudioClip _beatClip;
        private int _timeMarker;

        private static float _audioPlayDelay = 0.0f;
            
        
        private void Update()
        {
            if (!LevelSequencer.instance.songPlaying) return;
            
            float timeAudioOffset = LevelSequencer.instance.timeBPM + _audioPlayDelay;
            if (_timeMarker > timeAudioOffset)
                _timeMarker = Mathf.FloorToInt(timeAudioOffset);

            if (timeAudioOffset - _timeMarker >= 2.0f)
                _timeMarker = Mathf.FloorToInt(timeAudioOffset);

            if (timeAudioOffset - _timeMarker >= 1.0f)
            {
                _timeMarker = Mathf.FloorToInt(timeAudioOffset);

                if (Mathf.FloorToInt(_timeMarker) % 4 == 0)
                    _audioSource.PlayOneShot(_tempoClip);
                else
                    _audioSource.PlayOneShot(_beatClip);
                
            }
        }
    }
}