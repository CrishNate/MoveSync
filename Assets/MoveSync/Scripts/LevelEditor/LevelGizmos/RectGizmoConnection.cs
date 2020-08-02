using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class RectGizmoConnection : BaseGizmoConnection
    {
        protected override void UpdateGizmo()
        {
            Vector3 offset = _gizmos2.transform.position - _gizmos1.transform.position;
            transform.position = _gizmos1.transform.position + offset * 0.5f;
            transform.localScale = offset;
        }
    }
}