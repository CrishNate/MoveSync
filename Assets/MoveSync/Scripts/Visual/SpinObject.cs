using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private Vector3 _dRotation;
    [SerializeField] private bool _random;
    [SerializeField] private float _randomAmplitude = 1;


    void Start()
    {
        if (_random)
            _dRotation = Random.insideUnitSphere.normalized * _randomAmplitude;
    }

    void Update()
    {
        transform.Rotate(_dRotation * Time.deltaTime);
    }
}
