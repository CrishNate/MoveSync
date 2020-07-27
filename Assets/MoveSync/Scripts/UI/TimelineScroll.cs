using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoveSync
{
    [RequireComponent(typeof(RectTransform))]
    public class TimelineScroll : MonoBehaviour, IScrollHandler
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private EditorGrid _editorGrid;
        [Header("Marker")] [SerializeField] private RectTransform _playMarkerRect;
        [SerializeField] private RectTransform _playMarkerOnScrollRect;
        [SerializeField] private TimelinePlayMarker _playMarker;

        public UnityEvent onZoomUpdated;

        private float _length;
        private float _zoom = 128.0f;
        private float _invZoom;
        private float _offset;
        private float _time;
        private bool _lockOnMarker;


        public void OnScroll(PointerEventData eventData)
        {
            float scrollDelta = eventData.scrollDelta.y;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                UpdateZoom(_zoom * Mathf.Pow(1.1f, scrollDelta));
            }
            else
            {
                UpdateTimeline(_offset + scrollDelta);
            }
        }

        public void UpdateZoom(float zoom)
        {
            _zoom = Mathf.Max(zoom, _viewportWidth / _length);
            _invZoom = 1 / zoom;
            _editorGrid.SetGridParams(_zoom);

            UpdateScroll();
            UpdatePlayMarker();
            UpdateTimeline(_scrollbar.value * _lengthSubScreen);

            onZoomUpdated.Invoke();
        }

        public void UpdateLength(float length)
        {
            _length = length;
            UpdateScroll();
        }

        public void UpdateTimeline(float offset)
        {
            _offset = Mathf.Clamp(offset, 0.0f, _lengthSubScreen);
            UpdateTimeline();
        }

        public void UpdateTimeline()
        {
            Vector2 position = _content.localPosition;
            position.x = -_offset * _zoom;
            _content.localPosition = position;

            _editorGrid.UpdateOffsetGrid();
            _scrollbar.SetValueWithoutNotify(Mathf.Clamp(_offset / _lengthSubScreen, 0.0f, 1.0f));
        }

        void UpdateScroll()
        {
            _scrollbar.size = _viewportWidth / _zoomLength;
        }

        void UpdatePlayMarker()
        {
            _time = LevelSequencer.instance.timeBPM;

            Vector2 position = _playMarkerRect.localPosition;
            position.x = _time * _zoom;
            _playMarkerRect.localPosition = position;

            // updating small marker
            position = _playMarkerOnScrollRect.localPosition;
            position.x = _time / _length * _viewportWidth;
            _playMarkerOnScrollRect.localPosition = position;
        }

        void OnScrollChanged(float value)
        {
            UpdateTimeline(value * (_length - _viewportWidth / _zoom));
        }

        void OnMarkerChanged(float value)
        {
            LevelSequencer.instance.SetSongTime((value / _zoom + _offset) *
                                                LevelSequencer.instance.toTime);

            // for some reason audioSource.time return 0 if sound is not in pause mode
            if (!LevelSequencer.instance.audioSource.isPlaying)
            {
                LevelSequencer.instance.audioSource.Play();
                LevelSequencer.instance.audioSource.Pause();
            }

            UpdatePlayMarker();
        }

        void Update()
        {
            if (LevelSequencer.instance.audioSource.isPlaying)
            {
                UpdatePlayMarker();
            }

            if (LockOnMarker && LevelSequencer.instance.audioSource.isPlaying)
            {
                UpdateTimeline(LevelSequencer.instance.timeBPM - _viewportWidth * invZoom * 0.5f);
            }
        }

        void Start()
        {
            _scrollbar.onValueChanged.AddListener(OnScrollChanged);
            _playMarker.onClick.AddListener(OnMarkerChanged);
            _editorGrid.SetGridParams(_zoom);
            _invZoom = 1 / zoom;

            // temp debug
            UpdateLength(Mathf.Floor(LevelSequencer.instance.audioSource.clip.length * LevelSequencer.instance.toBPM));
        }

        public bool LockOnMarker
        {
            get => _lockOnMarker;
            set
            {
                _lockOnMarker = value;
                if (_lockOnMarker) UpdateTimeline(LevelSequencer.instance.timeBPM - _viewportWidth * zoom);
            }
        }

        public float zoom => _zoom;
        public float invZoom => _invZoom;
        private float _zoomLength => _length * _zoom;
        private float _viewportWidth => _viewport.rect.width;
        private float _lengthSubScreen => _length - _viewportWidth * _invZoom;
    }
}