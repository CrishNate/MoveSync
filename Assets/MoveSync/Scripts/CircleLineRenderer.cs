using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CircleLineRenderer : MonoBehaviour
{
    [SerializeField] private int _divisions;
    [SerializeField] private float _radius;

    void OnValidate()
    {
        var lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = _divisions;

        for (int i = 0; i < _divisions; i++)
        {
            float x = Mathf.Cos((float) i / _divisions * Mathf.PI * 2.0f);
            float y = Mathf.Sin((float) i / _divisions * Mathf.PI * 2.0f);
            lineRenderer.SetPosition(i, new Vector3(x, 0, y) * _radius);
        }
    }
}