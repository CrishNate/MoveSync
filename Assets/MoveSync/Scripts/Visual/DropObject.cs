using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DropObject : MonoBehaviour
{
    [SerializeField]
    private float _delayDestroy;

    [SerializeField] 
    private float _forceValue;
    [SerializeField] 
    private float _torqueValue;
    
    private Rigidbody _rigidbody;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(_delayDestroy);
        Destroy(gameObject);
    }

    public void Drop()
    {
        StartCoroutine(DelayDestroy());
        
        _rigidbody.isKinematic = false;
        _rigidbody.AddTorque(Random.insideUnitSphere * _torqueValue, ForceMode.VelocityChange);
        _rigidbody.AddForce(Random.insideUnitSphere * _forceValue, ForceMode.VelocityChange);
    }
}
