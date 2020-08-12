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

            var position = _pivot.transform.position;
            Vector3 offset = _offset.transform.position - position;
            transform.position = position;
            transform.localScale = initialScale * offset.magnitude;
        }
    }
}