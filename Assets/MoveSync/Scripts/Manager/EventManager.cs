using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<PropertyName, UnityEventFloatParam> _events = new Dictionary<PropertyName, UnityEventFloatParam>();


        public void InvokeEvent(PropertyName name, float timeBPM)
        {
            UnityEventFloatParam gotEvent;
            if (_events.TryGetValue(name, out gotEvent))
            {
                gotEvent.Invoke(timeBPM);   
            }
            else
            {
                Debug.LogError("No Event Is Binded");
            }
        }
        
        public void BindEvent(PropertyName name, UnityAction<float> action)
        {
            UnityEventFloatParam gotEvent;
            if (!_events.TryGetValue(name, out gotEvent))
            {
                _events.Add(name, gotEvent = new UnityEventFloatParam());
            }
            
            gotEvent.AddListener(action);
        }
    }
}