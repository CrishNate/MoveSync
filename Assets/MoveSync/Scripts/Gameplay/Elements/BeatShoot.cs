using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class BeatShoot : BeatObject
    {
        [SerializeField] private GameObject _projectileObject;

        private Vector3 _position;
        private float _duration;
        private float _appear;
        private float _size;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _position = beatObjectData.getModel<POSITION>().value;

            _appear = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            transform.position = _position;

            transform.rotation = GetRotationByTargetState();

            Shoot();
        }
        
        void Shoot()
        {
            Instantiate(_projectileObject, transform).GetComponent<BaseProjectile>().Init(gameObject, beatObjectData.time, _duration, _appear, _size);
        }

        Quaternion GetRotationByTargetState()
        {
            return Quaternion.LookRotation(PlayerBehaviour.instance.transform.position - transform.position);
        }

        protected override void Update()
        {
            base.Update();

            if (LevelSequencer.instance.timeBPM >= beatObjectData.time + _duration + 1.0f)
            {
                Destroy(gameObject);
            }
        }

        public float size => _size;
    }
}