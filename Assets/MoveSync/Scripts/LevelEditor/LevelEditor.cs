using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        void Awake()
        {
            LevelSequencer.instance.audioSource.Stop();
        }

        public void SetSongTime(float time)
        {
            time *= LevelSequencer.instance.toTime;
            LevelSequencer.instance.audioSource.time = time;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (LevelSequencer.instance.audioSource.isPlaying)
                    LevelSequencer.instance.audioSource.Pause();
                else
                    LevelSequencer.instance.audioSource.Play();
            }
        }
    }
}