using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    enum BeatShooterStates
    {
        None,
        Moving,
        Prepare,
        Shoot,
        Finish
    }

    [SerializeField]
    enum TargetStates
    {
        Direction,
        Camera,
    }

    [Serializable]
    public struct ShootData
    {
        public bool _isBullet;
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
        [SerializeField] private Transform _shootTransform;

        [Header("Beat Shooter")] [SerializeField]
        private float _preShootDelay;

        [SerializeField] private bool _rescaleBeatObjectByProjectalieScale;
        [SerializeField] private float _shootAppearTime;
        [SerializeField] private float _projectileScale = 1.0f;
        [SerializeField] private ShootData _shootData;
        [SerializeField] private TargetStates _targetStates;

        [Header("Beat Shooter Events")] [SerializeField]
        private UnityEvent _onPrepare;

        [SerializeField] private UnityEvent _onFinished;

        private ExTransformData _transformOrigin;
        private float _totalDuration;

        private float _shootTimeMarker = -1;
        private int _shootIndexMarker = 0;

        private BeatShooterStates _state;

        void Start()
        {
            _transformOrigin = new ExTransformData
            {
                position = transform.position,
                rotation = transform.rotation
            };

            _totalDuration = _shootData.GetTotalTime();

            _state = BeatShooterStates.Moving;
            if (_rescaleBeatObjectByProjectalieScale) transform.localScale = transform.localScale * _projectileScale;
        }

        float GetShootTimeMarker()
        {
            return beatObjectData.time - _shootAppearTime;
        }

        void UpdateShoot()
        {
            if (_shootIndexMarker == _shootData._shoots) return;

            if (LevelSequencer.instance.timeBPM > _shootTimeMarker)
            {
                Instantiate(_projectileObject, _shootTransform.position, _shootTransform.rotation)
                    .GetComponent<BaseProjectile>()
                    .Init(gameObject, _shootTimeMarker, _shootData._duration, _shootAppearTime, _projectileScale);

                _shootIndexMarker++;
                _shootTimeMarker = GetShootTimeMarker() + _shootData.GetNextShootTime() * _shootIndexMarker;
            }
        }

        Quaternion GetRotationByTargetState()
        {
            // switch (_targetStates)
            // {
            //     case TargetStates.Direction:
            //         return beatObjectData.transformData.rotation;
            //     case TargetStates.Camera:
            //         return Quaternion.LookRotation(Camera.main.transform.position - beatObjectData.transformData.position);
            // }

            return Quaternion.identity;
        }

        void UpdateMovement()
        {
            // float dTimeAppear = (LevelSequencer.instance.timeBPM - appearTimeBPM) /
            //                     (GetShootTimeMarker() - _preShootDelay - appearTimeBPM);
            //
            // dTimeAppear = Mathf.Min(1.0f, dTimeAppear);
            // dTimeAppear = (1 - Mathf.Pow(1 - dTimeAppear, 2.0f));
            //
            // if (Math.Abs(dTimeAppear - 1.0f) < Mathf.Epsilon)
            // {
            //     _onPrepare.Invoke();
            //     _state = BeatShooterStates.Prepare;
            // }
            //
            // // transform.position = _transformOrigin.position +
            // //                      (beatObjectData.transformData.position - _transformOrigin.position) * dTimeAppear;
            //
            // float maxAngle = Quaternion.Angle(_transformOrigin.rotation, GetRotationByTargetState());
            // transform.rotation =
            //     Quaternion.RotateTowards(_transformOrigin.rotation, GetRotationByTargetState(), maxAngle * dTimeAppear);
        }

        protected override void Update()
        {
            base.Update();

            // movement logic
            if (_state == BeatShooterStates.Moving)
            {
                UpdateMovement();
            }

            // shoot event logic
            if (LevelSequencer.instance.timeBPM >= GetShootTimeMarker())
            {
                if (_state != BeatShooterStates.Finish
                    && LevelSequencer.instance.timeBPM <= beatObjectData.time + _totalDuration)
                {
                    _state = BeatShooterStates.Shoot;
                }

                UpdateShoot();
            }
            else
            {
                _shootIndexMarker = 0;
                _shootTimeMarker = GetShootTimeMarker();
            }

            // end state logic
            if (LevelSequencer.instance.timeBPM >=
                (_shootData._isBullet ? GetShootTimeMarker() + _totalDuration : beatObjectData.time + _totalDuration))
            {
                if (_state != BeatShooterStates.Finish)
                {
                    _state = BeatShooterStates.Finish;
                    _onFinished.Invoke();
                }
            }
        }

        public ShootData shootData => _shootData;
        public float projectileScale => _projectileScale;
    }
}