using System.Collections;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatShootAround : BeatObject
    {
        [Header("Beat Projectile Around")]
        [SerializeField] private float _projectileReachTimeBPM = 1f;
        [SerializeField] private Animator _animator;

        private Vector3 _positionOrigin;
        private Vector3 _positionEnd;
        private float _appear;
        private float _duration;
        private float _size;
        private float _speed;
        private Mesh _shape;

        private bool _finishMove;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _positionEnd = beatObjectData.getModel<POSITION>().value;
            _positionOrigin = _positionEnd + new Vector3(0, -10.0f, 0);
            _speed = beatObjectData.getModel<SPEED>().value;
            
            _appear = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            
            transform.localScale *= _size;
            transform.position = _positionOrigin;

            if (beatObjectData.tryGetModel<SHAPE>(out var shape))
            {
                _shape = shape.mesh;
                MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
                if (meshFilter)
                    meshFilter.sharedMesh = _shape;
            }
            
            _animator.speed = LevelSequencer.instance.toBPM;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / _appear;
            if (dTimeMove > 1.0f) 
                _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = Mathf.Max(0, 1 - Mathf.Pow(1 - dTimeMove, 4.0f));

            transform.position = Vector3.Lerp(_positionOrigin, _positionEnd, dTimeMove);
        }

        void SpawnAround()
        {
            transform.position = _positionEnd;

            MathEx.ExecuteInIsometricSphere(SpawnProjectile, beatObjectData.getModel<COUNT>().value);
        }

        IEnumerator ShootSequence()
        {
            float dTime;

            do
            {
                dTime = (LevelSequencer.instance.timeBPM - beatObjectData.time) / _duration;

                SpawnAround();
                yield return new WaitForSeconds(0.2f);
            } while (dTime < 1.0f);
            
            Destroy(gameObject);
        }
        
        void SpawnProjectile(Vector3 direction)
        {
            direction = transform.rotation * direction;
            BaseProjectile.ProjectileParam initParam = new BaseProjectile.ProjectileParam
            {
                instigator = gameObject,
                invokeTimeStamp = LevelSequencer.instance.timeBPM,
                duration = _projectileReachTimeBPM,
                scale = _size,
                speed = _speed,
                shape = _shape
            };
            
            Instantiate(beatObjectData.getModel<PROJECTILE>().projectile.gameObject, transform.position, Quaternion.LookRotation(direction))
                .GetComponent<BaseProjectile>()
                .Init(initParam);
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateMovement();
        }

        protected override void OnTriggered()
        {
            base.OnTriggered();

            StartCoroutine(ShootSequence());
            _animator.enabled = false;
        }
    }
}