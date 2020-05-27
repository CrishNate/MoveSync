using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected float _duration;
    protected float _appearTime;
    protected float _scale;
    
    protected float _timeStamp = -1;

    public virtual void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime, float scale)
    {
        _duration = duration;
        _appearTime = appearTime;
        _scale = scale;
        
        _timeStamp = invokeTimeStamp;
    }
}
