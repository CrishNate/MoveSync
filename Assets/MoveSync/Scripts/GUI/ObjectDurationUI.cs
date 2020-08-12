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
    private float _lastPosition;
    private Coroutine _draggingUpdate;


    public void OnPointerDown (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _dragOffset = eventData.position.x - transform.position.x;
            _draggingUpdate = StartCoroutine(Dragging());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StopCoroutine(_draggingUpdate);
        }
    }

    public void SetValue(float value)
    {
        _content.sizeDelta = new Vector2(value, _content.sizeDelta.y);
    }

    public void IsShown(bool show)
    {
        _content.gameObject.SetActive(show);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    { }

    public void OnDrag(PointerEventData eventData)
    { }

    IEnumerator Dragging()
    {
        while (true)
        {
            float currentPosition = Input.mousePosition.x - _content.transform.position.x;

            if (Mathf.Abs(currentPosition - _lastPosition) > Mathf.Epsilon)
            {
                float offset = currentPosition - _dragOffset;
                if (isLeft) offset *= -1;
                offset = Mathf.Max(0, offset);

                _content.sizeDelta = new Vector2(offset, _content.sizeDelta.y);

                onValueChanged.Invoke(offset);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
