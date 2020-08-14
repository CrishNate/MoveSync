using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MoveSync
{
     public class TimelinePlayMarker : MonoBehaviour, IPointerClickHandler
     {
          public UnityEventFloatParam onClick;

          [SerializeField] private RectTransform _playMarkerRect;
          [SerializeField] private RectTransform _playMarkerOnScrollRect;
          [SerializeField] private Text _timeText;
          private RectTransform _viewport;

          public void OnPointerClick(PointerEventData eventData)
          {
               onClick.Invoke(eventData.position.x - _viewport.position.x);
          }

          public void UpdateUI(float markerX, float smallMarkerX)
          {
               Vector2 position = _playMarkerRect.localPosition;
               position.x = markerX;
               _playMarkerRect.localPosition = position;
               
               position = _playMarkerOnScrollRect.localPosition;
               position.x = smallMarkerX;
               _playMarkerOnScrollRect.localPosition = position;

               _timeText.text = Mathf.FloorToInt(LevelSequencer.instance.timeBPM).ToString();
          }

          private void Start()
          {
               _viewport = GetComponent<RectTransform>();
               _viewport.pivot = Vector2.zero;
          }
     }
}