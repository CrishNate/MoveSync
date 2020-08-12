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
            foreach (var modelInput in beatObjectData.modelInputsData)
            {
                if (modelInput is POSITION input)
                {
                    GameObject gameObject = Instantiate(instancePositionGizmo);
                    gameObject.GetComponent<PositionGizmo>().InitGizmo(beatObjectData.id, input);
                    _gizmos.Add(gameObject);
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
