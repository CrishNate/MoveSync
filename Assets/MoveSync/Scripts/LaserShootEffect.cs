using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShootEffect : BaseProjectile
{
    [SerializeField] private bool _useOnX = true;
    [SerializeField] private bool _useOnY = true;
    [SerializeField] private bool _useOnZ = true;
    [SerializeField] private float _disapearMultiplier = 2.0f;

    private BeatShoot _beatShoot;
    private Vector3 _savedScale;
    
    public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime, float scale)
    {
        base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);
        
        _savedScale = transform.localScale;
        
        Vector3 tempScale = _savedScale;
        if (_useOnX) tempScale.x = 0;
        if (_useOnY) tempScale.y = 0;
        if (_useOnZ) tempScale.z = 0;
        
        transform.localScale = tempScale;
        
        transform.parent = instigator.transform;
    }

    void Update()
    {
        if (_timeStamp <= 0) return;

        float endTimeStamp = _timeStamp + _appearTime + _duration;
        float deltaScale = 0;
        if (LevelSequencer.instance.timeBPM < endTimeStamp)
        {
            deltaScale = (LevelSequencer.instance.timeBPM - _timeStamp) / _appearTime;
        }
        else
        {
            deltaScale = 1 - (LevelSequencer.instance.timeBPM - endTimeStamp) / (_appearTime * _disapearMultiplier);

            if (deltaScale < 0)
            {
                Destroy(gameObject);
            }
        }

        deltaScale = Mathf.Clamp(deltaScale, 0.0f, 1.0f);
        
        Vector3 tempScale = _savedScale;
        if (_useOnX) tempScale.x = _scale * deltaScale;
        if (_useOnY) tempScale.y = _scale * deltaScale;
        if (_useOnZ) tempScale.z = _scale * deltaScale;

        transform.localScale = tempScale;
    }
}
