using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class MenuBackLight : MonoBehaviour
    {
        private Animation[] _animations;
        private float _time;

        private void Start()
        {
            _animations = GetComponentsInChildren<Animation>();
        }

        private void Update()
        {
            if (LevelSequencer.instance.timeBPM > _time)
            {
                _time = Mathf.Round(LevelSequencer.instance.timeBPM) + 1;
                Flicker();
            }
            else if (LevelSequencer.instance.timeBPM < _time - 1)
            {
                _time = Mathf.Round(LevelSequencer.instance.timeBPM);
            }
        }

        private void Flicker()
        {
            for (int i = 0; i < Random.Range(0, _animations.Length) + 1; i++)
            {
                _animations[Random.Range(0, _animations.Length)].Play(PlayMode.StopAll);
            }
        }
    }
}