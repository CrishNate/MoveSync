using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync.LevelGizmos
{
    public class BaseGizmo : MonoBehaviour
    {
        public UnityEventVector3 onGizmoMoved = new UnityEventVector3();
        private float _mouseZ;
        private Vector3 _mouseOffset;
        private bool _dragging;
        private static GameObject _currentHoldingGizmo;


        protected virtual void OnGizmoMove()
        {
            onGizmoMoved.Invoke(transform.position);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // left click
            {
                if (_currentHoldingGizmo == null)
                {
                    var position = gameObject.transform.position;
                    Vector3 viewPosition = Camera.main.WorldToScreenPoint(position);

                    if (Vector2.Distance(Input.mousePosition, viewPosition) < 30.0f)
                    {
                        _mouseZ = viewPosition.z;
                        _mouseOffset = position - GetMouseWorldPosition();
                        _dragging = true;
                        _currentHoldingGizmo = gameObject;
                    }
                }
            }

            if (Input.GetMouseButton(0)) // left hold
            {
                if (_dragging)
                {
                    transform.position = GetMouseWorldPosition() + _mouseOffset;
                    OnGizmoMove();
                }
            }

            if (Input.GetMouseButtonUp(0)) // left release
            {
                _dragging = false;
                _currentHoldingGizmo = null;
            }
            
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                _mouseZ += Input.mouseScrollDelta.y;
            }
        }

        Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _mouseZ;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}