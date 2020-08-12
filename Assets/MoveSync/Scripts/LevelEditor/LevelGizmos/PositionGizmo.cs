using System;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public class PositionGizmo : MonoBehaviour
    {
        [SerializeField] private BaseGizmo _baseGizmo;
        [SerializeField] private BaseGizmo _offsetGizmo;

        [SerializeField] private GameObject _lineConnection;
        [SerializeField] private GameObject _circleConnection;
        [SerializeField] private GameObject _rectConnection;
        [SerializeField] private GameObject _sphereConnection;
        
        private int _id;
        private POSITION _position;

        
        public void InitGizmo(int id, POSITION position)
        {
            _position = position;
            _id = id;

            _baseGizmo.onGizmoMoved.AddListener(x => GizmoMoved());
            _offsetGizmo.onGizmoMoved.AddListener(x => GizmoMoved());

            UpdateGizmo(id);
            
            if (position.randomSpawnType != RandomSpawnType.None)
            {
                _offsetGizmo.transform.position = position.value + position.pivot;
            }
        }

        public void UpdateGizmo(int id)
        {
            if (id != _id) return;
            
            Hide();
            
            _baseGizmo.transform.position = _position.value;
            _offsetGizmo.transform.position = _baseGizmo.transform.position + _position.pivot;

            if (_position.randomSpawnType != RandomSpawnType.None)
                _offsetGizmo.gameObject.SetActive(true);

            switch (_position.randomSpawnType)
            {
                case RandomSpawnType.None:
                    break;
                case RandomSpawnType.Line:
                    _lineConnection.SetActive(true);
                    break;
                case RandomSpawnType.Rect:
                    _rectConnection.SetActive(true);
                    break;
                case RandomSpawnType.Circle:
                    _circleConnection.SetActive(true);
                    break;
                case RandomSpawnType.Sphere:
                    _sphereConnection.SetActive(true);
                    break;
            }
        }
        
        void Hide()
        {
            _lineConnection.SetActive(false);
            _circleConnection.SetActive(false);
            _rectConnection.SetActive(false);
            _sphereConnection.SetActive(false);
            _offsetGizmo.gameObject.SetActive(false);
        }

        void GizmoMoved()
        {
            _position.value = _baseGizmo.transform.position;
            _position.pivot = _offsetGizmo.transform.position - _baseGizmo.transform.position;
            
            LevelDataManager.instance.UpdateBeatObject(_id);
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