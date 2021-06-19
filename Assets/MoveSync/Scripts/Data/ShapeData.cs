using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoveSync.Data
{
    [CreateAssetMenu(fileName = "ShapeData", menuName = "MoveSync/ShapeData", order = 0)]
    public class ShapeData : ScriptableObject
    {
        public SerializableDictionary<string, Mesh> shapes;
        public List<string> shapesNameList => shapes.Keys.ToList();
    }
}