using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MoveSync.ModelData;
using Newtonsoft.Json;
using UnityEngine;

namespace MoveSync
{
        
    [Serializable]
    public class BeatObjectData
    {
        public SerializableGuid id;
        public float time = -1;
        // editor
        public int editorLayer;
        // custom data
        public ModelInput[] modelInputsData;
        private Dictionary<Type, ModelInput> _modelInputs;
        
        [JsonProperty("objectTag")] private string _objectTagIntern; // used for saving object tag
        private PropertyName _objectTag;
        
        public BeatObjectData()
        { }

        [OnSerializing]
        public void OnBeforeSerialize(StreamingContext context)
        {
            if (PropertyName.IsNullOrEmpty(_objectTag)) 
                return;
            
            _objectTagIntern = ObjectManager.ObjectName(_objectTag);
        }

        [OnDeserialized]
        public void OnAfterDeserialize(StreamingContext context)
        {
            _objectTag = _objectTagIntern;
            _objectTagIntern = null;
            
            FixModelInputs();
        }
        
        public BeatObjectData(PropertyName objectTag, SerializableGuid id, int editorLayer, ModelInput[] modelInputsData)
        {
            this._objectTag = objectTag;
            this.id = id;
            this.editorLayer = editorLayer;
            this.modelInputsData = modelInputsData;
            
            _modelInputs = new Dictionary<Type, ModelInput>();
            foreach (var m in modelInputsData)
            {
                _modelInputs.Add(m.GetType(), m);
            }
        }
        public BeatObjectData Clone()
        {
            BeatObjectData other = new BeatObjectData
            {
                _objectTag = _objectTag,
                id = SerializableGuid.NewGuid(),
                modelInputsData = ModelInput.CloneInputs(modelInputsData)
            };
            
            other._modelInputs = new Dictionary<Type, ModelInput>();
            for (int i = 0; i < other.modelInputsData.Length; i++)
            {
                ModelInput m = other.modelInputsData[i];
                other._modelInputs.Add(m.GetType(), m);
            }
            
            return other;
        }
        
        void FixModelInputs()
        {
            if (_modelInputs != null) return;
            
            _modelInputs = new Dictionary<Type, ModelInput>();
            for (int i = 0; i < modelInputsData.Length; i++)
            {
                ModelInput m = modelInputsData[i] = ModelInput.RecreateRealModel(modelInputsData[i]);
                _modelInputs.Add(m.GetType(), m);
            }
        }
        
        public bool hasModel<T>() where T : ModelInput
        {
            return _modelInputs.ContainsKey(typeof(T));
        }
        public bool hasModel(Type type)
        {
            return _modelInputs.ContainsKey(type);
        }
        public T getModel<T>() where T : ModelInput
        {
            return (T)_modelInputs[typeof(T)];
        }
        public bool tryGetModel<T>(out T modelInput) where T : ModelInput
        {
            bool result = _modelInputs.TryGetValue(typeof(T), out var tempModelInput);
            modelInput = (T)tempModelInput;
            return result;
        }
        public bool tryGetModel(Type type, out ModelInput modelInput)
        {
            return _modelInputs.TryGetValue(type, out modelInput);
        }
        
        public float spawnTime
        {
            get
            {
                if (tryGetModel<APPEAR>(out var appear)) 
                    return time - appear.value;

                return time;
            }
        }
        public float durationTime
        {
            get
            {
                if (tryGetModel<DURATION>(out var durationModel)) 
                    return time + durationModel.value;

                return time;
            }
        }

        [JsonIgnore] 
        public PropertyName ObjectTag => _objectTag;
    }
}