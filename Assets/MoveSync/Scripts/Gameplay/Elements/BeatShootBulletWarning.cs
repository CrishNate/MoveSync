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
            base.Init(beatObjectData);
            
            _laserWarning.Init(spawnTimeBPM - appear, appear, beatObjectData.getModel<SIZE>().value);
        }
    }
}