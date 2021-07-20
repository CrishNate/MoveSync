using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoveSync
{
    public class ObjectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler 
    {
        [HideInInspector] public UnityEventObjectUI onStartDrag = new UnityEventObjectUI();
        [HideInInspector] public UnityEventObjectUI onDrag = new UnityEventObjectUI();
        [HideInInspector] public UnityEventObjectUI onStopDrag = new UnityEventObjectUI();
        [HideInInspector] public BeatObjectData beatObjectData;

        [SerializeField] private Image _image;
        [SerializeField] private ObjectDurationUI _appearUI;
        [SerializeField] private ObjectDurationUI _durationUI;

        private TimelineScroll _timeline;
        
        
        public void Init(BeatObjectData data, TimelineScroll timeline)
        {
            beatObjectData = data;
            _timeline = timeline;

            if (beatObjectData.hasModel<APPEAR>())
                _appearUI.onValueChanged.AddListener(OnSetAppear);
            else
                _appearUI.IsShown(false);

            if (beatObjectData.hasModel<DURATION>())
                _durationUI.onValueChanged.AddListener(OnSetDuration);
            else
                _durationUI.IsShown(false);

            _image.color = MoveSyncData.instance.colorData.DefaultUIBeatObject;

            // TODO: Rework this shit
            if (LevelEditor.isEditor && LevelEditor.instance.objectIcons.TryGetValue(data.ObjectTag, out Sprite sprite))
            {
                _image.sprite = sprite;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onStartDrag.Invoke(this);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ObjectProperties.instance.Select(beatObjectData);
            }
        }

        // Drag drop logic
        public void OnBeginDrag(PointerEventData eventData)
        {
            onStartDrag.Invoke(this);
        }

        public void OnDrag(PointerEventData data)
        {
            onDrag.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onStopDrag.Invoke(this);
        }
        public void UpdateUI()
        {
            rectTransform.localPosition = new Vector2(beatObjectData.time * _timeline.zoom, TimelineObjectsUI.layerHeight * beatObjectData.editorLayer * -1.0f);
            
            if (beatObjectData.hasModel<APPEAR>()) 
                _appearUI.SetValue(beatObjectData.getModel<APPEAR>().value * _timeline.zoom);
            
            if (beatObjectData.hasModel<DURATION>()) 
                _durationUI.SetValue(beatObjectData.getModel<DURATION>().value * _timeline.zoom);
        }

        void OnSetAppear(float value)
        {
            beatObjectData.getModel<APPEAR>().value = value * _timeline.invZoom;
            LevelDataManager.instance.UpdateBeatObject(beatObjectData.id);
        }
        
        void OnSetDuration(float value)
        {
            beatObjectData.getModel<DURATION>().value = value * _timeline.invZoom;
            LevelDataManager.instance.UpdateBeatObject(beatObjectData.id);
        }
        
        public void OnSelect()
        {
            _image.color = MoveSyncData.instance.colorData.SelectedUIBeatObject;
        }
        public void OnDeselect()
        {
            _image.color = MoveSyncData.instance.colorData.DefaultUIBeatObject;
        }
        
        public void OnSelectProperties()
        {
            _image.color = MoveSyncData.instance.colorData.SelectedPropertiesUIBeatObject;
        }
        public void OnDeselectProperties(bool stillSelected)
        {
            _image.color = stillSelected
                ? MoveSyncData.instance.colorData.SelectedUIBeatObject
                : MoveSyncData.instance.colorData.DefaultUIBeatObject;
        }

        public void ShowOnlyKey(bool show)
        {
            if (beatObjectData.hasModel<APPEAR>()) 
                _appearUI.IsShown(!show);
            
            if (beatObjectData.hasModel<DURATION>()) 
                _durationUI.IsShown(!show);
        }
        

        public RectTransform rectTransform => (RectTransform)transform;
    }

}