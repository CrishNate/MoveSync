using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BulletProjectile : BaseProjectile
    {
        private float _distance;
        private float _speed;

        private static float _distanceOffset = 1.0f;
        private static float _disappearSpeed = 1.0f;
        private static float _disappearAfterTimeBPM = 10.0f;

        public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime,
            float scale)
        {
            base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);
            
            transform.localScale = Vector3.one * scale;
            _distance = (PlayerBehaviour.instance.transform.position - transform.position).magnitude - _distanceOffset;
            _speed = _distance / (appearTime * LevelSequencer.instance.toTime);
        }

        void Update()
        {
            transform.position +=
                transform.forward * (_speed * Time.deltaTime * LevelSequencer.instance.audioSource.pitch);

            
            if (LevelSequencer.instance.timeBPM > (_timeStamp + _disappearAfterTimeBPM))
            {
                // disappear effect
                float disappear = (LevelSequencer.instance.timeBPM - (_timeStamp + _disappearAfterTimeBPM)) / _disappearSpeed;
                disappear = 1 - disappear;
                transform.localScale = Vector3.one * (_scale * disappear);
            }
            
            if (LevelSequencer.instance.timeBPM < _timeStamp
                || LevelSequencer.instance.timeBPM > (_timeStamp + _disappearAfterTimeBPM + _disappearSpeed))
            {
                Destroy(gameObject);
            }
        }
    }
}