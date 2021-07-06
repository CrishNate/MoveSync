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
        protected float appear;
        protected float duration;
        protected Vector3 position;
        protected float size;
        protected BaseProjectile projectile;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            position = beatObjectData.getModel<POSITION>().value;

            appear = beatObjectData.getModel<APPEAR>().value;
            duration = beatObjectData.getModel<DURATION>().value;
            size = beatObjectData.getModel<SIZE>().value;
            projectile = beatObjectData.getModel<PROJECTILE>().projectile;

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
            return beatObjectData.getModel<ROTATION>().value;
        }

        protected virtual float GetDestroyTime()
        {
            return -1f;
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();

            if (LevelSequencer.instance.timeBPM >= GetDestroyTime())
                Destroy(gameObject);
        }

        void Shoot()
        {
            float speed = beatObjectData.tryGetModel<SPEED>(out var speedModel) ? speedModel.value : 0.0f;
            Mesh shape = beatObjectData.tryGetModel<SHAPE>(out var shapeModel) ? shapeModel.mesh : null;

            BaseProjectile.ProjectileParam initParam = new BaseProjectile.ProjectileParam
            {
                instigator = gameObject,
                invokeTimeStamp = beatObjectData.time,
                duration = duration,
                appearTime = appear,
                scale = size,
                speed = speed,
                shape = shape
            };

            Instantiate(projectile.gameObject, transform.position, transform.rotation)
                .GetComponent<BaseProjectile>()
                .Init(initParam);
        }
    }
}