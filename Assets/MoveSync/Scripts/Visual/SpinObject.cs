using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private Vector3 _dRotation;
    [SerializeField] private bool _random;
    [SerializeField] private float _randomAmplitude = 1;
    private float timeStamp;


    void Start()
    {
        timeStamp = Time.time;

        if (_random)
        {
            _dRotation = Random.insideUnitSphere.normalized * _randomAmplitude;
        }
    }

    void Update()
    {
        float t = Time.time - timeStamp;
        float p = Mathf.Abs(_dRotation.x) > 0f ? t * _dRotation.x : 0f;
        float y = Mathf.Abs(_dRotation.y) > 0f ? t * _dRotation.y : 0f;
        float r = Mathf.Abs(_dRotation.z) > 0f ? t * _dRotation.z : 0f;

        transform.localRotation = Quaternion.Euler(p, y, r);
    }
}
