using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected float duration;
    protected float appearTime;
    protected float scale;
    protected float timeStamp = -1;

    
    public virtual void Init(GameObject instigator, float invokeTimeStamp, float _duration, float _appearTime, float _scale)
    {
        duration = _duration;
        appearTime = _appearTime;
        scale = _scale;
        
        timeStamp = invokeTimeStamp;
    }

    public virtual float GetDisappearTime()
    {
        return 0.0f;
    }


    public float Duration => duration;
    public float AppearTime => appearTime;
}
