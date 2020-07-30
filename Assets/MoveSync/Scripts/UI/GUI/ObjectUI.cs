using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoveSync
{
    
    public class EventObjectUI : UnityEvent<ObjectUI>{};
    
    public class ObjectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler 
    {
        [HideInInspector] public BeatObject _beatObject;
        [HideInInspector] public EventObjectUI onStartDrag = new EventObjectUI();
        [HideInInspector] public EventObjectUI onDrag = new EventObjectUI();
        [HideInInspector] public BeatObjectData beatObjectData;

        [SerializeField] private RectTransform _middleHandler;
        [SerializeField] private GameObject _selection;
        [SerializeField] private ObjectDurationUI _appearUI;
        [SerializeField] private ObjectDurationUI _durationUI;

        private TimelineScroll _timeline;
        
        
        public void Init(BeatObjectData data, TimelineScroll timeline)
        {
            beatObjectData = data;
            _timeline = timeline;

            ObjectModel model = ObjectManager.instance.objectModels[data.objectTag];

            if ((model.inputUi & ModelInputUI.APPEAR) == 0) _appearUI.gameObject.SetActive(false);
            if ((model.inputUi & ModelInputUI.STAY) == 0) _durationUI.gameObject.SetActive(false);
            
            _appearUI.onValueChanged.AddListener(OnSetAppear);
            _durationUI.onValueChanged.AddListener(OnSetDuration);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onStartDrag.Invoke(this);
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

        public void UpdateUI(float zoom)
        {
            rectTransform.localPosition = new Vector2(beatObjectData.time * zoom, TimelineObjectsUI.layerHeight * beatObjectData.editorLayer * -1.0f);
            
            _appearUI.SetValue(beatObjectData.appearDuration * zoom);
            _durationUI.SetValue(beatObjectData.duration * zoom);
        }

        void OnSetAppear(float value)
        {
            beatObjectData.appearDuration = value * _timeline.invZoom;
        }
        
        void OnSetDuration(float value)
        {
            beatObjectData.duration = value * _timeline.invZoom;
        }
        
        public void OnSelect()
        {
            _selection.SetActive(true);
        }
        public void OnDeselect()
        {
            _selection.SetActive(false);
        }
        

        public RectTransform rectTransform => (RectTransform)transform;
    }

}