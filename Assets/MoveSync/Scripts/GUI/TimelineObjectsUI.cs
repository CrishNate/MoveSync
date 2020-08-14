using System;
using System.Collections.Generic;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoveSync
{
    public struct SelectedObjectData
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

        private Dictionary<int, ObjectUI> _objectsUi = new Dictionary<int, ObjectUI>();
        
        // selection
        [SerializeField] private GameObject _selectionUiInstance;
        private SelectionUI _selectionUi;
        private Dictionary<int, SelectedObjectData> _selectedObjects = new Dictionary<int, SelectedObjectData>();
        private SelectedObjectData _currentSelection;
        
        // copy/paste
        private Dictionary<int, SelectedObjectData> _copyObjects = new Dictionary<int, SelectedObjectData>();

        private bool _drawOnlyBeatKeys;
        
        public void UpdateObjects()
        {
            foreach (var data in LevelDataManager.instance.levelInfo.beatObjectDatas)
            {
                UpdateObject(_objectsUi[data.id]);
            }
        }
        
        public void RecreateObjects()
        {
            ClearObjects();
            
            foreach (var data in LevelDataManager.instance.levelInfo.beatObjectDatas)
            {
                if (!_objectsUi.ContainsKey(data.id))
                    AddObject(data);
            }
        }
        
        public void AddObject(BeatObjectData data)
        {
            ObjectUI objectUi = Instantiate(_objectUiInstance, _rectObjectsList).GetComponent<ObjectUI>();
            objectUi.Init(data, _timeline);
            objectUi.onStartDrag.AddListener(OnStartDragObject);
            objectUi.onDrag.AddListener(OnDragObject);
            objectUi.onStopDrag.AddListener(OnStopDragObject);
            UpdateObject(objectUi);
            
            if (_drawOnlyBeatKeys)
                objectUi.ShowOnlyKey(true);
            
            _objectsUi.Add(objectUi.beatObjectData.id, objectUi);
        }

        public void RemoveObject(int id)
        {
            Destroy(_selectedObjects[id].objectUi.gameObject);
            _selectedObjects.Remove(id);
            _objectsUi.Remove(id);
        }

        public void UpdateObject(int id)
        {
            if (_objectsUi.TryGetValue(id, out var objectUi))
                objectUi.UpdateUI();
        }
        
        public void UpdateObject(ObjectUI objectUi)
        {
            objectUi.UpdateUI();
        }
        
        public void ClearObjects()
        {
            foreach (var objectUi in _objectsUi)
            {
                Destroy(objectUi.Value.gameObject);
            }
            _objectsUi.Clear();
        }

        /*
         * Object spawning
         */
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                float time = mouseTime;
                if (InputData.shouldSnap) time = Mathf.Round(time);
                
                // spawning object from bind data
                bool usedBind = false;
                foreach (var bind in BindingManager.instance.bind)
                {
                    if (Input.GetKey(bind.Value.key))
                    {
                        LevelDataManager.instance.CopyBeatObject(bind.Value.beatObjectData, time, mouseLayer);
                        usedBind = true;
                        break;
                    }
                }
                
                if (!usedBind && ObjectManager.instance.currentObjectModel != null)
                    LevelDataManager.instance.NewBeatObject(ObjectManager.instance.currentObjectModel, time, mouseLayer);
            }

            // wipe click
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ClearSelection();
                ObjectProperties.instance.WipeSelections();
            }
        }
        
        /*
         * Selecting logic
         */
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            // select in rectangle
            ClearSelection();

            _selectionUi = Instantiate(_selectionUiInstance, _rectObjectsList).GetComponent<SelectionUI>();
            _selectionUi.Init(eventData);

            ObjectProperties.instance.WipeSelections();
        }

        public void OnDrag(PointerEventData data)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            // select in rectangle
            foreach (var objectUi in _objectsUi)
            {
                if (_selectionUi.IsOverlapRect(objectUi.Value.rectTransform))
                    AddSelection(objectUi.Value);
            }
            
            Destroy(_selectionUi.gameObject);
        }

        bool AddSelection(ObjectUI objectUi)
        {
            if (_selectedObjects.ContainsKey(objectUi.beatObjectData.id)) return false;
            
            SelectedObjectData newSelectedObject = new SelectedObjectData
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
            _currentSelection.objectUi = null;
        }

        // calculate offset for each element starting from time/layer stamps
        void CalculateOffsets(ref Dictionary<int, SelectedObjectData> selectedObjects, float timeStamp, int layerStamp)
        {
            List<int> keys = new List<int>(selectedObjects.Keys);
            foreach(var key in keys)
            {
                SelectedObjectData newSelectedObject = selectedObjects[key];
                newSelectedObject.offset = newSelectedObject.beatObjectData.time - timeStamp;
                newSelectedObject.offsetLayer = newSelectedObject.beatObjectData.editorLayer - layerStamp;
                selectedObjects[key] = newSelectedObject;
            }
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
                List<int> tempKeys = new List<int>(_selectedObjects.Keys);
                foreach(var key in tempKeys)
                {
                    SelectedObjectData selectedObject = _selectedObjects[key];
                    if (selectedObject.beatObjectData.id != objectUi.beatObjectData.id)
                    {
                        RemoveSelection(selectedObject.objectUi);
                    }
                }
            }

            CalculateOffsets(ref _selectedObjects, objectUi.beatObjectData.time, objectUi.beatObjectData.editorLayer);
        }

        void OnDragObject(ObjectUI objectUi)
        {
            List<int> keys = new List<int>(_selectedObjects.Keys);
            foreach(var key in keys)
            {
                SelectedObjectData newSelectedObject = _selectedObjects[key];
                BeatObjectData data = newSelectedObject.beatObjectData;
                
                // offsetting beat objects while moving
                if (!InputData.shouldOnlyLayer) data.time = Mathf.Max(0, mouseTime + newSelectedObject.offset);
                data.editorLayer = Mathf.Max(0, mouseLayer + newSelectedObject.offsetLayer);
                if (InputData.shouldSnap) data.time = Mathf.Round(data.time);
                
                UpdateObject(newSelectedObject.objectUi);
            }
        }

        void OnStopDragObject(ObjectUI objectUi)
        {
            LevelDataManager.instance.SortBeatObjects();
        }

        void OnSelect(BeatObjectData beatObjectData)
        {
            if (_objectsUi.TryGetValue(beatObjectData.id, out var objectUi))
                objectUi.OnSelectProperties();
        }
        
        void OnDeselect(BeatObjectData beatObjectData)
        {
            if (_objectsUi.TryGetValue(beatObjectData.id, out var objectUi))
                objectUi.OnDeselectProperties();
        }

        void Start()
        {
            LevelDataManager.instance.onNewObject.AddListener(AddObject);
            LevelDataManager.instance.onRemoveObject.AddListener(RemoveObject);
            LevelDataManager.instance.onUpdateObject.AddListener(UpdateObject);
            LevelDataManager.instance.onLoadedSong.AddListener(RecreateObjects);
            LevelDataManager.instance.onUpdateObjects.AddListener(RecreateObjects);
            
            ObjectProperties.instance.onSelected.AddListener(OnSelect);
            ObjectProperties.instance.onDeselected.AddListener(OnDeselect);
            
            _timeline.onZoomUpdated.AddListener(UpdateObjects);
        }

        public void DrawOnlyBeatKeys(bool draw)
        {
            _drawOnlyBeatKeys = draw;
            foreach (var objectUi in _objectsUi)
            {
                objectUi.Value.ShowOnlyKey(draw);
            }
        }

        void Update()
        {
            // destroying all selections
            if (InputData.shouldDelete && _selectedObjects.Count > 0)
            {
                List<int> keys = new List<int>(_selectedObjects.Keys);
                foreach(var key in keys)
                {
                    LevelDataManager.instance.RemoveBeatObject(_selectedObjects[key].beatObjectData);
                }
            }
            
            // copy objects
            if (InputData.shouldCopy)
            {
                _copyObjects.Clear();
                float lessTime = -1;
                int lessLayer = -1;
                
                foreach (var selectedObject in _selectedObjects)
                {
                    _copyObjects.Add(selectedObject.Key, selectedObject.Value);

                    if (selectedObject.Value.beatObjectData.time < lessTime || lessTime < 0)
                        lessTime = selectedObject.Value.beatObjectData.time;
                    if (selectedObject.Value.beatObjectData.editorLayer < lessLayer || lessLayer < 0)
                        lessLayer = selectedObject.Value.beatObjectData.editorLayer;
                }

                CalculateOffsets(ref _copyObjects, lessTime, lessLayer);
            }
                        
            // paste objects
            if (InputData.shouldPaste)
            {
                foreach (var copyObject in _copyObjects)
                {
                    LevelDataManager.instance.CopyBeatObject(copyObject.Value.beatObjectData,
                        mouseTime + copyObject.Value.offset, mouseLayer + copyObject.Value.offsetLayer);
                }
                
                LevelDataManager.instance.SortBeatObjects();
            }
        }


        private Vector2 localMousePosition => Input.mousePosition - _rectObjectsList.position;
        private float mouseTime => Mathf.Max(0, localMousePosition.x * _timeline.invZoom);
        private int mouseLayer => Mathf.Max(0, Mathf.FloorToInt(-localMousePosition.y / layerHeight));
    }
}