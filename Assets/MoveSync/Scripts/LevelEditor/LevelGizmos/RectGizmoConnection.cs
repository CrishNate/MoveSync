using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class RectGizmoConnection : BaseGizmoConnection
    {
        protected override void UpdateGizmo()
        {
            Vector3 offset = _offset.transform.position - _pivot.transform.position;
            transform.position = _pivot.transform.position + offset * 0.5f;
            transform.localScale = offset;
        }
    }
}