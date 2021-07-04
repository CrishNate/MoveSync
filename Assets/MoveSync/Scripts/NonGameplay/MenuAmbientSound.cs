using System;
using UnityEngine;

namespace MoveSync
{
    public class MenuAmbientSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (!LevelSequencer.instance.songPlaying)
                return;
            
            _audioSource.volume = 1 - LevelSequencer.instance.audioSource.volume;
        }
    }
}