using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace MoveSync
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SerializablePropertyName : IEquatable<SerializablePropertyName>
    {
        internal int id;

        public static implicit operator int(SerializablePropertyName propertyName) => propertyName.id;
        public static implicit operator SerializablePropertyName(string str) => new SerializablePropertyName(str);

        public SerializablePropertyName(string name)
            : this(PropertyNameFromString(name))
        { }

        public SerializablePropertyName(SerializablePropertyName other)
        {
            id = other.id;
        }

        public SerializablePropertyName(int id)
        {
            this.id = id;
        }
        
        public static bool IsNullOrEmpty(SerializablePropertyName prop) { return prop.id == 0; }

        public static bool operator==(SerializablePropertyName lhs, SerializablePropertyName rhs)
        {
            return lhs.id == rhs.id;
        }

        public static bool operator!=(SerializablePropertyName lhs, SerializablePropertyName rhs)
        {
            return lhs.id != rhs.id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public override bool Equals(object other)
        {
            return other is PropertyName && Equals((SerializablePropertyName)other);
        }

        public bool Equals(SerializablePropertyName other)
        {
            return this == other;
        }

        private static SerializablePropertyName PropertyNameFromString(string str)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            int id = BitConverter.ToInt32(hashed, 0);
            
            return new SerializablePropertyName(id);
        }
    }
}