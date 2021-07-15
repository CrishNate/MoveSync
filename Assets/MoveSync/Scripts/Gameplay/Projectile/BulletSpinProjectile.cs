using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class BulletSpinProjectile : BulletProjectile
    {
        private float _dAngle;
        
        public override void Init(ProjectileParam initParam)
        {
            base.Init(initParam);

            _dAngle = Random.Range(100, 200);
            transform.Rotate(Vector3.forward, Random.Range(0, 360), Space.Self);
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();

            transform.Rotate(Vector3.forward, _dAngle * Time.deltaTime, Space.Self);
        }
    }
}