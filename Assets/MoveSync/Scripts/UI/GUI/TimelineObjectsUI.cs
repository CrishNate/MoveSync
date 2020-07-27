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
        private GameObject _selectionUi;
        private Dictionary<Guid, SelectedObject> _selectedObjects = new Dictionary<Guid, SelectedObject>();
        
        
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
                LevelDataManager.instance.NewBeatObject(ObjectManager.instance.currentObjectModel.objectTag, mouseTime, mouseLayer);
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _selectedObjects.Clear();
            }
        }
        
        /*
         * Selecting logic
         */
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _selectionUi = Instantiate(_selectionUiInstance, _rectObjectsList);
            _selectionUi.GetComponent<SelectionUI>().Init(eventData);
        }

        public void OnDrag(PointerEventData data)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            Destroy(_selectionUi);
        }

        void AddSelection(ObjectUI objectUi)
        {
            if (!_selectedObjects.ContainsKey(objectUi.beatObjectData.id))
            {
                SelectedObject newSelectedObject = new SelectedObject
                {
                    objectUi = objectUi
                };
                _selectedObjects.Add(objectUi.beatObjectData.id, newSelectedObject);
            }
        }
        
        /*
         * Selecting move
         */
        void OnStartDragObject(ObjectUI objectUi)
        {
            AddSelection(objectUi);
            
            List<Guid> keys = new List<Guid>(_selectedObjects.Keys);
            foreach(var key in keys)
            {
                SelectedObject newSelectedObject = _selectedObjects[key];
                // calculate offset for each element staring from dragging object
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
                int index = LevelDataManager.instance.levelInfo.beatObjectDatas.FindIndex(x => x.id == newSelectedObject.beatObjectData.id);;
                BeatObjectData data = LevelDataManager.instance.levelInfo.beatObjectDatas[index];
                
                // offsetting beat objects while moving
                data.time = mouseTime + newSelectedObject.offset;
                data.editorLayer = mouseLayer + newSelectedObject.offsetLayer;
                LevelDataManager.instance.levelInfo.beatObjectDatas[index] = data;
                
                //newSelectedObject.objectUi.beatObjectData = data;
                
                UpdateObject(newSelectedObject.objectUi);
            }
            
            foreach (var selectedObjectPair in _selectedObjects)
            {
                // offsetting beat objects while moving
                BeatObjectData data = _selectedObjects[selectedObjectPair.Key].beatObjectData;
                SelectedObject newSelectedObject = selectedObjectPair.Value;
                data.time = mouseTime + newSelectedObject.offset;
                data.editorLayer = mouseLayer + newSelectedObject.offsetLayer;

                //newSelectedObject.objectUi.beatObjectData = data;
                
                UpdateObject(selectedObjectPair.Value.objectUi);
                LevelDataManager.instance.levelInfo.beatObjectDatas.Find(x => x.id == selectedObjectPair.Value.objectUi.beatObjectData.id);
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