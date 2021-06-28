using System;
using UnityEngine;

namespace MoveSync
{
    public class NearColorBeatObject : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private static float _invNearDistance = 1f / 6.0f;
        private static float _nearFull = 1f / 6.0f;
        private static readonly int Override = Shader.PropertyToID("Override");


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
            float dColor = (PlayerBehaviour.instance.transform.position - transform.position).magnitude - _nearFull;
            dColor = Mathf.Clamp(dColor * _invNearDistance, 0, 1);

            if (initial || dColor < 1.0f)
            {
                _meshRenderer.material.SetFloat(Override, dColor);
            }
        }
    }
}