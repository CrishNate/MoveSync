using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionUI : MonoBehaviour
{
    private Vector2 _startPoint;
    private Rect _rect;
    
    public bool IsOverlapRect(RectTransform other)
    {
        Rect otherRect = new Rect(other.localPosition.x, -other.localPosition.y, other.rect.width, other.rect.height);
        FixRect(ref otherRect);
        
        return _rect.Overlaps(otherRect);
    }

    public void Init(PointerEventData pointerEventData)
    {
        _rectTransform.localPosition = pointerEventData.pressPosition - (Vector2) _parentRectTransform.position;
    }

    void Update()
    {
        Vector2 size = Input.mousePosition - _rectTransform.position;
        size.y *= -1;
        
        _rectTransform.localScale = size;
        _rect = new Rect(_rectTransform.localPosition.x, -_rectTransform.localPosition.y, _rectTransform.localScale.x, _rectTransform.localScale.y);
        FixRect(ref _rect);
    }

    void FixRect(ref Rect rect)
    {
        if (rect.height < 0)
        {
            rect.y += rect.height;
            rect.height *= -1;
        }
        
        if (rect.width < 0)
        {
            rect.x += rect.width;
            rect.width *= -1;
        }
    }
    

    
    private RectTransform _rectTransform => (RectTransform) gameObject.transform;
    private RectTransform _parentRectTransform => (RectTransform) gameObject.transform.parent;
}
