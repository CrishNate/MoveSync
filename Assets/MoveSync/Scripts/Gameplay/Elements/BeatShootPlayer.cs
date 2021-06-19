using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatShootBullet : BeatShoot
    {
        private static float _distanceFromTarget = 1.0f;
        
        
        protected override Vector3 GetSpawnPosition()
        {
            float speed = beatObjectData.getModel<SPEED>().value;
            Vector3 dir = (position - PlayerBehaviour.instance.transform.position);
            float distance = dir.magnitude;
            dir = dir.normalized;
            
            return position + dir * (appear * speed - distance + _distanceFromTarget);
        }
    }
}