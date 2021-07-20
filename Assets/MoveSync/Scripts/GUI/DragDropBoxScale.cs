using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragDropBoxScale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool _unrestricted = false;
    [SerializeField] private float maxDrag = 100.0f;
    [SerializeField] private float minDrag;
    [SerializeField] private RectTransform _content;
    [SerializeField] private bool _horizontal;
    [SerializeField] private bool _vertical;
    [SerializeField] private UnityEvent _onDragClosed;
    [SerializeField] private UnityEvent _onDragOpen;

    private Vector2 _dragOffset;
    private Vector2 _lastContentScale;
    private bool _closed;
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragOffset = eventData.position;
        _lastContentScale = _content.sizeDelta;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 offset = data.position - _dragOffset;
        if (!_horizontal) offset.x = 0;
        if (!_vertical) offset.y = 0;
        
        Vector2 scale = _lastContentScale + offset;
        if (!_unrestricted)
        {
            if (_horizontal) 
                scale.x = Mathf.Clamp(scale.x, minDrag, maxDrag);
            
            if (_vertical) 
                scale.y = Mathf.Clamp(scale.y, minDrag, maxDrag);
        }

        if (!_closed && Mathf.Abs(scale.magnitude - minDrag) < Mathf.Epsilon)
        {
            _onDragClosed.Invoke();
            _closed = true;
        }
        else if (_closed && Mathf.Abs(scale.magnitude - minDrag) > Mathf.Epsilon)
        {
            _onDragOpen.Invoke();
            _closed = false;
        }

        _content.sizeDelta = scale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
