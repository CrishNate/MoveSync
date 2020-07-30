using System;
using System.Collections.Generic;
using UnityEngine;


namespace MoveSync
{
    [Serializable]
    public class SerializableGuid
    {
        [NonSerialized] private static int _lastId;
        public int value;

        public static implicit operator SerializableGuid(int index) => new SerializableGuid(index);
        public static implicit operator int(SerializableGuid guid) => guid.value;

        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid();
        }

        SerializableGuid()
        {
            value = ++_lastId;
        }

        SerializableGuid(int id)
        {
            _lastId = Mathf.Max(_lastId, id);
        }
    }
}