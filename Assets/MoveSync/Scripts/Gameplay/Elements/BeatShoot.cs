using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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

    public class BeatShoot : BeatObject
    {
        [SerializeField] private GameObject _projectileObject;

        [Header("Beat Shooter")] 
        [SerializeField] private GameObject _shootPreview;
        [SerializeField] private TargetStates _targetStates;
        [SerializeField] private Transform _shootTransform;
        [SerializeField] private float _shootAppearTime = 0.2f;
        [SerializeField] private float _moveTime = 0.5f;

        [SerializeField] private UnityEvent _onFinished;

        private Vector3 _positionOrigin;
        private Vector3 _positionEnd;
        private float _appearDuration;
        private float _duration;
        private float _size;
        private float _shootTimeBPM;

        private bool _finishMove;
        private bool _shooted;
        private bool _finished;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _positionEnd = beatObjectData.getModel<POSITION>().value;
            _positionOrigin = _positionEnd;
            _positionOrigin.y = -10;

            Vector3 offset = Random.insideUnitCircle * 20.0f;
            offset = new Vector3(offset.x, 10, offset.y);

            _appearDuration = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            
            _shootTimeBPM = beatObjectData.time - _shootAppearTime;
            //transform.localScale = transform.localScale * _size;
            transform.position = _positionOrigin;
        }
        
        void UpdateShoot()
        {
            if (_shooted) return;
            
            if (LevelSequencer.instance.timeBPM > _shootTimeBPM)
            {
                _shooted = true;
                if (_shootPreview)_shootPreview.SetActive(false);

                Instantiate(_projectileObject, _shootTransform.position, _shootTransform.rotation)
                    .GetComponent<BaseProjectile>()
                    .Init(gameObject, _shootTimeBPM, _duration, _shootAppearTime, _size);
            }
        }

        Quaternion GetRotationByTargetState()
        {
            switch (_targetStates)
            {
                case TargetStates.Camera:
                    return Quaternion.LookRotation(PlayerBehaviour.instance.transform.position - transform.position);
            }

            return Quaternion.identity;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / _moveTime;
            if (dTimeMove > 1.0f)
            {
                if (_shootPreview)_shootPreview.SetActive(true);
                _finishMove = true;
            }
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = (1 - Mathf.Pow(1 - dTimeMove, 2.0f));

            transform.position = _positionOrigin + (_positionEnd - _positionOrigin) * dTimeMove;
            
            float maxAngle = Quaternion.Angle(transform.rotation, GetRotationByTargetState());
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, GetRotationByTargetState(), maxAngle * dTimeMove);
        }

        protected override void Update()
        {
            base.Update();

            UpdateMovement();
            UpdateShoot();

            if (LevelSequencer.instance.timeBPM >= beatObjectData.time + _duration)
            {
                if (!_finished)
                {
                    _onFinished.Invoke();
                    _finished = true;
                }
            }
        }


        public float size => _size;
    }
}