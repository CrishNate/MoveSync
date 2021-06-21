using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoveSync.Data
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "MoveSync/ProjectileData", order = 0)]
    public class ProjectileData : ScriptableObject
    {
        public SerializableDictionary<string, BaseProjectile> projectiles;
        public List<string> projectilesNameList => projectiles.Keys.ToList();
    }
}