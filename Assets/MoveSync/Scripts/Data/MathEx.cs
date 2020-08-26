using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public static class MathEx
    {
        public static float DistanceToLine(Vector3 startingPoint, Vector3 direction, Vector3 point)
        {
            Ray ray = new Ray(startingPoint, direction);
            
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static void ExecuteInIsometricSphere(UnityAction<Vector3> actionVector, int divisions)
        {
            int ia, ib, nb;
            float x, y, z, r;
            float a, b, da, db;
            da = Mathf.PI / (divisions - 1);
            for (a = -0.5f * Mathf.PI, ia = 0; ia < divisions; ia++, a += da)
            {
                r = Mathf.Cos(a);
                z = Mathf.Sin(a);
                nb = Mathf.CeilToInt(2.0f * Mathf.PI * r / da);
                db = 2.0f * Mathf.PI / nb;
                if ((ia == 0) || (ia == divisions - 1))
                {
                    nb = 1;
                    db = 0.0f;
                }

                for (b = 0.0f, ib = 0; ib < nb; ib++, b += db)
                {
                    x = r * Mathf.Cos(b);
                    y = r * Mathf.Sin(b);
                    actionVector.Invoke(new Vector3(x, y, z));
                }
            }
        }
    }
}