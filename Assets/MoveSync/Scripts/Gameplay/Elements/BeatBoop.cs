using System;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatBoop : BeatObject
    {
        private float _appear;
        private float _duration;

        [Header("Beat Boop")]
        [SerializeField] private Animator _animator;

        private bool _finishMove;
        private MeshRenderer _meshRenderer;
        
        private static readonly int Appear = Animator.StringToHash("appear");
        private static readonly int Duration = Animator.StringToHash("duration");

        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            
            _appear = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;

            if (MoveSyncData.instance.shapeData.shapes.TryGetValue(beatObjectData.getModel<SHAPE>().value, out Mesh mesh))
                GetComponentInChildren<MeshFilter>().sharedMesh = mesh;

            transform.localScale *= beatObjectData.getModel<SIZE>().value;
            transform.position = beatObjectData.getModel<POSITION>().value;
            
            // perfect sync
            AnimatorStateInfo current = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(current.fullPathHash, 0, (LevelSequencer.instance.timeBPM - spawnTimeBPM) % 1.0f);
            
            _animator.SetFloat(Appear, 1.0f / _appear);
            _animator.SetFloat(Duration, 1.0f / _duration);
            _animator.speed = LevelSequencer.instance.toBPM;
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!(spawnTimeBPM > 0)) return;
            
            float dTimeDuration = _duration > 0 ? Mathf.Max(0, LevelSequencer.instance.timeBPM - (spawnTimeBPM + _appear)) / _duration : 0.0f;

                if (dTimeDuration > 1)
                    Destroy(gameObject);
        }
    }
}