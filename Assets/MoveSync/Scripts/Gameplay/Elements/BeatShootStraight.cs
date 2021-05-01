using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class BeatShootStraight : BeatShootBullet
    {
        protected override Quaternion GetRotationByTargetState() 
        {
            return Quaternion.LookRotation(PlayerBehaviour.instance.transform.position - transform.position);
        }
    }
}