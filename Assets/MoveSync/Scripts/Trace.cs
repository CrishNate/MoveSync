using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{
    public float radius;
    private static float _distanceFromHit = 1.0f;
    
    [SerializeField] private bool _useOnX = true;
    [SerializeField] private bool _useOnY = true;
    [SerializeField] private bool _useOnZ = true;
    [SerializeField] private Vector3 _direction;
    
    [SerializeField] private GameObject _previewMesh;

    private float _distance;
    private Vector3 _scale;
    private SpriteRenderer _sprite;

    void Start()
    {
        _scale = transform.localScale;
        
        Vector3 tempScale = _scale;
        if (_useOnX) tempScale.x = 0;
        if (_useOnY) tempScale.y = 0;
        if (_useOnZ) tempScale.z = 0;
        
        transform.localScale = tempScale;
        _distance = _direction.magnitude;

        radius = GetComponentInParent<BeatShoot>().projectileScale / 2.0f;
        _sprite = GetComponentInChildren<SpriteRenderer>();

        _sprite.gameObject.transform.localScale = Vector3.one * radius * 2;
        _previewMesh.transform.localScale = new Vector3(radius * 2, radius * 2, _previewMesh.transform.localScale.z);
    }

    void Update()
    {
        RaycastHit hitResult;
        bool hit = Physics.SphereCast(transform.position, radius, transform.rotation * _direction, out hitResult, _distance, 1 << LayerMask.NameToLayer("Player"));
        float distance = (hit ? hitResult.distance : _distance) - _distanceFromHit;
        
        Vector3 tempScale = _scale;
        if (_useOnX) tempScale.x = distance;
        if (_useOnY) tempScale.y = distance;
        if (_useOnZ) tempScale.z = distance;

        transform.localScale = tempScale;
    }
}
