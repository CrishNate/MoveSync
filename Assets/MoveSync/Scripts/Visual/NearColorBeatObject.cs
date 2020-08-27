using System;
using UnityEngine;

namespace MoveSync
{
    public class NearColorBeatObject : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private static float _invNearDistance = 1f / 6.0f;


        private void Start()
        {
            UpdateColor(true);
        }

        private void Update()
        {
            UpdateColor();
        }

        void UpdateColor(bool initial = false)
        {
            float dColor = (PlayerBehaviour.instance.transform.position - transform.position).magnitude;
            dColor = Mathf.Clamp(dColor * _invNearDistance, 0, 1);

            if (initial || dColor < 1.0f)
            {
                float alpha = _meshRenderer.material.color.a;
                _meshRenderer.material.color = Color.Lerp(
                    MoveSyncData.instance.colorData.NearBeatObjectColor,
                    MoveSyncData.instance.colorData.FarBeatObjectColor,
                    dColor);
                _meshRenderer.material.color = new Color(_meshRenderer.material.color.r,
                    _meshRenderer.material.color.g,
                    _meshRenderer.material.color.b,
                    alpha);
                
            }
        }
    }
}