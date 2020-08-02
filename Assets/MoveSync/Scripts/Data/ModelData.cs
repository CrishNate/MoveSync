using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


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
            ModelInput[] newModelInputs = new ModelInput[modelInputs.Length];

            for (int i = 0; i < newModelInputs.Length; i++)
            {
                newModelInputs[i] = modelInputs[i].Clone();
            }
            return newModelInputs;
        }

        public static ModelInput RecreateRealModel(ModelInput origin)
        {
            if (origin.type == ModelData.DURATION.TYPE) return DURATION.CopyValues(origin);
            if (origin.type == ModelData.APPEAR.TYPE) return APPEAR.CopyValues(origin);
            if (origin.type == ModelData.SIZE.TYPE) return SIZE.CopyValues(origin);
            if (origin.type == ModelData.TRANSFORM.TYPE) return TRANSFORM.CopyValues(origin);
            if (origin.type == ModelData.EVENT.TYPE) return EVENT.CopyValues(origin);

            return null;
        }
        
        public static ModelInput DURATION => new DURATION();
        public static ModelInput APPEAR => new APPEAR();
        public static ModelInput SIZE => new SIZE();
        public static ModelInput TRANSFORM => new TRANSFORM();
        public static ModelInput EVENT => new EVENT();
    }

    public class DURATION : ModelInput
    {
        public static PropertyName TYPE = "DURATION";
        public DURATION() { type = TYPE; }

        public float value
        {
            get => float.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public class APPEAR : ModelInput
    {
        public static PropertyName TYPE = "APPEAR";
        public APPEAR() { type = TYPE; }

        public float value
        {
            get => float.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public class SIZE : ModelInput
    {
        public static PropertyName TYPE = "SIZE";
        public SIZE() { type = TYPE; }
        
        public float value
        {
            get => float.Parse(stringValue);
            set => stringValue = value.ToString();
        }
    }
    public class TRANSFORM : ModelInput
    {
        public static PropertyName TYPE = "TRANSFORM";
        public TRANSFORM() { type = TYPE; }

        public ExTransformData value
        {
            get => JsonUtility.FromJson<ExTransformData>(stringValue);
            set => stringValue = JsonUtility.ToJson(value);
        }
    }
    public class EVENT : ModelInput
    {
        public static PropertyName TYPE = "TRANSFORM";
        public EVENT() { type = TYPE; }

        public string value
        {
            get => value;
            set => stringValue = value;
        }
    }
}