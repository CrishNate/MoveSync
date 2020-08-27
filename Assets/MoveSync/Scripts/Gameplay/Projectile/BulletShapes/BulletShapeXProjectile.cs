using UnityEngine;

namespace MoveSync
{
    public class BulletShapeXProjectile: BulletShapeProjectile
    {
        // count should be 4*n+1
        public override Vector3 GetShapePoint(int bulletIndex)
        {
            float ceil = Mathf.Ceil(count * .5f);
            
            if (bulletIndex < ceil)
            {
                float d = bulletIndex / (ceil - 1f);
                return Vector3.up * ((0.5f - d) * radius);
            }
            else
            {
                float d = (bulletIndex - ceil) / (ceil - 1);

                if (d < 0.5)
                    return Vector3.right * ((0.5f - d) * radius);
                else
                    return Vector3.right * ((d - 1f) * radius);
            }
        }
    }
}