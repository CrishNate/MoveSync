using System;
using UnityEngine;

namespace MoveSync
{
    public class SkyVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private float timeBPM = -1;
        
        private static readonly float Duration = 2.0f;
        private static readonly float MaxExposure = 0.3f;
        
        private void Start()
        {
            EventManager.instance.BindEvent(MoveSyncEvent.SkyFlicker, Flicker);
        }

        private void OnDestroy()
        {
            if (!EventManager.isShutDown)
                EventManager.instance.UnbindEvent(MoveSyncEvent.SkyFlicker, Flicker);
        }

        private void Update()
        {
            _meshRenderer.material.SetFloat($"_Rotation", Time.time * 0.1f);

            float dDuration = (LevelSequencer.instance.timeBPM - timeBPM) / Duration;
            if (dDuration > 1 || dDuration < 0 || timeBPM < 0)
                return;

            if (dDuration > 0)
            {
                _meshRenderer.material.SetFloat($"_Exposure", Mathf.Clamp(1 - dDuration, 0, 1) * MaxExposure);
            }
        }

        void Flicker(float time)
        {
            timeBPM = time;
        }
    }
}