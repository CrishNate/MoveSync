using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LineRandom : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private int _softness = 100;
    [SerializeField] private float _radius;
    [SerializeField] private float _frequency = 0.2f;

    public void GenerateLine(float length, float radius)
    {
        float random = Random.value;
        float randomRad = random * 2.0f * Mathf.PI;
        
        var lineRenderers = GetComponentsInChildren<LineRenderer>();

        float softCoef = 1.0f / _softness;
        
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = _softness;

            for (int i = 0; i < _softness; i++)
            {
                float itter = i * softCoef;
                
                float r = (1 + Mathf.Cos(itter * length * _frequency * 1.5f + randomRad)) * radius * 0.5f;
                float x = Mathf.Cos(itter * length * _frequency + randomRad) * r;
                float y = Mathf.Sin(itter * length * _frequency + randomRad) * r;
                lineRenderer.SetPosition(i, new Vector3(x, itter * length, y));
            }
        }
    }
    void OnValidate()
    {
        GenerateLine(_length, _radius);
    }
}