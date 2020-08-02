using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class LineBetweenGizmo : BaseGizmoConnection
    {
        private LineRenderer _lineRenderer;

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        protected override void UpdateGizmo()
        {
            if (!_lineRenderer) _lineRenderer = GetComponent<LineRenderer>();
            
            _lineRenderer.SetPosition(0, _gizmos1.transform.position);
            _lineRenderer.SetPosition(1, _gizmos2.transform.position);
        }
    }
}