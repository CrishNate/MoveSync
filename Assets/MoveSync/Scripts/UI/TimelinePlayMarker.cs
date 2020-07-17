using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TimelinePlayMarker : MonoBehaviour, IPointerClickHandler
{
     public UnityEventFloatParam onClick;

     private RectTransform _rectTransform;

     private void Start()
     {
          _rectTransform = GetComponent<RectTransform>();
          _rectTransform.pivot = Vector2.zero;
     }

     public void OnPointerClick(PointerEventData eventData)
     {
          onClick.Invoke(eventData.position.x - _rectTransform.position.x);
     }
}
