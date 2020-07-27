using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionUI : MonoBehaviour
{
    private Vector2 _startPoint;
    
    public void Init(PointerEventData pointerEventData)
    {
        _rectTransform.localPosition = pointerEventData.pressPosition - (Vector2) _parentRectTransform.position;
    }

    void Update()
    {
        Vector2 size = Input.mousePosition - _rectTransform.position;
        size.y *= -1;
        
        _rectTransform.localScale = size;
    }
    
    private RectTransform _rectTransform => (RectTransform) gameObject.transform;
    private RectTransform _parentRectTransform => (RectTransform) gameObject.transform.parent;
}
