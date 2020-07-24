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
        TRANSFORM         = 0x00001,
        INITIAL_TRANSFORM = 0x00002,
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

    public class ObjectManager : MonoBehaviour
    {
        public static ObjectManager instance = null;

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
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance == this)
            {
                Destroy(gameObject);
            }
        }

        void AddSpawnTable(ObjectModel model)
        {
            _objectModels.Add(model.objectTag, model);
        }

        void Start()
        {
            AddSpawnTable(new ObjectModel("test_item", ModelInputUI.TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("test_item2", ModelInputUI.TRANSFORM & ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("test_item3", ModelInputUI.TRANSFORM & ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("test_item4", ModelInputUI.TRANSFORM & ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("test_item5", ModelInputUI.TRANSFORM & ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            AddSpawnTable(new ObjectModel("test_item6", ModelInputUI.TRANSFORM & ModelInputUI.INITIAL_TRANSFORM, "Assets/MoveSync/Prefab/LaserFade.prefab"));
            
            _onObjectsLoaded.Invoke();
        }

        public Dictionary<PropertyName, ObjectModel> objectModels => _objectModels;
        public ObjectModel currentObjectModel => _currentObjectModel;
    }
}
