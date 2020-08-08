using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace MoveSync
{
    [Serializable]
    public class SerializableGuid : ISerializationCallbackReceiver
    {
        public int value;
        private static HashSet<int> _takenIds = new HashSet<int>();
        private static int maxInt = 2147483647;
        
        
        public static implicit operator int(SerializableGuid guid) => guid.value;
        
        public static SerializableGuid NewGuid()
        {
            int id;
            Random rnd = new Random();
            do
            {
                id = rnd.Next(-maxInt, maxInt);
            } while (!_takenIds.Add(id));

            return new SerializableGuid{ value = id };
        }

        public void OnBeforeSerialize()
        { }

        public void OnAfterDeserialize()
        {
            _takenIds.Add(value);
        }

        ~SerializableGuid()
        {
            _takenIds.Remove(value);
        }
    }
}