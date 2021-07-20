using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class BeatShootBulletWarning : BeatShootBullet
    {
        [SerializeField] private LaserWarning _laserWarning;

        public override void Init(BeatObjectData beatObjectData)
        {
            shootOnAwake = false;
            
            base.Init(beatObjectData);
            
            _laserWarning.Init(spawnTimeBPM, appear, beatObjectData.getModel<SIZE>().value);
        }

        protected override void PreShoot(ref BaseProjectile.ProjectileParam initParam)
        {
            initParam.speed = 25.0f;
            initParam.appearTime = 1f;
            initParam.duration = 0.25f;
        }
        
        protected override void OnTriggered()
        {
            base.OnTriggered();
            
            Shoot(transform.position, transform.rotation, beatObjectData.time);
        }
        
        protected override float GetDestroyTime()
        {
            return beatObjectData.time;
        }
    }
}