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
        protected Vector3 position;
        protected float size;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            position = beatObjectData.getModel<POSITION>().value;

            appear = beatObjectData.getModel<APPEAR>().value;
            duration = beatObjectData.getModel<DURATION>().value;
            size = beatObjectData.getModel<SIZE>().value;
            
            transform.position = GetSpawnPosition();
            transform.rotation = GetRotationByTargetState();

            Shoot();
            
            if (GetDestroyTime() < 0)
                Destroy(gameObject);
        }
        
        protected virtual Vector3 GetSpawnPosition()
        {
            return position;
        }
        protected virtual Quaternion GetRotationByTargetState()
        {
            return Quaternion.LookRotation(PlayerBehaviour.instance.GetRandomPointNearPlayer() - transform.position);
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
            float speed = beatObjectData.tryGetModel<SPEED>(out var speedModel) ? speedModel.value : 0.0f;
            
            Instantiate(projectile.gameObject, transform.position, transform.rotation)
                .GetComponent<BaseProjectile>()
                .Init(gameObject, beatObjectData.time, duration, appear, size, speed);
        }
    }
}