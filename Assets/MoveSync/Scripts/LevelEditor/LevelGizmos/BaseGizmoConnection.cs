using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public abstract class BaseGizmoConnection : MonoBehaviour
    {
        [SerializeField] protected BaseGizmo _pivot;
        [SerializeField] protected BaseGizmo _offset;

        public virtual void Start()
        {
            _pivot.onGizmoMoved.AddListener(x=>UpdateGizmo());
            _offset.onGizmoMoved.AddListener(x=>UpdateGizmo());

            UpdateGizmo();
        }
        
        protected virtual void UpdateGizmo() { }
    }
}