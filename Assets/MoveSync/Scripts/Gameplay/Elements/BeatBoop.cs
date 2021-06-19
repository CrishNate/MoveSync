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

        public override void Init(BeatObjectData beatObjectData)
        {
            base.Init(beatObjectData);
            
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
            float dTimeAppear = (LevelSequencer.instance.timeBPM - spawnTimeBPM) / _appear;
            float dTimeDuration = (LevelSequencer.instance.timeBPM - (spawnTimeBPM + _appear)) / _duration;

            if (dTimeDuration <= 0)
                transform.localScale = Vector3.one * _size * Mathf.Clamp(Mathf.Pow(dTimeAppear, 0.5f), 0, 1);
            else
            {
                transform.localScale = Vector3.one * _size * Mathf.Clamp((1 - Mathf.Pow(dTimeDuration, 2)), 0, 1);

                if (dTimeDuration > 1)
                    Destroy(gameObject);
            }
        }

        protected override void OnTriggered()
        {
            base.OnTriggered();
        }
    }
}