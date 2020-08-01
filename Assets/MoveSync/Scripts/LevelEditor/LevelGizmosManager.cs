using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class LevelGizmosManager : MonoBehaviour
    {
        private GameObject[] _gizmos;
        
        void Start()
        {
            SelectionManager.instance.onSelected.AddListener(OnSelected);
            SelectionManager.instance.onDeselected.AddListener(OnDeselected);
        }

        void OnSelected(BeatObjectData beatObjectData)
        {
            
        }
        
        void OnDeselected(BeatObjectData beatObjectData)
        {
            foreach (var gizmo in _gizmos)
            {
                Destroy(gizmo);
            }
        }
    }
}
