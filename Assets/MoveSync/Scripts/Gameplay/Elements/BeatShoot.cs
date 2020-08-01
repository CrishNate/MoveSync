using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [SerializeField]
    enum TargetStates
    {
        Direction,
        Camera,
    }

    [Serializable]
    public struct ShootData
    {
        public int _shoots;
        public float _duration;
        public float _delay;
        public float _postShootDelay;

        public float GetNextShootTime()
        {
            return _duration + _delay;
        }

        public float GetTotalTime()
        {
            return _duration * _shoots + _delay * Mathf.Max(0, _shoots - 1) + _postShootDelay;
        }
    }

    [Serializable]
    public class UnityEventFloatParam : UnityEvent<float>
    {
    };

    public class BeatShoot : BeatObject
    {
        [SerializeField] private GameObject _projectileObject;
        
        [Header("Beat Shooter")]
        [SerializeField] private TargetStates _targetStates;
        [SerializeField] private Transform _shootTransform;
        [SerializeField] private float _shootAppearTime = 0.2f;
        [SerializeField] private float _moveTime = 0.5f;

        [Header("Beat Shooter Events")] [SerializeField]
        private UnityEvent _onPrepare;

        [SerializeField] private UnityEvent _onFinished;

        private ExTransformData _transformOrigin;
        private ExTransformData _transformEnd;
        private float _appearDuration;
        private float _duration;
        private float _size;
        private float _shootTimeBPM;

        private bool _finishMove;
        private bool _shooted;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);
            
            _transformOrigin = new ExTransformData
            {
                position = transform.position,
                rotation = transform.rotation
            };
            _appearDuration = APPEAR.Get(beatObjectData.getModel(APPEAR.TYPE));
            _duration = DURATION.Get(beatObjectData.getModel(DURATION.TYPE));
            _size = SIZE.Get(beatObjectData.getModel(SIZE.TYPE));
            
            transform.localScale = transform.localScale * _size;
            _shootTimeBPM = beatObjectData.time - _shootAppearTime;
        }
        
        void UpdateShoot()
        {
            if (_shooted) return;
            
            if (LevelSequencer.instance.timeBPM > _shootTimeBPM)
            {
                _shooted = true;
                Instantiate(_projectileObject, _shootTransform.position, _shootTransform.rotation)
                    .GetComponent<BaseProjectile>()
                    .Init(gameObject, beatObjectData.time, _duration, _shootAppearTime, _size);
            }
        }

        Quaternion GetRotationByTargetState()
        {
            switch (_targetStates)
            {
                case TargetStates.Direction:
                    return _transformEnd.rotation;
                case TargetStates.Camera:
                    return Quaternion.LookRotation(Camera.main.transform.position - transform.position);
            }

            return Quaternion.identity;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / _moveTime;
            if (dTimeMove > 1.0f) _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = (1 - Mathf.Pow(1 - dTimeMove, 2.0f));
            
            if (Mathf.Abs(dTimeMove - 1.0f) < Mathf.Epsilon)
                _onPrepare.Invoke();

            transform.position = _transformOrigin.position +
                                 (_transformEnd.position - _transformOrigin.position) * dTimeMove;
            
            float maxAngle = Quaternion.Angle(_transformOrigin.rotation, GetRotationByTargetState());
            transform.rotation =
                Quaternion.RotateTowards(_transformOrigin.rotation, GetRotationByTargetState(), maxAngle * dTimeMove);
        }

        protected override void Update()
        {
            base.Update();

            // movement logic
            UpdateMovement();
            UpdateShoot();

            // end state logic
            if (LevelSequencer.instance.timeBPM >= beatObjectData.time + _duration)
            {
                _onFinished.Invoke();
            }
        }


        public float size => _size;
    }
}