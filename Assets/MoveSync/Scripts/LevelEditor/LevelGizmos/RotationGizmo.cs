using System;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class RotationGizmo : MonoBehaviour
    {
        [SerializeField] private BaseGizmo _forwardGizmo;
        [SerializeField] private BaseGizmo _upGizmo;
        [SerializeField] private GameObject _forwardLine;

        private int _id;
        private ROTATION _rotation;

        
        public void InitGizmo(int id, ROTATION rotation)
        {
            _rotation = rotation;
            _id = id;

            _forwardGizmo.gameObject.SetActive(true);
            _upGizmo.gameObject.SetActive(true);
            _forwardLine.gameObject.SetActive(true);
            
            _forwardGizmo.onGizmoMoved.AddListener(x => GizmoMoved());
            _upGizmo.onGizmoMoved.AddListener(x => GizmoMoved());

            UpdateGizmo(id);
        }

        public void UpdateGizmo(int id)
        {
            if (id != _id) return;

            Quaternion rotation = _rotation.value;
            
            _forwardGizmo.transform.position = transform.position + rotation * Vector3.forward * 2.0f;
            _upGizmo.transform.position = transform.position + rotation * Vector3.up * 1.0f;
            _forwardLine.transform.rotation = rotation;
        }

        void GizmoMoved()
        {
            Vector3 forward = _forwardGizmo.transform.position - transform.position;
            Vector3 up = _upGizmo.transform.position - transform.position;
            _rotation.value = Quaternion.LookRotation(forward, up);
            
            LevelDataManager.instance.UpdateBeatObject(_id);
            UpdateGizmo(_id);
        }

        private void Start()
        {
            LevelDataManager.instance.onUpdateObject.AddListener(UpdateGizmo);
        }

        private void OnDestroy()
        {
            if (LevelDataManager.instance != null)
            {
                LevelDataManager.instance.onUpdateObject.RemoveListener(UpdateGizmo);
            }
        }
    }
}