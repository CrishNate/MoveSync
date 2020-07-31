using System;
using UnityEngine;

namespace MoveSync.ModelData
{
    [Serializable]
    public class ModelInput
    {
        public string stringValue;
        public PropertyName type;


        public ModelInput Clone() { return (ModelInput) MemberwiseClone(); }
        
        public ModelInput defaultValue(string value)
        {
            stringValue = value;
            return this;
        }

        public static ModelInput[] CloneInputs(ModelInput[] modelInputs)
        {
            for (int i = 0; i < modelInputs.Length; i++)
            {
                modelInputs[i] = modelInputs[i].Clone();
            }
            return modelInputs;
        }

        
        public static ModelInput DURATION = new DURATION();
        public static ModelInput APPEAR = new APPEAR();
        public static ModelInput TRANSFORM = new TRANSFORM();
        public static ModelInput INITIAL_TRANSFORM = new INITIAL_TRANSFORM();
        public static ModelInput SIZE = new SIZE();
        public static ModelInput ANIMATION = new ANIMATION();
    }

    public class DURATION : ModelInput
    {
        public static PropertyName TYPE = "DURATION";
        public DURATION() { type = TYPE; }

        public static float Get(ModelInput model) { return float.Parse(model.stringValue);}
        public static void Set(ModelInput model, float value) { model.stringValue = value.ToString(); }
    }
    public class APPEAR : ModelInput
    {
        public static PropertyName TYPE = "APPEAR";
        public APPEAR() { type = TYPE; }

        public static float Get(ModelInput model) { return float.Parse(model.stringValue);}
        public static void Set(ModelInput model, float value) { model.stringValue = value.ToString(); }
    }
    public class SIZE : ModelInput
    {
        public static PropertyName TYPE = "SIZE";
        public SIZE() { type = TYPE; }
        
        public static float Get(ModelInput model) { return float.Parse(model.stringValue);}
        public static void Set(ModelInput model, float value) { model.stringValue = value.ToString(); }
    }
    public class TRANSFORM : ModelInput
    {
        public static PropertyName TYPE = "TRANSFORM";
        public TRANSFORM() { type = TYPE; }
        
        public static ExTransformData Get(ModelInput model) { return JsonUtility.FromJson<ExTransformData>(model.stringValue); }
        public static void Set(ModelInput model, ExTransformData value) { model.stringValue = JsonUtility.ToJson(value); }
    }
    public class INITIAL_TRANSFORM : ModelInput
    {
        public static PropertyName TYPE = "INITIAL_TRANSFORM";
        public INITIAL_TRANSFORM() { type = TYPE; }
        
        public static ExTransformData Get(ModelInput model) { return JsonUtility.FromJson<ExTransformData>(model.stringValue); }
        public static void Set(ModelInput model, ExTransformData value) { model.stringValue = JsonUtility.ToJson(value); }
    }
    public class ANIMATION : ModelInput
    {
        public static PropertyName TYPE = "ANIMATION";
        public ANIMATION() { type = TYPE; }
    }
}