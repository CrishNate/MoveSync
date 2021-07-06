using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public abstract class BeatObject : MonoBehaviour, BeatUpdate
    {
        [SerializeField] private UnityEvent _onTriggeredEvent;

        private BeatObjectData _beatObjectData;
        private bool _triggered;
        private float _spawnTimeBPM;

        public virtual void Init(BeatObjectData beatObjectData)
        {
            _beatObjectData = beatObjectData;
            _spawnTimeBPM = beatObjectData.spawnTime;
            
            LevelSequencer.instance.beatObjectInstances.Add(this);
        }

        public void OnDestroy()
        {
            if (LevelSequencer.isShutDown) 
                return;
        
            LevelSequencer.instance.beatObjectInstances.Remove(this);
        }

        public virtual void InnerUpdate()
        {
            if (LevelSequencer.instance.timeBPM < _spawnTimeBPM)
            {
                Destroy(gameObject);
                return;
            }
            
            if (LevelSequencer.instance.timeBPM >= beatObjectData.time)
            {
                if (!_triggered)
                {
                    OnTriggered();
                    _triggered = true;
                }
            }
            else
            {
                _triggered = false;
            }
        }

        protected virtual void OnTriggered()
        {
            _onTriggeredEvent.Invoke();
        }

        protected BeatObjectData beatObjectData => _beatObjectData;
        protected float spawnTimeBPM => _spawnTimeBPM;
    }
}