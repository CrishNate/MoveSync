using System.Collections;
using System.Collections.Generic;
using Oculus.Platform.Samples.VrHoops;
using UnityEngine;

namespace MoveSync
{
    public class LaserWarning : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        private Vector3 _scale;
        private Color _savedColor;
        private float _radius;
        private float _startTimeStamp;

        private static float _distance = 100.0f;
        private static Vector3 _direction = Vector3.forward * _distance;
        private static float _distanceFromHit = 1.0f;
        private static float _distanceOfApproach = 1.0f;
        private static float _invWarningAppearTime = 1f / 0.25f;

        
        public void Init(float startTimeStamp, float radius)
        {
            _startTimeStamp = startTimeStamp;
            _radius = radius;
        }

        void Start()
        {
            _distance = _direction.magnitude;
            transform.localScale = Vector3.zero;
            _savedColor = _meshRenderer.material.color;
        }
        
        void Update()
        {
            float dRadius = Mathf.Min(1f, (LevelSequencer.instance.timeBPM - _startTimeStamp) * _invWarningAppearTime);
            float appearRadius = _radius * dRadius;
            
            RaycastHit hitResult;
            bool hit = Physics.SphereCast(transform.position, appearRadius * 0.5f, transform.rotation * _direction, out hitResult, _distance, 1 << LayerMask.NameToLayer("Player"));
            float distance = (hit ? hitResult.distance : _distance) - _distanceFromHit;

            var parent = transform.parent;
            transform.transform.parent = null;
            transform.localScale = new Vector3(appearRadius, appearRadius, distance);
            transform.transform.parent = parent;

            UpdateDistanceColor();
        }

        void UpdateDistanceColor()
        {
            float distanceToLine = MathEx.DistanceToLine(transform.position, transform.rotation * _direction, PlayerBehaviour.instance.transform.position);
            float deltaColor = 1 - Mathf.Clamp((distanceToLine - _radius * 0.5f) / _distanceOfApproach, 0, 1);
            deltaColor *= deltaColor;
            
            Color color = _meshRenderer.material.color;
            color.a = Mathf.Lerp(0.05f, _savedColor.a, deltaColor);
            _meshRenderer.material.color = color;
        }
    }
}
