using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BulletProjectile : BaseProjectile
    {
        enum AppearStates
        {
            Appear,
            Stay,
            Disappear
        }

        
        private float _distance;
        private float _speed;
        private AppearStates _appearStates = AppearStates.Appear;

        private static float _distanceOffset = 1.0f;
        private static float _appearSpeed = 1.0f;
        private static float _disappearAfterTimeBPM = 10.0f;
        
        
        public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime,
            float scale)
        {
            base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);
            
            transform.localScale = Vector3.zero;
            _distance = (PlayerBehaviour.instance.transform.position - transform.position).magnitude - _distanceOffset;
            _speed = _distance / (appearTime * LevelSequencer.instance.toTime);
        }

        protected virtual void Update()
        {
            transform.position +=
                transform.forward * (_speed * Time.deltaTime * LevelSequencer.instance.audioSource.pitch);

            AppearUpdate();
            
            if (LevelSequencer.instance.timeBPM < (timeStamp - appearTime)
                || LevelSequencer.instance.timeBPM > (timeStamp + _disappearAfterTimeBPM + _appearSpeed))
            {
                Destroy(gameObject);
            }
        }

        void AppearUpdate()
        {
            float dTime;
            switch (_appearStates)
            {
                case AppearStates.Appear:
                    dTime = (LevelSequencer.instance.timeBPM - (timeStamp - appearTime)) / _appearSpeed;
                    dTime = Mathf.Min(dTime, 1f);
                    transform.localScale = Vector3.one * (scale * dTime);

                    if (LevelSequencer.instance.timeBPM > timeStamp - appearTime + _appearSpeed)
                        _appearStates = AppearStates.Stay;
                    break;
                
                case AppearStates.Stay:
                    if (LevelSequencer.instance.timeBPM > timeStamp + _disappearAfterTimeBPM)
                        _appearStates = AppearStates.Disappear;
                    break;
                    
                case AppearStates.Disappear:
                    dTime = (LevelSequencer.instance.timeBPM - (timeStamp + _disappearAfterTimeBPM)) / _appearSpeed;
                    dTime = 1 - dTime;
                    transform.localScale = Vector3.one * (scale * dTime);
                    break;
            }
        }
    }
}