using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync.UI
{
    public struct BindKey
    {
        public KeyCode key;
        public PropertyName objectTag;
    }

    public class BindingManager : MonoBehaviour
    {
        private Dictionary<int, BindKey> _bind = new Dictionary<int, BindKey>();

        
        public void AddKeyBind(int layer, BindKey bind)
        {
            if (_bind.ContainsKey(layer))
            {
                _bind[layer] = bind;
            }
            else
            {
                _bind.Add(layer, bind);
            }
        }
        
        public void RemoveKeyBind(int layer)
        {
            if (_bind.ContainsKey(layer))
            {
                _bind.Remove(layer);
            }
        }

        private void Update()
        {
            if (!LevelSequencer.instance.audioSource.isPlaying) return;
            
            foreach (var bind in _bind)
            {
                if (Input.GetKeyDown(bind.Value.key))
                {
                    LevelDataManager.instance.NewBeatObjectAtMarker(bind.Value.objectTag, bind.Key);
                }
            }
        }


        public Dictionary<int, BindKey> bind => _bind;
    }
}
