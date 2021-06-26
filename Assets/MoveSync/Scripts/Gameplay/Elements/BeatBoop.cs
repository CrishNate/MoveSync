using System;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatBoop : BeatObject
    {
        private float _appear;
        private float _duration;
        private float _size;

        private bool _finishMove;
        private MeshRenderer _meshRenderer;
        
        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);

            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            
            _appear = beatObjectData.getModel<APPEAR>().value;
            _duration = beatObjectData.getModel<DURATION>().value;
            _size = beatObjectData.getModel<SIZE>().value;

            if (MoveSyncData.instance.shapeData.shapes.TryGetValue(beatObjectData.getModel<SHAPE>().value, out Mesh mesh))
                GetComponentInChildren<MeshFilter>().sharedMesh = mesh;

            transform.localScale = Vector3.zero;
            transform.position = beatObjectData.getModel<POSITION>().value;
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!(spawnTimeBPM > 0)) return;
            
            float dTimeAppear = _appear > 0 ? Mathf.Max(0, LevelSequencer.instance.timeBPM - spawnTimeBPM) / _appear : 0.0f;
            float dTimeDuration = _duration > 0 ? Mathf.Max(0, LevelSequencer.instance.timeBPM - (spawnTimeBPM + _appear)) / _duration : 0.0f;

            if (dTimeDuration <= 0)
            {
                float delta = Mathf.Clamp(dTimeAppear, 0, 1);
                    
                transform.localScale = Vector3.one * _size * delta;
                UpdateColor(delta);
            }
            else
            {
                float delta = Mathf.Clamp((1 - dTimeDuration), 0, 1);
                transform.localScale = Vector3.one * _size * delta;
                UpdateColor(delta);

                if (dTimeDuration > 1)
                    Destroy(gameObject);
            }
        }
        
        void UpdateColor(float delta)
        {
            float alpha = _meshRenderer.material.color.a;
            _meshRenderer.material.color = Color.Lerp(
                MoveSyncData.instance.colorData.NearBeatObjectColor,
                Color.white,
                delta);
            _meshRenderer.material.color = new Color(_meshRenderer.material.color.r,
                _meshRenderer.material.color.g,
                _meshRenderer.material.color.b,
                alpha);
        }
    }
}