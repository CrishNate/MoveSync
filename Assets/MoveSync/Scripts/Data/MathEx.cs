using UnityEngine;

namespace MoveSync
{
    public static class MathEx
    {
        public static float DistanceToLine(Vector3 startingPoint, Vector3 direction, Vector3 point)
        {
            Ray ray = new Ray(startingPoint, direction);
            
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }
    }
}