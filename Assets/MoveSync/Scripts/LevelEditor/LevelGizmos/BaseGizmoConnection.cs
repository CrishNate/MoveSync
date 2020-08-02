using UnityEngine;

namespace MoveSync.LevelGizmos
{
    public abstract class BaseGizmoConnection : MonoBehaviour
    {
        protected BaseGizmo _gizmos1;
        protected BaseGizmo _gizmos2;

        public virtual void Initialize(BaseGizmo gizmo1, BaseGizmo gizmo2)
        {
            _gizmos1 = gizmo1;
            _gizmos2 = gizmo2;
            
            _gizmos1.onGizmoMoved.AddListener(x=>UpdateGizmo());
            _gizmos2.onGizmoMoved.AddListener(x=>UpdateGizmo());

            UpdateGizmo();
        }
        
        protected virtual void UpdateGizmo() { }
    }
}