using UnityEngine;

namespace MoveSync
{
    public class BulletShapePrismProjectile : BulletShapeProjectile
    {
        public override Vector3 GetShapePoint(int bulletIndex)
        {
            float div = 2f * Mathf.PI * 0.33333333f;
            Vector3 p1 = new Vector3(Mathf.Cos(div), Mathf.Sin(div));
            Vector3 p2 = new Vector3(Mathf.Cos(div * 2), Mathf.Sin(div * 2));
            Vector3 p3 = new Vector3(Mathf.Cos(div * 3), Mathf.Sin(div * 3));

            float x = (float) bulletIndex / count;
            if (x < 0.33333333f) return Vector3.Lerp(p1, p2, x * 3 % 1f);
            if (x < 0.66666666f) return Vector3.Lerp(p2, p3, x * 3 % 1f);
            return Vector3.Lerp(p3, p1, x * 3 % 1f);
        }
    }
}