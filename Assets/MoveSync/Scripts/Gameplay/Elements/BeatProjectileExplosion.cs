using System.Collections;
using System.Collections.Generic;
using MoveSync;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public class BeatProjectileExplosion : BeatObject
    {
        [SerializeField] private GameObject _projectileObject;

        [Header("Beat Projectile Explosion")] 
        [SerializeField] private int _divisions = 10;

        [SerializeField] private float _maxAppearTime = 0.5f;
        [SerializeField] private float _projectileReachTimeBPM = 1f;

        private Vector3 _positionOrigin;
        private Vector3 _positionEnd;
        private float _appearDuration;
        private float _size;
        private Vector3 _savedScale;

        private bool _finishMove;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _positionEnd = beatObjectData.getModel<POSITION>().value;
            _positionOrigin = _positionEnd + new Vector3(0, -10.0f, 0);
            
            _appearDuration = beatObjectData.getModel<APPEAR>().value;
            _size = beatObjectData.getModel<SIZE>().value;

            _savedScale = transform.localScale;
            transform.localScale = _savedScale * _size;
            transform.position = _positionOrigin;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / Mathf.Max(_maxAppearTime, _appearDuration);
            if (dTimeMove > 1.0f) _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = (1 - Mathf.Pow(1 - dTimeMove, 2.0f));

            transform.position = _positionOrigin + (_positionEnd - _positionOrigin) * dTimeMove;
        }

        void SpawnExplosion()
        {
            transform.position = _positionEnd;

            MathEx.ExecuteInIsometricSphere(SpawnProjectile, _divisions);
        }

        void SpawnProjectile(Vector3 direction)
        {
            Instantiate(_projectileObject, transform.position, Quaternion.LookRotation(direction))
                .GetComponent<BaseProjectile>()
                .Init(gameObject, beatObjectData.time, 0, _projectileReachTimeBPM, _size);
        }

        protected override void Update()
        {
            base.Update();
            transform.localScale = _savedScale * (_size + _size * 0.3f * Mathf.Cos(LevelSequencer.instance.timeBPM * Mathf.PI * 2f + Mathf.PI));
            
            UpdateMovement();
        }

        protected override void OnTriggered()
        {
            base.OnTriggered();

            SpawnExplosion();
            Destroy(gameObject);
        }
    }
}