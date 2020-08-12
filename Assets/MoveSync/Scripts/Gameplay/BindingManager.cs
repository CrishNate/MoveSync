using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync.UI
{
    public struct BindKey
    {
        public KeyCode key;
        public BeatObjectData beatObjectData;
    }

    public class BindingManager : Singleton<BindingManager>
    {
        [HideInInspector] public UnityEventIntParam onStartListening = new UnityEventIntParam();
        [HideInInspector] public UnityEventIntParam onFinishListening = new UnityEventIntParam();
            
        private Dictionary<int, BindKey> _bind = new Dictionary<int, BindKey>();
        private static int _currentLayer = -1;
        private static bool _awaitButton;


        public void StartListening(int layer)
        {
            if (ObjectManager.instance.currentObjectModel == null) return;
            
            if (_currentLayer != -1)
                onFinishListening.Invoke(_currentLayer);

            _currentLayer = layer;
            _awaitButton = true;
            onStartListening.Invoke(_currentLayer);
        }

        public void AddKeyBind(int layer, BindKey bind)
        {
            if (_bind.ContainsKey(layer))
            {
                if (ObjectProperties.instance.selectedObject != null && ObjectProperties.instance.selectedObject.id == _bind[layer].beatObjectData.id)
                    ObjectProperties.instance.WipeSelections();
                
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
                if (ObjectProperties.instance.selectedObject != null && ObjectProperties.instance.selectedObject.id == _bind[layer].beatObjectData.id)
                    ObjectProperties.instance.WipeSelections();

                _bind.Remove(layer);
            }
            onFinishListening.Invoke(layer);
        }

        void Update()
        {
            if (!LevelSequencer.instance.audioSource.isPlaying) return;

            float time = LevelSequencer.instance.timeBPM;
            if (InputData.shouldSnap) 
                time = Mathf.Round(time);
            
            foreach (var bind in _bind)
            {
                if (Input.GetKeyDown(bind.Value.key))
                    LevelDataManager.instance.CopyBeatObject(bind.Value.beatObjectData, time, bind.Key);
            }
        }
        
        void OnGUI()
        {
            if (_awaitButton && Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    FinishListening();
                    return;
                };

                if (Event.current.keyCode == KeyCode.Delete)
                {
                    RemoveKeyBind(_currentLayer);
                    FinishListening();
                    return;
                };
                
                AddKeyBind(_currentLayer, new BindKey
                {
                    key = Event.current.keyCode,
                    beatObjectData = new BeatObjectData
                    {
                        objectTag = ObjectManager.instance.currentObjectModel.objectTag, 
                        id = SerializableGuid.NewGuid(), 
                        editorLayer = _currentLayer,
                        modelInputsData = ModelInput.CloneInputs(ObjectManager.instance.currentObjectModel.modelInput),
                    }
                });
                FinishListening();
            }
        }

        void FinishListening()
        {
            onFinishListening.Invoke(_currentLayer);
            _currentLayer = -1;
            _awaitButton = false;
        }


        public Dictionary<int, BindKey> bind => _bind;
    }
}
