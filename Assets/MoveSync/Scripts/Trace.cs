using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{
    public float radius;
    
    [SerializeField] private bool _useOnX = true;
    [SerializeField] private bool _useOnY = true;
    [SerializeField] private bool _useOnZ = true;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _distanceFromHit;

    private float _distance;
    private Vector3 _scale;

    void Start()
    {
        _scale = transform.localScale;
        
        Vector3 tempScale = _scale;
        if (_useOnX) tempScale.x = 0;
        if (_useOnY) tempScale.y = 0;
        if (_useOnZ) tempScale.z = 0;
        
        transform.localScale = tempScale;
        _distance = _direction.magnitude;
    }

    void Update()
    {
        RaycastHit hitResult;
        bool hit = Physics.SphereCast(transform.position, radius, transform.rotation * _direction, out hitResult);
        float distance = (hit ? hitResult.distance : _distance) - _distanceFromHit;    
        
        Vector3 tempScale = _scale;
        if (_useOnX) tempScale.x = distance;
        if (_useOnY) tempScale.y = distance;
        if (_useOnZ) tempScale.z = distance;

        transform.localScale = tempScale;
    }
}
