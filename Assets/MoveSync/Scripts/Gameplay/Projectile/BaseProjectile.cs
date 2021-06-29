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

    public struct ProjectileParam
    {
        public GameObject instigator;
        public float invokeTimeStamp;
        public float duration;
        public float appearTime;
        public float scale;
        public float speed;
        public int count;
        public Mesh shape;
    }
    
    public virtual void Init(ProjectileParam initParam)
    {
        duration = initParam.duration;
        appearTime = initParam.appearTime;
        scale = initParam.scale;
        speed = initParam.speed;
        
        timeStamp = initParam.invokeTimeStamp;
    }

    public virtual float GetDisappearTime()
    {
        return 0.0f;
    }
}
