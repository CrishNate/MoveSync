using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
}

public abstract class BeatObject : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTriggeredEvent;
    
    private float _time;
    private TransformData _transformData;

    private float _timeStamp;

    private bool _triggered;
    private float _dtime;

    public void Initialize(float time, TransformData transformData)
    {
        _time = time;
        _transformData = transformData;
    }

    protected virtual void Start()
    {
        _timeStamp = LevelSequencer.instance.timeBPM;
    }
    
    protected virtual void Update()
    {
        _dtime = (LevelSequencer.instance.timeBPM - _timeStamp) / (_time - _timeStamp);
        
        if (_dtime < 0)
        {
            Destroy(gameObject);
            return;
        }

        if (_dtime >= 1.0f)
        {
            if (!_triggered)
            {
                OnTriggered();
                _triggered = true;
            }
        }
        else
        {
            _triggered = false;
        }
    }

    protected void OnTriggered()
    {
        _onTriggeredEvent.Invoke();
    }
    
    protected float time => _time;
    protected float dtime => _dtime;
    protected float timeStamp => _timeStamp;
    protected TransformData transformData => _transformData;
}
