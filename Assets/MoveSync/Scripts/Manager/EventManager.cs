using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<MoveSyncEvent, UnityEventFloatParam> _events = new Dictionary<MoveSyncEvent, UnityEventFloatParam>();


        public void InvokeEvent(MoveSyncEvent name, float timeBPM)
        {
            UnityEventFloatParam gotEvent;
            if (_events.TryGetValue(name, out gotEvent))
            {
                gotEvent.Invoke(timeBPM);   
            }
            else
            {
                Debug.Log("No Event Is Binded");
            }
        }
        
        public void BindEvent(MoveSyncEvent name, UnityAction<float> action)
        {
            UnityEventFloatParam gotEvent;
            if (!_events.TryGetValue(name, out gotEvent))
            {
                _events.Add(name, gotEvent = new UnityEventFloatParam());
            }
            
            gotEvent.AddListener(action);
        }

        public void UnbindEvent(MoveSyncEvent name, UnityAction<float> action)
        {
            if (_events.TryGetValue(name, out UnityEventFloatParam gotEvent))
            {
                gotEvent.RemoveListener(action);
            }
        }
    }
}