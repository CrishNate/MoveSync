using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropBox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _content;
    
    private Vector2 _dragOffset;
    private Vector2 _lastContentPosition;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragOffset = eventData.position;
        _lastContentPosition = _content.localPosition;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 offset = data.position - _dragOffset;
        Vector2 position = _lastContentPosition + offset;

        _content.localPosition = position;
    }

    public void OnEndDrag(PointerEventData eventData)
    { }
}
