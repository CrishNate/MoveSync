using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MoveSync.ModelData;

namespace MoveSync
{
   public struct ObjectModel
   {
        public ObjectModel(PropertyName _objectTag, ModelInput[] _modelInput, string _prefabPath)
        {
            objectTag = _objectTag;
            modelInput = _modelInput;
            prefabPath = _prefabPath;
            prefab = Resources.Load<GameObject>(prefabPath);
        }
        
        public PropertyName objectTag;
        public ModelInput[] modelInput;
        public string prefabPath;
        public GameObject prefab;
   }

    public class ObjectManager : Singleton<ObjectManager>
    {
        [SerializeField] private UnityEvent _onObjectsLoaded;

        private Dictionary<PropertyName, ObjectModel> _objectModels = new Dictionary<PropertyName, ObjectModel>();
        private ObjectModel _currentObjectModel;


        public void SetCurrentObject(string objectTag)
        {
            SetCurrentObject(new PropertyName(objectTag));
        }
        
        public void SetCurrentObject(PropertyName objectTag)
        {
            _currentObjectModel = _objectModels[objectTag];
        }

        void AddSpawnTable(ObjectModel model)
        {
            _objectModels.Add(model.objectTag, model);
        }

        void Start()
        {
            //AddSpawnTable(new ObjectModel("laser_shooter", new[] { ModelInput.APPEAR.defaultValue("2"), ModelInput.DURATION.defaultValue("1") }, "MoveSync/Prefab/LaserFade.prefab"));
            //AddSpawnTable(new ObjectModel("shooter", new[] { ModelInput.APPEAR.defaultValue("2") } , "MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("explosion", new ModelInput[] {ModelInput.APPEAR.defaultValue("2"), ModelInput.SIZE.defaultValue("1")}, "MoveSync/BeatObjects/ProjectileSphereExplosion"));
            
            _onObjectsLoaded.Invoke();
        }

        public Dictionary<PropertyName, ObjectModel> objectModels => _objectModels;
        public ObjectModel currentObjectModel => _currentObjectModel;
    }
}
