using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TimelineScroll : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private EditorGrid _editorGrid;
    [Header("Marker")]
    [SerializeField] private RectTransform _playMarkerRect;
    [SerializeField] private RectTransform _playMarkerOnScrollRect;
    [SerializeField] private TimelinePlayMarker _playMarker;

    private float _length;
    private float _zoom = 128.0f;
    private float _offset;
    private float _time;
    private float _zoomLength => _length * _zoom;

    public void UpdateZoom(float zoom)
    {
        _zoom = zoom;
        _editorGrid.SetGridParams(_zoom);

        UpdateTimeline();
        UpdateScroll();
        UpdatePlayMarker();
    }
    public void UpdateLength(float length)
    {
        _length = length;
        UpdateScroll();
    }
    public void UpdateTimeline(float offset)
    {
        _offset = offset;
        UpdateTimeline();
    }
    public void UpdateTimeline()
    {
        Vector2 position = _content.localPosition;
        position.x = -_offset * _zoom;
        _content.localPosition = position;
        
        _editorGrid.UpdateOffsetGrid();
    }

    public void IncreeaseZoom() { UpdateZoom(_zoom * 2); }
    public void DecreaseZoom() { UpdateZoom(_zoom / 2); }
    
    void UpdateScroll()
    {
        _scrollbar.size = _viewport.rect.width / _zoomLength;
    }
    void UpdatePlayMarker()
    {
        _time = LevelSequencer.instance.timeBPM;
        
        Vector2 position = _playMarkerRect.localPosition;
        position.x = _time * _zoom;
        _playMarkerRect.localPosition = position;

        // updating small marker
        position = _playMarkerOnScrollRect.localPosition;
        position.x = _time / _length * _viewport.rect.width;
        _playMarkerOnScrollRect.localPosition = position;
    }

    void OnScrollChanged(float value)
    {
        UpdateTimeline(Mathf.Max(0, value * (_length - _viewport.rect.width / _zoom)));
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
    }

    void Start()
    {
        _scrollbar.onValueChanged.AddListener(OnScrollChanged);
        _playMarker.onClick.AddListener(OnMarkerChanged);
        _editorGrid.SetGridParams(_zoom);
        
        // temp debug
        UpdateLength(Mathf.Floor(LevelSequencer.instance.audioSource.clip.length * LevelSequencer.instance.toBPM));
    }
}
