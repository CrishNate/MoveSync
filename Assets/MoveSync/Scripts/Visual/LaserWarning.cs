using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class LaserWarning : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        private Vector3 _scale;
        private Color[] _savedColor;
        private float _savedWidth;
        
        private float _radius;
        private float _startTimeStamp;
        private float _appearTime;

        private static float _distance = 10000.0f;
        private static Vector3 _direction = Vector3.forward * _distance;
        private static float _marginHit = 0.5f;
        private static float _marginHitMove = 1.5f;
        private static float _invWarningAppearTime = 1f / 0.25f;

        
        public void Init(float startTimeStamp, float appearTime, float radius)
        {
            _startTimeStamp = startTimeStamp;
            _appearTime = appearTime;
            _radius = radius;
        }

        void Start()
        {
            transform.localScale = Vector3.zero;

            _savedColor = new Color[_meshRenderer.materials.Length];
            for (int i = 0; i < _savedColor.Length; i++)
                _savedColor[i] = _meshRenderer.materials[i].color;
        }
        
        void Update()
        {
            float appearDelta = (LevelSequencer.instance.timeBPM - _startTimeStamp) / _appearTime;
            float dRadius = Mathf.Min(1f, (LevelSequencer.instance.timeBPM - _startTimeStamp) * _invWarningAppearTime);
            float appearRadius = _radius * dRadius;
            
            RaycastHit hitResult;
            bool hit = Physics.SphereCast(transform.position, appearRadius * 0.5f, transform.rotation * _direction, out hitResult, _distance, 1 << LayerMask.NameToLayer("Player"));
            float distance = (hit ? hitResult.distance : _distance) - _marginHit - _marginHitMove * (1 - appearDelta);
            
            var parent = transform.parent;
            transform.transform.parent = null;
            transform.localScale = new Vector3(appearRadius, appearRadius, distance);
            transform.transform.parent = parent;

            UpdateColor(appearDelta);
        }

        void UpdateColor(float appearDelta)
        {
            for (int i = 0; i < _savedColor.Length; i++)
            {
                Color color = _meshRenderer.materials[i].color;
                color.a = appearDelta;
                _meshRenderer.materials[i].color = color;
            }
        }
    }
}
