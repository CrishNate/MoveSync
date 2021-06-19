using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatShootPlayer : BeatShoot
    {
        protected override Quaternion GetRotationByTargetState() 
        {
            return Quaternion.LookRotation(PlayerBehaviour.instance.GetRandomPointNearPlayer() - transform.position);
        }
    }
}