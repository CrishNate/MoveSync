using System.Collections.Generic;
using MoveSync.Data;
using UnityEngine;

namespace MoveSync
{
    public enum MoveSyncEvent
    {
        None,
        SkyFlicker
    }
    public class MoveSyncData : Singleton<MoveSyncData>
    {
        public ColorData colorData;
        public ShapeData shapeData;
        public ProjectileData projectileData;
        public static readonly string[] MoveSyncEvents = System.Enum.GetNames (typeof(MoveSyncEvent));
    }
}