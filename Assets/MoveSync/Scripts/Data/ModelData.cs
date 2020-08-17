using System;
using System.Linq;
using UnityEngine;


namespace MoveSync.ModelData
{
    [Serializable]
    public class ModelInput
    {
        public string stringValue = "";
        public PropertyName type;


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
            if (origin.type == ModelData.DURATION.TYPE) return DURATION.CopyValues(origin);
            if (origin.type == ModelData.APPEAR.TYPE) return APPEAR.CopyValues(origin);
            if (origin.type == ModelData.SIZE.TYPE) return SIZE.CopyValues(origin);
            if (origin.type == ModelData.POSITION.TYPE) return POSITION.CopyValues(origin);
            if (origin.type == ModelData.ROTATION.TYPE) return ROTATION.CopyValues(origin);
            if (origin.type == ModelData.EVENT.TYPE) return EVENT.CopyValues(origin);
            if (origin.type == ModelData.COUNT.TYPE) return COUNT.CopyValues(origin);

            return null;
        }
        
        public static ModelInput DURATION => new DURATION();
        public static ModelInput APPEAR => new APPEAR();
        public static ModelInput SIZE => new SIZE();
        public static ModelInput POSITION => new POSITION();
        public static ModelInput ROTATION => new ROTATION();
        public static ModelInput EVENT => new EVENT();
        public static ModelInput COUNT => new COUNT();
    }

    public abstract class FloatModelInput : ModelInput
    {
        public float value
        {
            get => float.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public abstract class IntModelInput : ModelInput
    {
        public int value
        {
            get => int.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public abstract class Vector3ModelInput : ModelInput
    {
        protected Vector3ModelInput() => stringValue = JsonUtility.ToJson(Vector3.zero);

        public Vector3 value
        {
            get => JsonUtility.FromJson<Vector3>(stringValue);
            set => stringValue = JsonUtility.ToJson(value);
        }
    }
    public abstract class StringModelInput : ModelInput
    {
        public string value
        {
            get => stringValue;
            set => stringValue = value;
        }
    }
    
    public class DURATION : FloatModelInput
    {
        public static PropertyName TYPE = "DURATION";
        public DURATION() { type = TYPE; }
    }
    public class APPEAR : FloatModelInput
    {
        public static PropertyName TYPE = "APPEAR";
        public APPEAR() { type = TYPE; }
    }
    public class SIZE : FloatModelInput
    {
        public static PropertyName TYPE = "SIZE";
        public SIZE() { type = TYPE; }
    }
    public class POSITION : Vector3ModelInput
    {
        public static PropertyName TYPE = "POSITION";
        public POSITION() { type = TYPE; }
        public RandomSpawnType randomSpawnType;
        public Vector3 pivot = Vector3.one;
    }
    public class ROTATION : Vector3ModelInput
    {
        public static PropertyName TYPE = "ROTATION";
        public ROTATION() { type = TYPE; }
        
        public Quaternion value
        {
            get => Quaternion.Euler(JsonUtility.FromJson<Vector3>(stringValue));
            set => stringValue = JsonUtility.ToJson(value.eulerAngles);
        }
    }
    public class COUNT : IntModelInput
    {
        public static PropertyName TYPE = "COUNT";
        public COUNT() { type = TYPE; }
    }
    public class EVENT : StringModelInput
    {
        public static PropertyName TYPE = "EVENT";
        public EVENT() { type = TYPE; }
    }
}