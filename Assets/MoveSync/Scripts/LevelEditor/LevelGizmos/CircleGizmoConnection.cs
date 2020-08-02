using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class CircleGizmoConnection : BaseGizmoConnection
    {
        private Vector3 initialScale;

        protected override void UpdateGizmo()
        {
            if (initialScale.Equals(Vector3.zero)) initialScale = transform.localScale; 
            
            Vector3 offset = _gizmos2.transform.position - _gizmos1.transform.position;
            transform.position = _gizmos1.transform.position;
            transform.localScale = initialScale * offset.magnitude;
        }
    }
}