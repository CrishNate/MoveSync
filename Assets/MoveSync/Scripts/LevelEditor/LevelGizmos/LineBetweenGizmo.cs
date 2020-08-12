using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class LineBetweenGizmo : BaseGizmoConnection
    {
        private LineRenderer _lineRenderer;

        public override void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            base.Start();
        }

        protected override void UpdateGizmo()
        {
            if (!_lineRenderer) _lineRenderer = GetComponent<LineRenderer>();
            
            _lineRenderer.SetPosition(0, _pivot.transform.position);
            _lineRenderer.SetPosition(1, _offset.transform.position);
        }
    }
}