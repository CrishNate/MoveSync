using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoveSync
{
    
    public class EventObjectUI : UnityEvent<ObjectUI>{};
    
    public class ObjectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector] public BeatObject _beatObject;
        [HideInInspector] public EventObjectUI onStartDrag = new EventObjectUI();
        [HideInInspector] public EventObjectUI onDrag = new EventObjectUI();
        [HideInInspector] public BeatObjectData beatObjectData;

        [SerializeField] private RectTransform _appearRectTransform;
        [SerializeField] private RectTransform _durationRectTransform;

        public void Init(BeatObjectData data)
        {
            beatObjectData = data;
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
        }

        public void UpdateUI(float zoom)
        {
            rectTransform.localPosition = new Vector2(beatObjectData.time * zoom, TimelineObjectsUI.layerHeight * beatObjectData.editorLayer * -1.0f);
            
            _appearRectTransform.sizeDelta = new Vector2(beatObjectData.appearDuration * zoom, _appearRectTransform.sizeDelta.y);
            _durationRectTransform.sizeDelta = new Vector2(beatObjectData.duration * zoom, _durationRectTransform.sizeDelta.y);
        }

        public RectTransform rectTransform => (RectTransform)gameObject.transform;
    }

}