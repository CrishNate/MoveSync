using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class AnimationMusicSequence : MonoBehaviour
    {
        private Animator _animator;
        

        void Start()
        {
            _animator = GetComponent<Animator>();
            
            EventManager.instance.BindEvent("event_swim", PlayAnim);
        }

        void PlayAnim(float timeBPM)
        {
            _animator.speed = LevelSequencer.instance.toBPM;

            float difference = LevelSequencer.instance.timeBPM - timeBPM;
            _animator.Play("Base Layer.Swim", 0, difference * LevelSequencer.instance.toTime);
        }
    }
}