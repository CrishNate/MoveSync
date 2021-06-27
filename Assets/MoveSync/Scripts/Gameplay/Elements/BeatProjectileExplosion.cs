using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatProjectileExplosion : BeatObject
    {
        [Header("Beat Projectile Explosion")]
        [SerializeField] private float _projectileReachTimeBPM = 1f;
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshFilter _meshFilter;

        private Vector3 _positionOrigin;
        private Vector3 _positionEnd;
        private float _appearDuration;
        private float _size;
        private Mesh _shape;

        private bool _finishMove;


        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            AnimatorStateInfo current = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(current.fullPathHash, 0, LevelSequencer.instance.timeBPM % 1.0f);

            _positionEnd = beatObjectData.getModel<POSITION>().value;
            _positionOrigin = _positionEnd + new Vector3(0, -10.0f, 0);
            
            _appearDuration = beatObjectData.getModel<APPEAR>().value;
            _size = beatObjectData.getModel<SIZE>().value;
            _shape = beatObjectData.getModel<SHAPE>().mesh;
            
            transform.localScale *= _size;
            transform.position = _positionOrigin;

            _animator.speed = LevelSequencer.instance.toBPM;
        }

        void UpdateMovement()
        {
            if (_finishMove) return;
            
            float dTimeMove = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / _appearDuration;
            if (dTimeMove > 1.0f) 
                _finishMove = true;
            
            dTimeMove = Mathf.Min(1.0f, dTimeMove);
            dTimeMove = Mathf.Max(0, 1 - Mathf.Pow(1 - dTimeMove, 4.0f));

            transform.position = Vector3.Lerp(_positionOrigin, _positionEnd, dTimeMove);
        }

        void SpawnExplosion()
        {
            transform.position = _positionEnd;

            MathEx.ExecuteInIsometricSphere(SpawnProjectile, beatObjectData.getModel<COUNT>().value);
        }

        void SpawnProjectile(Vector3 direction)
        {
            BaseProjectile.ProjectileParam initParam = new BaseProjectile.ProjectileParam
            {
                instigator = gameObject,
                invokeTimeStamp = beatObjectData.time,
                duration = _projectileReachTimeBPM,
                scale = _size,
                speed = beatObjectData.getModel<SPEED>().value,
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

            SpawnExplosion();
            
            Destroy(gameObject, LevelSequencer.instance.toTime * 0.2f);
        }
    }
}