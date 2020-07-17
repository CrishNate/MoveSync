using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class BossPoint : MonoBehaviour
{
    private static float _appearTime = 2.0f;

    private float _timeBPM;
    private bool _active;
    private MeshRenderer _meshRenderer;
    
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }

    void OnActivate()
    {
        _meshRenderer.enabled = true;
    }

    void OnHit()
    {
        OnDeactivate();
    }

    void OnDeactivate()
    {
        _meshRenderer.enabled = false;
    }

    void Update()
    {
        float difference = LevelSequencer.instance.timeBPM - _timeBPM;

        if (difference > 0.0f && !_active)
        {
            _active = true;
            OnActivate();
        }
        else if (difference > _appearTime)
        {
            _timeBPM += Random.Range(4, 16);
            _active = false;
            OnDeactivate();
        }
    }
}
