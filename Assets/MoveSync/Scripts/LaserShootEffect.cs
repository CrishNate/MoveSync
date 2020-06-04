using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShootEffect : BaseProjectile
{
    [SerializeField] private bool _useOnX = true;
    [SerializeField] private bool _useOnY = true;
    [SerializeField] private bool _useOnZ = true;
    [SerializeField] private float _disapearMultiplier = 2.0f;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private LineRenderer _lineRenderer;

    private BeatShoot _beatShoot;
    private ParticleSystem _particleSystem;

    private Vector3 _savedScale;
    private Color _savedColor;
    private float _savedWidth;
    
    public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime, float scale)
    {
        base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);

        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _savedScale = transform.localScale;
        _savedColor = _meshRenderer.material.color;
        
        Vector3 tempScale = _savedScale;
        if (_useOnX) tempScale.x = 0;
        if (_useOnY) tempScale.y = 0;
        if (_useOnZ) tempScale.z = 0;
        
        transform.localScale = tempScale;
        
        transform.parent = instigator.transform;
        _particleSystem.gameObject.transform.parent = null;
        _particleSystem.gameObject.transform.localScale = Vector3.one;
        _savedWidth = _lineRenderer.startWidth;
    }

    void Update()
    {
        if (_timeStamp <= 0) return;

        float endTimeStamp = _timeStamp + _appearTime + _duration;
        float deltaScale = 0;
        float deltaScaleForward = 0;
        
        if (LevelSequencer.instance.timeBPM < endTimeStamp)
        {
            deltaScale = (LevelSequencer.instance.timeBPM - _timeStamp) / _appearTime;
            deltaScaleForward = deltaScale;
        }
        else
        {
            deltaScale = 1 - (LevelSequencer.instance.timeBPM - endTimeStamp) / (_appearTime * _disapearMultiplier);
            deltaScaleForward = 1.0f;
            
            if (deltaScale < 0)
            {
                Destroy(gameObject);
            }
        }

        deltaScale = Mathf.Clamp(deltaScale, 0.0f, 1.0f);

        float powDeltaScale = Mathf.Pow(deltaScale, 8);
        _meshRenderer.material.color = _savedColor + (Color.white - _savedColor) * powDeltaScale;
        _lineRenderer.startWidth = _lineRenderer.endWidth = _savedWidth * deltaScale;
        
        Vector3 tempScale = _savedScale;
        if (_useOnX) tempScale.x = _scale * deltaScale;
        if (_useOnY) tempScale.y = _scale * deltaScale;
        if (_useOnZ) tempScale.z = _scale * deltaScale;

        tempScale.z = deltaScaleForward;

        transform.localScale = tempScale;
    }
}
