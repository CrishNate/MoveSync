using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class LaserProjectile : BaseProjectile
    {
        [SerializeField] private GameObject _warningGameObject;
        [SerializeField] private LaserWarning _laserWarning;
        [SerializeField] private GameObject _laserGameObject;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _disapearMultiplier = 2.0f;
        [SerializeField] private float _scaleMultiplier = 1.0f;
        [SerializeField] private float _scaleTime = 0.2f;

        private BeatShoot _beatShoot;

        private Vector3 _savedScale;
        private Color _savedColor;
        private float _savedWidth;
        private bool isShooting;
        private float _endTimeStamp;
        

        public override void Init(ProjectileParam initParam)
        {
            base.Init(initParam);
            
            _savedScale = _laserGameObject.transform.localScale;
            _savedColor = _meshRenderer.material.color;

            _laserGameObject.transform.localScale = Vector3.zero;
            if (_lineRenderer) 
                _savedWidth = _lineRenderer.startWidth * initParam.size;

            _warningGameObject.SetActive(true);

            _endTimeStamp = timeStamp + duration;
            _laserWarning.Init(timeStamp - appearTime, appearTime, scale);
        }

        public override float GetDisappearTime()
        {
            return _scaleTime;
        }

        void Update()
        {
            if (timeStamp <= 0) return;

            float deltaScale = 0;
            float deltaScaleForward = 0;

            // laser warning
            if (LevelSequencer.instance.timeBPM < timeStamp - _scaleTime)
            {
                
            }
            // laser appear/shoot
            else if (LevelSequencer.instance.timeBPM < _endTimeStamp)
            {
                _warningGameObject.SetActive(false);
                _laserGameObject.SetActive(true);

                deltaScale = (LevelSequencer.instance.timeBPM - (timeStamp - _scaleTime)) / _scaleTime;
                deltaScaleForward = Mathf.Min(1, deltaScale);
            }
            // laser fade
            else
            {
                deltaScale = 1 - (LevelSequencer.instance.timeBPM - _endTimeStamp) / (_scaleTime * _disapearMultiplier);
                deltaScaleForward = 1.0f;

                if (deltaScale < 0)
                    Destroy(gameObject);
            }

            deltaScale = Mathf.Clamp(deltaScale, 0.0f, 1.0f);

            float powDeltaScale = Mathf.Pow(deltaScale, 3);
            _meshRenderer.material.color = _savedColor + (Color.white - _savedColor) * powDeltaScale;
            
            if (_lineRenderer) 
                _lineRenderer.startWidth = _lineRenderer.endWidth = _savedWidth * deltaScale;

            Vector3 tempScale = _savedScale;
            tempScale.x = tempScale.y = scale * deltaScale;
            tempScale.z = deltaScaleForward * _scaleMultiplier;
            _laserGameObject.transform.localScale = tempScale;
            
            if (LevelSequencer.instance.timeBPM < timeStamp - appearTime)
                Destroy(gameObject);
        }
    }
}
