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
        [SerializeField] private int _spawnCount;

        [SerializeField] private float _maxAppearTime = 0.5f;
        [SerializeField] private float _projectileReachTimeBPM = 1f;

        private ExTransformData _transformOrigin;
        private ExTransformData _transformEnd;
        private float _appearDuration;
        private float _size;

        private bool _finishMove;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);
            
            _transformOrigin = new ExTransformData
            {
                position = transform.position,
                rotation = transform.rotation
            };

            _transformEnd = new ExTransformData
            {
                position = transform.position + new Vector3(0, 10.0f, 0),
            };
            
            _appearDuration = APPEAR.Get(beatObjectData.getModel(APPEAR.TYPE));
            _size = SIZE.Get(beatObjectData.getModel(SIZE.TYPE));
            
            transform.localScale = transform.localScale * _size;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / Mathf.Max(_maxAppearTime, _appearDuration);
            if (dTimeMove > 1.0f) _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = (1 - Mathf.Pow(1 - dTimeMove, 2.0f));

            transform.position = _transformOrigin.position +
                                 (_transformEnd.position - _transformOrigin.position) * dTimeMove;
        }

        void Icosphere()
        {
            float offset = Random.Range(0, 360.0f);
            
            int ia, ib, nb;
            float x, y, z, r;
            float a, b, da, db;
            da = Mathf.PI / (_spawnCount - 1);
            for (a = -0.5f * Mathf.PI, ia = 0; ia < _spawnCount; ia++, a += da)
            {
                r = Mathf.Cos(a);
                z = Mathf.Sin(a);
                nb = Mathf.CeilToInt(2.0f * Mathf.PI * r / da);
                db = 2.0f * Mathf.PI / nb;
                if ((ia == 0) || (ia == _spawnCount - 1))
                {
                    nb = 1;
                    db = 0.0f;
                }

                for (b = 0.0f, ib = 0; ib < nb; ib++, b += db)
                {
                    x = r * Mathf.Cos(b + offset);
                    y = r * Mathf.Sin(b + offset);
                    SpawnProjectile(new Vector3(x, y, z));
                    double w = 1.2;
                }
            }
        }

        void SpawnExplosion()
        {
            // Evenly distributing n points on a sphere
            float n = _spawnCount * 0.5f;
            for (int x = 0; x < n; x++)
            {
                float lon = 360.0f * ((x + 0.5f) / n);
                
                for (int y = 0; y < n; y++)
                {
                    float midpt = (y + 0.5f) / n;
                    float lat = 180 * Mathf.Asin(2 * ((y + 0.5f) / n - 0.5f));
                    SpawnProjectile(new Vector3(lon, lat, 0));
                }
            }
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
            
            UpdateMovement();

            if (LevelSequencer.instance.timeBPM >= beatObjectData.time)
            {
                Icosphere();
                Destroy(gameObject);
            }
        }
    }
}