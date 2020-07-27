using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BulletProjectile : BaseProjectile
    {
        private float _distance;
        private float _speed;
        private float _distanceOffset = 1.0f;

        public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime,
            float scale)
        {
            base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);

            _distance = (Camera.main.transform.position - transform.position).magnitude - _distanceOffset;

            transform.localScale = Vector3.one * scale;
            _speed = _distance / (appearTime * LevelSequencer.instance.toTime);
        }

        void Update()
        {
            transform.position +=
                transform.forward * (_speed * Time.deltaTime * LevelSequencer.instance.audioSource.pitch);

            if (LevelSequencer.instance.timeBPM < _timeStamp
                || LevelSequencer.instance.timeBPM > _timeStamp + 10)
            {
                Destroy(gameObject);
            }
        }
    }
}