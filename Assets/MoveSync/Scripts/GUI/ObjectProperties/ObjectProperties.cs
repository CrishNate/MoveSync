using System;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectProperties : Singleton<ObjectProperties>
    {
        public Dictionary<int, SelectedObjectData> SelectedObjects;
        public UnityEventBeatObjectData onSelected = new UnityEventBeatObjectData();
        public UnityEventBeatObjectData onDeselected = new UnityEventBeatObjectData();

        [SerializeField] private Text _itemTag;
        [SerializeField] private GameObject _objectProperty;
        [SerializeField] private GameObject _objectPropertyFloatInstance;
        [SerializeField] private GameObject _objectPropertyIntInstance;
        [SerializeField] private GameObject _objectPropertyStringInstance;
        [SerializeField] private GameObject _objectPropertyVector3Instance;
        [SerializeField] private GameObject _objectPropertyPositionInstance;
        [SerializeField] private GameObject _objectPropertyShapeInstance;
        [SerializeField] private GameObject _objectPropertyProjectileInstance;
        
        private List<ObjectProperty> _propertiesObjects = new List<ObjectProperty>();
        private BeatObjectData _selectedObject;
        
        
        public void Select(BeatObjectData beatObjectData)
        {
            if (_selectedObject != null)
                onDeselected.Invoke(_selectedObject);

            _selectedObject = beatObjectData;
            onSelected.Invoke(_selectedObject);
            OpenProperties();
        }
        
        public void WipeSelections()
        {
            if (_selectedObject != null)
                onDeselected.Invoke(_selectedObject);
            
            _selectedObject = null;
            CloseProperties();
        }

        void OpenProperties()
        {
            _objectProperty.SetActive(true);

            ConstructProperties();
        }

        void CloseProperties()
        {
            _objectProperty.SetActive(false);
        }

        GameObject GetInstanceProperty(ModelInput modelInput)
        {
            if (modelInput is FloatModelInput) return _objectPropertyFloatInstance;
            if (modelInput is IntModelInput) return _objectPropertyIntInstance;
            if (modelInput is POSITION) return _objectPropertyPositionInstance;
            if (modelInput is Vector3ModelInput) return _objectPropertyVector3Instance;
            if (modelInput is SHAPE) return _objectPropertyShapeInstance;
            if (modelInput is PROJECTILE) return _objectPropertyProjectileInstance;
            if (modelInput is StringModelInput) return _objectPropertyStringInstance;

            return null;
        }

        void ConstructProperties()
        {
            _itemTag.text = LevelDataManager.PropertyNameToString(_selectedObject.objectTag);
            
            foreach (var propertiesObject in _propertiesObjects)
            {
                Destroy(propertiesObject.gameObject);
            }
            _propertiesObjects.Clear();
            
            foreach (var modelInput in _selectedObject.modelInputsData)
            {
                GameObject instanceProperty = GetInstanceProperty(modelInput);
                
                GameObject propertyObject = Instantiate(instanceProperty, instanceProperty.transform.parent);
                propertyObject.SetActive(true);

                ObjectProperty objectProperty = propertyObject.GetComponent<ObjectProperty>();
                objectProperty.Init(modelInput, this);
                _propertiesObjects.Add(objectProperty);
            }
        }

        void CheckObjectRemove(int id)
        {
            if (_selectedObject == null) return;
            
            if (id == _selectedObject.id)
                WipeSelections();
        }

        void OnUpdateObjects(int id)
        {
            foreach (var propertyObject in _propertiesObjects)
            {
                propertyObject.UpdateUI();
            }
        }
        
        void Start()
        {
            _objectPropertyFloatInstance.SetActive(false);
            _objectPropertyIntInstance.SetActive(false);
            _objectPropertyStringInstance.SetActive(false);
            _objectPropertyVector3Instance.SetActive(false);
            _objectPropertyPositionInstance.SetActive(false);
            _objectPropertyShapeInstance.SetActive(false);
            _objectPropertyProjectileInstance.SetActive(false);
            
            _objectProperty.SetActive(false);
            
            LevelDataManager.instance.onRemoveObject.AddListener(CheckObjectRemove);
            LevelDataManager.instance.onUpdateObject.AddListener(OnUpdateObjects);
        }
        
        
        public BeatObjectData selectedObject => _selectedObject;
    }
}