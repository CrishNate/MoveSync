using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class Trace : MonoBehaviour
    {
        public float radius;
        private static float _distanceFromHit = 1.0f;

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Vector3 _direction;

        private float _distance;
        private Vector3 _scale;

        void Start()
        {
            _distance = _direction.magnitude;
            radius = GetComponentInParent<BeatShoot>().size;
            transform.localScale = new Vector3(radius, radius, 0.0f);
        }

        void Update()
        {
            RaycastHit hitResult;
            bool hit = Physics.SphereCast(transform.position, radius / 2.0f, transform.rotation * _direction,
                out hitResult, _distance, 1 << LayerMask.NameToLayer("Player"));
            float distance = (hit ? hitResult.distance : _distance) - _distanceFromHit;

            //_meshRenderer.material.mainTextureScale = new Vector2(1, distance / 4.0f);
            var parent = transform.parent;
            transform.transform.parent = null;
            transform.localScale = new Vector3(radius, radius, distance);
            transform.transform.parent = parent;
        }
    }
}
