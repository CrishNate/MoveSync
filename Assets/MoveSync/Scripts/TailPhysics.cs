using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPhysics : MonoBehaviour
{
    [SerializeField] private float _angularSpeed;
    private List<Transform> _tailParts;
    private List<Quaternion> _rotations;
    
    void Start()
    {
        _tailParts = new List<Transform>();
        _rotations = new List<Quaternion>();
        
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            _tailParts.Add(child);
            _rotations.Add(child.rotation);
        }
    }

    void Update()
    {
        for (int i = 0; i < _tailParts.Count; i++)
        {
            Transform child = _tailParts[i];
            Quaternion rotation = _rotations[i];
            
            float maxAngle = Quaternion.Angle(rotation, child.parent.rotation);
            child.transform.rotation = _rotations[i] =
                Quaternion.RotateTowards(rotation, child.parent.rotation, maxAngle * _angularSpeed * Time.deltaTime);
        }
    }
}
