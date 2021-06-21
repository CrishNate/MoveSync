using MoveSync.Data;
using UnityEngine;

namespace MoveSync
{
    public class MoveSyncData : Singleton<MoveSyncData>
    {
        public ColorData colorData;
        public ShapeData shapeData;
        public ProjectileData projectileData;
    }
}