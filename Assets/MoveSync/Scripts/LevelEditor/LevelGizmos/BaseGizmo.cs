using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync.LevelGizmos
{
    public class UnityEventVector3 : UnityEvent<Vector3>{ };

    public class BaseGizmo : MonoBehaviour
    {
        public UnityEventVector3 onGizmoMoved = new UnityEventVector3();
        private float _mouseZ;
        private Vector3 _mouseOffset;


        protected virtual void OnGizmoMove()
        {
            onGizmoMoved.Invoke(transform.position);
        }
        
        void OnMouseDown()
        {
            _mouseZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            _mouseOffset = gameObject.transform.position - GetMouseWorldPosition();
        }

        void OnMouseDrag()
        {
            transform.position = GetMouseWorldPosition() + _mouseOffset;
            OnGizmoMove();
        }

        private void Update()
        {
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