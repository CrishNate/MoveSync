using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoveSync
{
    public struct SelectedObject
    {
        public ObjectUI objectUi;
        public float offset;
        public int offsetLayer;
        
        public BeatObjectData beatObjectData => objectUi.beatObjectData;
    }
    
    public class TimelineObjectsUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler 
    {
        public static float layerHeight = 30.0f;

        [SerializeField] private RectTransform _rectObjectsList;
        [SerializeField] private TimelineScroll _timeline;
        [SerializeField] private GameObject _objectUiInstance;

        private Dictionary<Guid, ObjectUI> objectsUi = new Dictionary<Guid, ObjectUI>();
        
        // selection
        [SerializeField] private GameObject _selectionUiInstance;
        private SelectionUI _selectionUi;
        private Dictionary<Guid, SelectedObject> _selectedObjects = new Dictionary<Guid, SelectedObject>();
        private SelectedObject _currentSelection;
        
        public void UpdateObjects()
        {
            foreach (var data in LevelDataManager.instance.levelInfo.beatObjectDatas)
            {
                if (!objectsUi.ContainsKey(data.id))
                {
                    AddObject(data);
                }
                else
                {
                    UpdateObject(objectsUi[data.id]);
                }
            }
        }

        public void AddObject(BeatObjectData data)
        {
            ObjectUI objectUi = Instantiate(_objectUiInstance, _rectObjectsList).GetComponent<ObjectUI>();
            objectUi.Init(data);
            objectUi.onStartDrag.AddListener(OnStartDragObject);
            objectUi.onDrag.AddListener(OnDragObject);
            UpdateObject(objectUi);
            
            objectsUi.Add(objectUi.beatObjectData.id, objectUi);
        }

        public void UpdateObject(ObjectUI objectUi)
        {
            objectUi.UpdateUI(_timeline.zoom);
        }
        
        public void ClearObjects()
        {
            foreach (var objectUi in objectsUi)
            {
                Destroy(objectUi.Value.gameObject);
            }
            objectsUi.Clear();
        }

        /*
         * Object spawning
         */
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                float time = mouseTime;
                if (Input.GetKey(KeyCode.LeftShift)) time = Mathf.Round(time);
                
                LevelDataManager.instance.NewBeatObject(ObjectManager.instance.currentObjectModel.objectTag, time, mouseLayer);
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ClearSelection();
            }
        }
        
        /*
         * Selecting logic
         */
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _selectionUi = Instantiate(_selectionUiInstance, _rectObjectsList).GetComponent<SelectionUI>();
            _selectionUi.Init(eventData);
        }

        public void OnDrag(PointerEventData data)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            // select in rectangle
            foreach (var objectUi in objectsUi)
            {
                if (_selectionUi.IsOverlapRect(objectUi.Value.rectTransform))
                    AddSelection(objectUi.Value);
            }
            
            Destroy(_selectionUi.gameObject);
        }

        bool AddSelection(ObjectUI objectUi)
        {
            if (_selectedObjects.ContainsKey(objectUi.beatObjectData.id)) return false;
            
            SelectedObject newSelectedObject = new SelectedObject
            {
                objectUi = objectUi
            };

            objectUi.OnSelect();
            _selectedObjects.Add(objectUi.beatObjectData.id, newSelectedObject);

            return true;
        }

        void RemoveSelection(ObjectUI objectUi)
        {
            objectUi.OnDeselect();
            _selectedObjects.Remove(objectUi.beatObjectData.id);
        }

        void ClearSelection()
        {
            foreach (var selectedObject in _selectedObjects)
            {
                selectedObject.Value.objectUi.OnDeselect();
            }
            _selectedObjects.Clear();
        }
        
        /*
         * Selection move
         */
        void OnStartDragObject(ObjectUI objectUi)
        {
            // check for individual selection
            if (AddSelection(objectUi))
            {
                // remove old selection
                if (_currentSelection.objectUi != null &&
                    _currentSelection.objectUi != _selectedObjects[objectUi.beatObjectData.id].objectUi)
                {
                    RemoveSelection(_currentSelection.objectUi);
                }

                _currentSelection = _selectedObjects[objectUi.beatObjectData.id];
                
                // remove all other selections
                List<Guid> tempKeys = new List<Guid>(_selectedObjects.Keys);
                foreach(var key in tempKeys)
                {
                    SelectedObject selectedObject = _selectedObjects[key];
                    if (selectedObject.beatObjectData.id != objectUi.beatObjectData.id)
                    {
                        RemoveSelection(selectedObject.objectUi);
                    }
                }
            }
            
            // calculate offset for each element starting from dragging object
            List<Guid> keys = new List<Guid>(_selectedObjects.Keys);
            foreach(var key in keys)
            {
                SelectedObject newSelectedObject = _selectedObjects[key];
                newSelectedObject.offset = newSelectedObject.beatObjectData.time - objectUi.beatObjectData.time;
                newSelectedObject.offsetLayer = newSelectedObject.beatObjectData.editorLayer - objectUi.beatObjectData.editorLayer;
                _selectedObjects[key] = newSelectedObject;
            }
        }

        void OnDragObject(ObjectUI objectUi)
        {
            
            List<Guid> keys = new List<Guid>(_selectedObjects.Keys);
            foreach(var key in keys)
            {
                SelectedObject newSelectedObject = _selectedObjects[key];
                BeatObjectData data = newSelectedObject.beatObjectData;
                
                // offsetting beat objects while moving
                if (!Input.GetKey(KeyCode.LeftControl)) data.time = Mathf.Max(0, mouseTime + newSelectedObject.offset);
                data.editorLayer = Mathf.Max(0, mouseLayer + newSelectedObject.offsetLayer);
                // snapping
                if (Input.GetKey(KeyCode.LeftShift)) data.time = Mathf.Round(data.time);
                
                UpdateObject(newSelectedObject.objectUi);
            }
        }

        void Start()
        {
            LevelDataManager.instance.onNewObject.AddListener(AddObject);
            _timeline.onZoomUpdated.AddListener(UpdateObjects);
        }

        
        public Vector2 localMousePosition => Input.mousePosition - _rectObjectsList.position;
        public float mouseTime => localMousePosition.x * _timeline.invZoom;
        public int mouseLayer => Mathf.FloorToInt(-localMousePosition.y / layerHeight);
    }
}