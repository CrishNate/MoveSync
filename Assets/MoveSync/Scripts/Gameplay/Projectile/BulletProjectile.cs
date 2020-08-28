using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BulletProjectile : BaseProjectile
    {
        enum FadeState
        {
            Appear,
            Stay,
            Disappear
        }
        
        private FadeState _fadeState = FadeState.Appear;

        private static float _appearSpeed = 1.0f;
        private static float _disappearAfterTimeBPM = 10.0f;
        
        
        public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime, float scale, float speed)
        {
            base.Init(instigator, invokeTimeStamp, duration, appearTime, scale, speed);
            
            transform.localScale = Vector3.zero;
        }

        protected virtual void Update()
        {
            transform.position += transform.forward * (speed * Time.deltaTime *
                                                       LevelSequencer.instance.audioSource.pitch *
                                                       LevelSequencer.instance.toBPM);

            FadeUpdate();
            
            if (LevelSequencer.instance.timeBPM < (timeStamp - appearTime)
                || LevelSequencer.instance.timeBPM > (timeStamp + _disappearAfterTimeBPM + _appearSpeed))
            {
                Destroy(gameObject);
            }
        }

        void FadeUpdate()
        {
            float dTime;
            switch (_fadeState)
            {
                case FadeState.Appear:
                    dTime = (LevelSequencer.instance.timeBPM - (timeStamp - appearTime)) / _appearSpeed;
                    dTime = Mathf.Min(dTime, 1f);
                    transform.localScale = Vector3.one * (scale * dTime);

                    if (LevelSequencer.instance.timeBPM > timeStamp - appearTime + _appearSpeed)
                        _fadeState = FadeState.Stay;
                    break;
                
                case FadeState.Stay:
                    if (LevelSequencer.instance.timeBPM > timeStamp + _disappearAfterTimeBPM)
                        _fadeState = FadeState.Disappear;
                    break;
                    
                case FadeState.Disappear:
                    dTime = (LevelSequencer.instance.timeBPM - (timeStamp + _disappearAfterTimeBPM)) / _appearSpeed;
                    dTime = 1 - dTime;
                    transform.localScale = Vector3.one * (scale * dTime);
                    break;
            }
        }
    }
}