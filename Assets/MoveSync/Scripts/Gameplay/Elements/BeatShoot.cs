using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class BeatShoot : BeatObject
    {
        [SerializeField] protected BaseProjectile projectile;
        protected float appear;
        protected float duration;
        
        private float _size;
        private Vector3 _position;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _position = beatObjectData.getModel<POSITION>().value;

            appear = beatObjectData.getModel<APPEAR>().value;
            duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            
            transform.position = _position;
            transform.rotation = GetRotationByTargetState();

            Shoot();
            
            if (GetDestroyTime() < 0)
                Destroy(gameObject);
        }
        
        protected virtual Quaternion GetRotationByTargetState()
        {
            return Quaternion.LookRotation(PlayerBehaviour.instance.transform.position - transform.position);
        }

        protected virtual float GetDestroyTime()
        {
            return -1f;
        }

        protected override void Update()
        {
            base.Update();

            if (LevelSequencer.instance.timeBPM >= GetDestroyTime())
                Destroy(gameObject);
        }

        void Shoot()
        {
            Instantiate(projectile.gameObject, transform.position, transform.rotation)
                .GetComponent<BaseProjectile>()
                .Init(gameObject, beatObjectData.time, duration, appear, size);
        }

        
        public float size => _size;
    }
}