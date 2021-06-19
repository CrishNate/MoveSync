using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [Serializable] public class UnityEventString : UnityEvent<string> { };
    [Serializable] public class UnityEventFloatParam : UnityEvent<float> {};
    [Serializable] public class UnityEventIntParam : UnityEvent<int> {};
    [Serializable] public class UnityEventBoolParam : UnityEvent<bool> {};
    public class UnityEventPropertyName : UnityEvent<PropertyName> { };
    public class UnityEventVector3 : UnityEvent<Vector3>{ };
    public class UnityEventBeatObjectData : UnityEvent<BeatObjectData> {};
    public class UnityEventObjectUI : UnityEvent<ObjectUI>{};
    
    public enum RandomSpawnType
    {
        None,
        Line,
        Rect,
        Circle,
        Sphere
    }
}