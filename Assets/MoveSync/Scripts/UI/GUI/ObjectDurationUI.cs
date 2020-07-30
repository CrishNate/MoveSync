using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.EventSystems;


public class ObjectDurationUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
{
    public UnityEventFloatParam onValueChanged;

    [SerializeField] private bool isLeft;
    [SerializeField] private RectTransform _content;
    
    private float _dragOffset;
    private bool _isDragging;


    public void OnPointerDown (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _dragOffset = eventData.position.x - transform.position.x;
            _isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _isDragging = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    { }

    public void OnDrag(PointerEventData eventData)
    { }

    public void SetValue(float value)
    {
        _content.sizeDelta = new Vector2(value, _content.sizeDelta.y);
    }
    
    void Update()
    {
        if (!_isDragging) return;
        
        float offset = Input.mousePosition.x - _content.transform.position.x - _dragOffset;
        if (isLeft) offset *= -1;
        offset = Mathf.Max(0, offset);
        
        _content.sizeDelta = new Vector2(offset, _content.sizeDelta.y);
        
        onValueChanged.Invoke(offset);
    }
}
