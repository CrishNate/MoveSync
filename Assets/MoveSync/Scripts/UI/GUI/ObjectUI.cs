using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoveSync
{
    public class ObjectUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler 
    {
        [HideInInspector] public BeatObject _beatObject;
        [HideInInspector] public UnityEventObjectUI onStartDrag = new UnityEventObjectUI();
        [HideInInspector] public UnityEventObjectUI onDrag = new UnityEventObjectUI();
        [HideInInspector] public UnityEventObjectUI onStopDrag = new UnityEventObjectUI();
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

            if (beatObjectData.hasModel(APPEAR.TYPE))
                _appearUI.onValueChanged.AddListener(OnSetAppear);
            else
                _appearUI.IsShown(false);

            if (beatObjectData.hasModel(DURATION.TYPE))
                _durationUI.onValueChanged.AddListener(OnSetDuration);
            else
                _durationUI.IsShown(false);
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

        public void OnEndDrag(PointerEventData eventData)
        {
            onStopDrag.Invoke(this);
        }

        public void UpdateUI(float zoom)
        {
            rectTransform.localPosition = new Vector2(beatObjectData.time * zoom, TimelineObjectsUI.layerHeight * beatObjectData.editorLayer * -1.0f);
            
            if (beatObjectData.hasModel(APPEAR.TYPE)) _appearUI.SetValue(beatObjectData.getModel<APPEAR>(APPEAR.TYPE).value * zoom);
            if (beatObjectData.hasModel(DURATION.TYPE)) _durationUI.SetValue(beatObjectData.getModel<DURATION>(DURATION.TYPE).value * zoom);
        }

        void OnSetAppear(float value)
        {
            beatObjectData.getModel<APPEAR>(APPEAR.TYPE).value = value * _timeline.invZoom;
        }
        
        void OnSetDuration(float value)
        {
            beatObjectData.getModel<DURATION>(DURATION.TYPE).value = value * _timeline.invZoom;
        }
        
        public void OnSelect()
        {
            _selection.SetActive(true);
        }
        public void OnDeselect()
        {
            _selection.SetActive(false);
        }

        public void ShowOnlyKey(bool show)
        {
            if (beatObjectData.hasModel(APPEAR.TYPE)) _appearUI.IsShown(!show);
            if (beatObjectData.hasModel(DURATION.TYPE)) _durationUI.IsShown(!show);
        }
        

        public RectTransform rectTransform => (RectTransform)transform;
    }

}