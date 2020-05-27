using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private float _multiplySpeed = 1.0f;
    [SerializeField] private bool _useOnP = false;
    [SerializeField] private bool _useOnY = false;
    [SerializeField] private bool _useOnR = false;

    void Update()
    {
        float p = _useOnP ? Time.time * _multiplySpeed : 0.0f;
        float y = _useOnY ? Time.time * _multiplySpeed : 0.0f;
        float r = _useOnR ? Time.time * _multiplySpeed : 0.0f;
        
        transform.localRotation = Quaternion.Euler(p, y, r);
    }
}
