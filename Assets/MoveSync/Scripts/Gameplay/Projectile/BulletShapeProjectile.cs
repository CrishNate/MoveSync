using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BulletShapeProjectile : BulletProjectile
    {
        [SerializeField] protected int count = 10;
        [SerializeField] protected float dRotation;
        [SerializeField] protected float dRadius;
        [SerializeField] protected float radius;

        [SerializeField] private GameObject _bulletInstanceModel;
        private Dictionary<GameObject, Vector3> _directions = new Dictionary<GameObject, Vector3>();
        private float _time;


        public virtual Vector3 GetShapePoint(int bulletIndex)
        {
            return Vector3.zero;
        }
        
        public override void Init(GameObject instigator, float invokeTimeStamp, float _duration, float _appearTime, float _scale, float _speed)
        {
            base.Init(instigator, invokeTimeStamp, _duration, _appearTime, _scale, _speed);

            for (int i = 0; i < count; i++)
            {
                GameObject bulletModel = Instantiate(_bulletInstanceModel, transform);
                bulletModel.transform.localPosition = GetShapePoint(i);
                
                _directions.Add(bulletModel, bulletModel.transform.localPosition);
            }
        }

        protected override void Update()
        {
            base.Update();

            _time += Time.deltaTime * LevelSequencer.instance.toBPM;
            if (Math.Abs(dRotation) > 0)
            {
                transform.RotateAround(transform.forward,
                    dRotation * Time.deltaTime * LevelSequencer.instance.toBPM *
                    LevelSequencer.instance.audioSource.pitch);
            }

            if (Math.Abs(dRadius) > 0)
            {
                float newRadius = radius + Mathf.Sin(_time) * dRadius;

                foreach (var direction in _directions)
                {
                    direction.Key.transform.localPosition = direction.Value * newRadius;
                }
            }
        }
    }
}