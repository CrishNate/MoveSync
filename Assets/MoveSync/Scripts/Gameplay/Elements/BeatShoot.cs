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
        protected int count;
        protected BaseProjectile projectile;

        protected bool shootOnAwake = true;

        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            position = beatObjectData.getModel<POSITION>().value;

            appear = beatObjectData.getModel<APPEAR>().value;
            duration = beatObjectData.getModel<DURATION>().value;
            size = beatObjectData.getModel<SIZE>().value;
            projectile = beatObjectData.getModel<PROJECTILE>().projectile;

            if (beatObjectData.tryGetModel(out COUNT modelCount))
                count = modelCount.value;

            transform.position = GetSpawnPosition();
            transform.rotation = GetRotationByTargetState();

            if (shootOnAwake)
                Shoot(transform.position, transform.rotation, beatObjectData.time);

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

        protected virtual void PreShoot(ref BaseProjectile.ProjectileParam initParam)
        { }

        protected void Shoot(Vector3 position, Quaternion direction, float invokeTimeStamp)
        {
            float speed = beatObjectData.tryGetModel<SPEED>(out var speedModel) ? speedModel.value : 0.0f;
            Mesh shape = beatObjectData.tryGetModel<SHAPE>(out var shapeModel) ? shapeModel.mesh : null;

            BaseProjectile.ProjectileParam initParam = new BaseProjectile.ProjectileParam
            {
                instigator = gameObject,
                invokeTimeStamp = invokeTimeStamp,
                duration = duration,
                appearTime = appear,
                size = size,
                speed = speed,
                shape = shape
            };

            PreShoot(ref initParam);

            Instantiate(projectile.gameObject, position, direction)
                .GetComponent<BaseProjectile>()
                .Init(initParam);
        }
    }
}