using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public abstract class BeatObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onTriggeredEvent;

        private BeatObjectData _beatObjectData;
        private bool _triggered;

        protected virtual void Update()
        {
            if (dAppearTime < 0)
            {
                Destroy(gameObject);
                return;
            }

            if (dAppearTime >= 1.0f)
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

        protected void OnTriggered()
        {
            _onTriggeredEvent.Invoke();
        }

        protected BeatObjectData beatObjectData => _beatObjectData;
        protected float appearTimeBPM => _beatObjectData.time - _beatObjectData.appearDuration;
        protected float dAppearTime => (LevelSequencer.instance.timeBPM - _beatObjectData.time) / (_beatObjectData.time - appearTimeBPM);
    }
}