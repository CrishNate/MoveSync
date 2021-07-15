using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class BulletRotateProjectile : BulletProjectile
    {
        private Vector3 _dRotation;
        
        public override void Init(ProjectileParam initParam)
        {
            base.Init(initParam);

            _dRotation = Random.insideUnitSphere.normalized * Random.Range(30, 60);
            transform.rotation = Random.rotation;
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();

            transform.Rotate(_dRotation * Time.deltaTime);
        }
    }
}