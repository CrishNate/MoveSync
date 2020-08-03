using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [Serializable] public class UnityEventString : UnityEvent<string> { };
    [Serializable] public class UnityEventFloatParam : UnityEvent<float> {};
    public class UnityEventPropertyName : UnityEvent<PropertyName> { };
    public class UnityEventVector3 : UnityEvent<Vector3>{ };
    public class EventRandomSpawnType : UnityEvent<RandomSpawnType> { };
    public class UnityEventBeatObjectData : UnityEvent<BeatObjectData> {};
    public class UnityEventObjectUI : UnityEvent<ObjectUI>{};
}