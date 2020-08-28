using System.Collections;
using System.Collections.Generic;
using MoveSync;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public class BeatDiscoBall : BeatObject
    {
        [Header("Beat Disco Ball")] 
        [SerializeField] private BaseProjectile _projectileObject;
        [SerializeField] private float _maxAppearTime = 0.5f;
        [SerializeField] private Vector3 _dRotation;

        private Vector3 _positionOrigin;
        private Vector3 _positionEnd;
        private float _appearDuration;
        private float _duration;
        private float _size;
        private bool _finishMove;
        private int _count;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _positionEnd = beatObjectData.getModel<POSITION>().value;
            _positionOrigin = _positionEnd + new Vector3(0, -10.0f, 0);
            
            _appearDuration = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            _count = beatObjectData.getModel<COUNT>().value;

            transform.localScale *= _size;
            transform.position = _positionOrigin;
            
            SpawnDisco();
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / Mathf.Max(_maxAppearTime, _appearDuration);
            if (dTimeMove > 1.0f) _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = Mathf.Max(0, 1 - Mathf.Pow(1 - dTimeMove, 2.0f));

            transform.position = Vector3.Lerp(_positionOrigin, _positionEnd, dTimeMove);
        }

        void SpawnDisco()
        {
            MathEx.ExecuteInIsometricSphere(SpawnProjectile, _count);
        }

        void SpawnProjectile(Vector3 direction)
        {
            GameObject projectileObj = Instantiate(_projectileObject.gameObject, transform.position + direction * _size, 
                Quaternion.LookRotation(direction));
            
            projectileObj.GetComponent<BaseProjectile>().Init(gameObject, beatObjectData.time, _duration, _appearDuration, _size, 0.0f);
            projectileObj.transform.parent = transform;
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateMovement();

            if (LevelSequencer.instance.timeBPM > beatObjectData.time + _duration + _projectileObject.GetDisappearTime())
                Destroy(gameObject);
        }
    }
}