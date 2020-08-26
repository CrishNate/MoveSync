using UnityEngine;

namespace MoveSync
{
    public class BulletShapeCircleProjectile : BulletShapeProjectile
    {
        public override Vector3 GetShapePoint(int bulletIndex)
        {
            float x = 2f * Mathf.PI * bulletIndex / count;
            return new Vector3(Mathf.Cos(x), Mathf.Sin(x)) * radius;
        }
    }
}