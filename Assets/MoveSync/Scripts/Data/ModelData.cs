using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoveSync.ModelData
{
    [Serializable]
    public enum ModelInputType
    {
        NONE,
        DURATION,
        APPEAR,
        SIZE,
        POSITION,
        ROTATION,
        EVENT,
        COUNT,
        SPEED,
        SHAPE,
        PROJECTILE,
    }
    public class ModelInputJsonConverter : JsonConverter
    {
        private readonly Type[] _types;

        public ModelInputJsonConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        bool CheckType<T>(JObject jo, JToken? type, JsonSerializer serializer, [CanBeNull] ref object obj) where T : ModelInput
        {
            string modelInputType = typeof(T).ToString();

            if (type.Value<ModelInputType>() == (ModelInputType) Enum.Parse(typeof(ModelInputType), modelInputType))
            {
                obj = jo.ToObject<T>(serializer);
                return true;
            }
            
            return false;
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jo = JObject.Load(reader);
            
            if (jo.Parent?["modelInputsData"] != null)
            {
                JToken? type = jo["type"];
                if (type != null)
                {
                    object obj = null;
                    if (CheckType<DURATION>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<APPEAR>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<SIZE>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<POSITION>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<ROTATION>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<EVENT>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<COUNT>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<SPEED>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<SHAPE>(jo, type, serializer, ref obj)) return obj;
                    if (CheckType<PROJECTILE>(jo, type, serializer, ref obj)) return obj;
                }
            }

            return null;
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }

    [Serializable]
    public class ModelInput
    {
        public string stringValue = "";
        public ModelInputType type;

        public ModelInput Clone()
        {
            return (ModelInput) MemberwiseClone();
        }
        
        public ModelInput defaultValue(string value)
        {
            stringValue = value;
            return this;
        }
        
        public ModelInput CopyValues(ModelInput origin)
        {
            type = origin.type;
            stringValue = origin.stringValue;
            return this;
        }

        public static ModelInput[] CloneInputs(ModelInput[] modelInputs)
        {
            var arrayQuery = from obj in modelInputs
                select obj.Clone();

            return arrayQuery.ToArray();
        }

        public static ModelInput RecreateRealModel(ModelInput origin)
        {
            switch (origin.type)
            {
                case ModelInputType.DURATION: return DURATION.CopyValues(origin);
                case ModelInputType.APPEAR: return APPEAR.CopyValues(origin);
                case ModelInputType.SIZE: return SIZE.CopyValues(origin);
                case ModelInputType.SPEED: return SPEED.CopyValues(origin);
                case ModelInputType.POSITION: return POSITION.CopyValues(origin);
                case ModelInputType.ROTATION: return ROTATION.CopyValues(origin);
                case ModelInputType.EVENT: return EVENT.CopyValues(origin);
                case ModelInputType.COUNT: return COUNT.CopyValues(origin);
                case ModelInputType.SHAPE: return SHAPE.CopyValues(origin);
                case ModelInputType.PROJECTILE: return PROJECTILE.CopyValues(origin);
            }

            Debug.LogErrorFormat("[ModelData] Couldn't recreate model {0}: type: {1}", origin, origin.type);
            return null;
        }
        
        // Inputs short
        public static ModelInput DURATION => new DURATION();
        public static ModelInput APPEAR => new APPEAR();
        public static ModelInput SIZE => new SIZE();
        public static ModelInput POSITION => new POSITION();
        public static ModelInput ROTATION => new ROTATION();
        public static ModelInput EVENT => new EVENT();
        public static ModelInput COUNT => new COUNT();
        public static ModelInput SPEED => new SPEED();
        public static ModelInput SHAPE => new SHAPE();
        public static ModelInput PROJECTILE => new PROJECTILE();
    }

    // Inputs types
    public abstract class FloatModelInput : ModelInput
    {
        [JsonIgnore]
        public float value
        {
            get => float.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public abstract class IntModelInput : ModelInput
    {
        [JsonIgnore]
        public int value
        {
            get => int.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public abstract class Vector3ModelInput : ModelInput
    {
        protected Vector3ModelInput() => stringValue = JsonUtility.ToJson(Vector3.zero);

        [JsonIgnore]
        public Vector3 value
        {
            get => JsonUtility.FromJson<Vector3>(stringValue);
            set => stringValue = JsonUtility.ToJson(value);
        }
    }
    public abstract class StringModelInput : ModelInput
    {
        [JsonIgnore]
        public string value
        {
            get => stringValue;
            set => stringValue = value;
        }
    }
    
    // Inputs
    public class DURATION : FloatModelInput
    {
        public static ModelInputType TYPE = ModelInputType.DURATION;
        public DURATION() { type = TYPE; }
    }
    public class APPEAR : FloatModelInput
    {
        public static ModelInputType TYPE = ModelInputType.APPEAR;
        public APPEAR() { type = TYPE; }
    }
    public class SIZE : FloatModelInput
    {
        public static ModelInputType TYPE = ModelInputType.SIZE;
        public SIZE() { type = TYPE; }
    }
    public class SPEED : FloatModelInput
    {
        public static ModelInputType TYPE = ModelInputType.SPEED;
        public SPEED() { type = TYPE; }
    }
    public class POSITION : Vector3ModelInput
    {
        public static ModelInputType TYPE = ModelInputType.POSITION;
        public POSITION() { type = TYPE; }
        [JsonIgnore]
        public RandomSpawnType randomSpawnType;
        [JsonIgnore]
        public Vector3 pivot = Vector3.one;
    }
    public class ROTATION : Vector3ModelInput
    {
        public static ModelInputType TYPE = ModelInputType.ROTATION;
        public ROTATION() { type = TYPE; }
        
        [JsonIgnore]
        public new Quaternion value
        {
            get => Quaternion.Euler(JsonUtility.FromJson<Vector3>(stringValue));
            set => stringValue = JsonUtility.ToJson(value.eulerAngles);
        }
    }
    public class COUNT : IntModelInput
    {
        public static ModelInputType TYPE = ModelInputType.COUNT;
        public COUNT() { type = TYPE; }
    }
    public class SHAPE : StringModelInput
    {
        public static ModelInputType TYPE = ModelInputType.SHAPE;
        public SHAPE() 
        { 
            type = TYPE;
            value = MoveSyncData.instance.shapeData.shapesNameList.First();
        }
        [JsonIgnore]
        public Mesh mesh => MoveSyncData.instance.shapeData.shapes[value];
    }
    public class PROJECTILE : StringModelInput
    {
        public static ModelInputType TYPE = ModelInputType.PROJECTILE;
        public PROJECTILE() 
        { 
            type = TYPE;
            value = MoveSyncData.instance.projectileData.projectilesNameList.First();
        }
        [JsonIgnore]
        public BaseProjectile projectile => MoveSyncData.instance.projectileData.projectiles[value];
    }
    public class EVENT : StringModelInput
    {
        public static ModelInputType TYPE = ModelInputType.EVENT;

        public EVENT()
        {
            type = TYPE;
            value = MoveSyncData.MoveSyncEvents.First();
        }
        [JsonIgnore]
        public MoveSyncEvent msEvent => (MoveSyncEvent)Enum.Parse(typeof(MoveSyncEvent), value);
    }
}