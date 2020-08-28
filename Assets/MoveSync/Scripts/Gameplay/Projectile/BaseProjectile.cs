using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected float duration;
    protected float appearTime;
    protected float scale;
    protected float speed;
    protected float timeStamp = -1;

    
    public virtual void Init(GameObject instigator, float invokeTimeStamp, float _duration, float _appearTime, float _scale, float _speed)
    {
        duration = _duration;
        appearTime = _appearTime;
        scale = _scale;
        speed = _speed;
        
        timeStamp = invokeTimeStamp;
    }

    public virtual float GetDisappearTime()
    {
        return 0.0f;
    }
}
