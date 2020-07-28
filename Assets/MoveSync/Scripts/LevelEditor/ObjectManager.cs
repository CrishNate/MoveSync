using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoveSync
{
    [Flags]
    public enum ModelInputUI
    {
        NONE              = 0,
        TRANSFORM         = 1,
        INITIAL_TRANSFORM = 2,
        APPEAR            = 4,
        STAY              = 8,
        ANIMATION         = 16,
    }
    
    public struct ObjectModel
    {
        public ObjectModel(PropertyName _objectTag, ModelInputUI _inputUi, string _prefabPath)
        {
            objectTag = _objectTag;
            inputUi = _inputUi;
            prefabPath = _prefabPath;
        }
        
        public PropertyName objectTag;
        public ModelInputUI inputUi;
        public string prefabPath;
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
            AddSpawnTable(new ObjectModel("laser_shooter", ModelInputUI.TRANSFORM | ModelInputUI.INITIAL_TRANSFORM | ModelInputUI.APPEAR | ModelInputUI.STAY, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("shooter", ModelInputUI.TRANSFORM | ModelInputUI.INITIAL_TRANSFORM | ModelInputUI.APPEAR, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("explosion", ModelInputUI.TRANSFORM | ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            
            _onObjectsLoaded.Invoke();
        }

        public Dictionary<PropertyName, ObjectModel> objectModels => _objectModels;
        public ObjectModel currentObjectModel => _currentObjectModel;
    }
}
