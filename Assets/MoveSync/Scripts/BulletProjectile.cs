using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : BaseProjectile
{
    private float _distance;
    private float _speed;
    
    public override void Init(GameObject instigator, float invokeTimeStamp, float duration, float appearTime,
        float scale)
    {
        base.Init(instigator, invokeTimeStamp, duration, appearTime, scale);

        _distance = (Camera.main.transform.position - instigator.transform.position).magnitude;

        transform.localScale = Vector3.one * scale;
        _speed = _distance / (_appearTime * LevelSequencer.instance.toTime);
    }

    void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;

        // just remove it
        if (LevelSequencer.instance.timeBPM > _timeStamp + 10)
        {
            Destroy(gameObject);
        }
    }
}
