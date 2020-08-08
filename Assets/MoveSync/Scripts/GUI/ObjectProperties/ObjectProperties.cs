using System;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync.UI
{
    public class ObjectProperties : Singleton<ObjectProperties>
    {
        [SerializeField] private GameObject _objectProperty;
        [SerializeField] private GameObject _objectPropertyFloatInstance;
        [SerializeField] private GameObject _objectPropertyStringInstance;
        [SerializeField] private GameObject _objectPropertyVector3Instance;
        [SerializeField] private GameObject _objectPropertyPositionInstance;
        
        
        private ModelInput[] _currentModelInputs;
        private int _currentId;
        private bool _hasObjectRef;
        private List<GameObject> _propertiesObjects = new List<GameObject>();


        public void OpenProperties(int id, ModelInput[] modelInputs)
        {
            _objectProperty.SetActive(true);
            _currentModelInputs = modelInputs;
            _currentId = id;
            _hasObjectRef = true;
            
            CreateProperties();
        }
        
        public void OpenProperties(ModelInput[] modelInputs)
        {
            _objectProperty.SetActive(true);
            _currentModelInputs = modelInputs;
            _hasObjectRef = false;

            CreateProperties();
        }

        public void CloseProperties()
        {
            _objectProperty.SetActive(false);
        }

        void CreateProperties()
        {
            foreach (var propertiesObject in _propertiesObjects)
            {
                Destroy(propertiesObject);
            }
            
            foreach (var modelInput in _currentModelInputs)
            {
                GameObject instanceProperty = null;
                if (modelInput is FloatModelInput) instanceProperty = _objectPropertyFloatInstance;
                else if (modelInput is StringModelInput) instanceProperty = _objectPropertyStringInstance;
                else if (modelInput is POSITION) instanceProperty = _objectPropertyPositionInstance;
                else if (modelInput is Vector3ModelInput) instanceProperty = _objectPropertyVector3Instance;

                GameObject propertyObject = Instantiate(instanceProperty, instanceProperty.transform.parent);
                propertyObject.GetComponent<ObjectProperty>().Init(modelInput, this);
                propertyObject.SetActive(true);
                _propertiesObjects.Add(propertyObject);
            }
        }

        void Start()
        {
            _objectPropertyFloatInstance.SetActive(false);
            _objectPropertyStringInstance.SetActive(false);
            _objectPropertyVector3Instance.SetActive(false);
            _objectPropertyPositionInstance.SetActive(false);
            
            _objectProperty.SetActive(false);
        }
        
        
        public bool hasObjectRef => _hasObjectRef;
        public int currentId => _currentId;
    }
}