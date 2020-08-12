using System.Collections;
using System.Collections.Generic;
using MoveSync.LevelGizmos;
using MoveSync.ModelData;
using MoveSync.UI;
using UnityEngine;

namespace MoveSync
{
    public class LevelGizmosManager : MonoBehaviour
    {
        [SerializeField] private GameObject instancePositionGizmo;
        private List<GameObject> _gizmos = new List<GameObject>();
        
        
        void Start()
        {
            ObjectProperties.instance.onSelected.AddListener(OnSelected);
            ObjectProperties.instance.onDeselected.AddListener(OnDeselected);
        }

        void OnSelected(BeatObjectData beatObjectData)
        {
            GameObject gizmo = null;
            foreach (var modelInput in beatObjectData.modelInputsData)
            {
                if (modelInput is POSITION || modelInput is ROTATION)
                {
                    if (gizmo == null)
                    {
                        gizmo = Instantiate(instancePositionGizmo);
                        _gizmos.Add(gizmo);
                    }
                }

                if (modelInput is ROTATION rotation)
                {
                    RotationGizmo rotationGizmo = gizmo.GetComponentInChildren<RotationGizmo>();
                    rotationGizmo.enabled = true;
                    rotationGizmo.InitGizmo(beatObjectData.id, rotation);
                }

                if (modelInput is POSITION position)
                {
                    PositionGizmo positionGizmo = gizmo.GetComponent<PositionGizmo>();
                    positionGizmo.enabled = true;
                    positionGizmo.InitGizmo(beatObjectData.id, position);
                }
            }
        }
        
        void OnDeselected(BeatObjectData beatObjectData)
        {
            foreach (var gizmo in _gizmos)
            {
                Destroy(gizmo);
            }
            _gizmos.Clear();
        }
    }
}
