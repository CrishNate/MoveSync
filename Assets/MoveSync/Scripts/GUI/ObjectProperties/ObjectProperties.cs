using System;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync.UI
{
    public class ObjectProperties : Singleton<ObjectProperties>
    {
        public UnityEventBeatObjectData onSelected = new UnityEventBeatObjectData();
        public UnityEventBeatObjectData onDeselected = new UnityEventBeatObjectData();

        [SerializeField] private GameObject _objectProperty;
        [SerializeField] private GameObject _objectPropertyFloatInstance;
        [SerializeField] private GameObject _objectPropertyStringInstance;
        [SerializeField] private GameObject _objectPropertyVector3Instance;
        [SerializeField] private GameObject _objectPropertyPositionInstance;
        
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

        void ConstructProperties()
        {
            foreach (var propertiesObject in _propertiesObjects)
            {
                Destroy(propertiesObject.gameObject);
            }
            _propertiesObjects.Clear();
            
            foreach (var modelInput in _selectedObject.modelInputsData)
            {
                GameObject instanceProperty = null;
                if (modelInput is FloatModelInput) instanceProperty = _objectPropertyFloatInstance;
                else if (modelInput is StringModelInput) instanceProperty = _objectPropertyStringInstance;
                else if (modelInput is POSITION) instanceProperty = _objectPropertyPositionInstance;
                else if (modelInput is Vector3ModelInput) instanceProperty = _objectPropertyVector3Instance;

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
            _objectPropertyStringInstance.SetActive(false);
            _objectPropertyVector3Instance.SetActive(false);
            _objectPropertyPositionInstance.SetActive(false);
            
            _objectProperty.SetActive(false);
            
            LevelDataManager.instance.onRemoveObject.AddListener(CheckObjectRemove);
            LevelDataManager.instance.onUpdateObject.AddListener(OnUpdateObjects);
        }
        
        
        public BeatObjectData selectedObject => _selectedObject;
    }
}