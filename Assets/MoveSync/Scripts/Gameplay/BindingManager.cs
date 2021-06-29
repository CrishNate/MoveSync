using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync.UI
{
    [Serializable]
    public struct BindKey
    {
        public KeyCode key;
        public BeatObjectData beatObjectData;
    }

    public class BindingManager : Singleton<BindingManager>
    {
        [HideInInspector] public UnityEventIntParam onStartListening = new UnityEventIntParam();
        [HideInInspector] public UnityEventIntParam onStartAwaitConfirm = new UnityEventIntParam();
        [HideInInspector] public UnityEventIntParam onFinishAwaitConfirm = new UnityEventIntParam();
        [HideInInspector] public UnityEventIntParam onFinishListening = new UnityEventIntParam();
        [HideInInspector] public UnityEventIntParam onClearBind = new UnityEventIntParam();
        [HideInInspector] public UnityEvent onUpdate = new UnityEvent();
            
        private Dictionary<int, BindKey> _bind = new Dictionary<int, BindKey>();
        private static int _currentLayer = -1;
        private static int _awaitConfirmOnLayer = -1;
        private static bool _awaitButton;


        public void StartListening(int layer)
        {
            if (_currentLayer != -1)
                onFinishListening.Invoke(_currentLayer);

            _currentLayer = layer;
            _awaitButton = true;
            onStartListening.Invoke(_currentLayer);
        }

        public void ClearAllBinds()
        {
            _bind.Clear();
            onUpdate.Invoke();
        }
        
        public void KeyBind(int layer, KeyCode key)
        {
            if (_bind.ContainsKey(layer))
            {
                BindKey bindKey = _bind[layer];
                bindKey.key = key;
                _bind[layer] = bindKey;
            }
            else
            {
                _bind.Add(layer, new BindKey {key = key, beatObjectData = null});
            }
        }
        
        public void ObjectBind(int layer, BeatObjectData beatObjectData)
        {
            if (_bind.ContainsKey(layer))
            {
                if (ObjectProperties.instance.selectedObject != null && ObjectProperties.instance.selectedObject.id == _bind[layer].beatObjectData.id)
                    ObjectProperties.instance.WipeSelections();
                
                BindKey bindKey = _bind[layer];
                bindKey.beatObjectData = beatObjectData;
                _bind[layer] = bindKey;
            }
            else
            {
                _bind.Add(layer, new BindKey {key = KeyCode.None, beatObjectData = beatObjectData});
            }
        }

        public void ObjectBind(int layer)
        {
            if (ObjectManager.instance.currentObjectModel == null) return;

            if (_awaitConfirmOnLayer != layer)
            {
                if (_awaitConfirmOnLayer != -1) 
                    onFinishAwaitConfirm.Invoke(_awaitConfirmOnLayer);
                _awaitConfirmOnLayer = layer;
                
                onStartAwaitConfirm.Invoke(layer);
                return;
            };

            if (!_bind.ContainsKey(layer))
            {
                StartListening(layer);
            }

            var newBeatObject = new BeatObjectData(ObjectManager.instance.currentObjectModel.objectTag,
                SerializableGuid.NewGuid(),
                _awaitConfirmOnLayer,
                ModelInput.CloneInputs(ObjectManager.instance.currentObjectModel.modelInput));
            
            ObjectBind(layer, newBeatObject);
            
            _awaitConfirmOnLayer = -1;
            onFinishAwaitConfirm.Invoke(layer);
        }

        public void RemoveKeyBind(int layer)
        {
            if (_bind.ContainsKey(layer))
            {
                if (ObjectProperties.instance.selectedObject != null && ObjectProperties.instance.selectedObject.id == _bind[layer].beatObjectData.id)
                    ObjectProperties.instance.WipeSelections();

                _bind.Remove(layer);
            }
            onClearBind.Invoke(layer);
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
                {
                    if (bind.Value.beatObjectData == null) continue;
                    
                    LevelDataManager.instance.CopyBeatObject(bind.Value.beatObjectData, time, bind.Key);
                    LevelDataManager.instance.SortBeatObjects();
                }
            }
        }
        
        void OnGUI()
        {
            if (Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                if (InputData.shouldClose && _awaitConfirmOnLayer != -1)
                {
                    onFinishAwaitConfirm.Invoke(_awaitConfirmOnLayer);
                    _awaitConfirmOnLayer = -1;
                }
                
                if (_awaitButton)
                {
                    if (InputData.shouldClose)
                    {
                        FinishListening();
                        return;
                    }

                    if (InputData.shouldDelete)
                    {
                        RemoveKeyBind(_currentLayer);
                        FinishListening();
                        return;
                    }

                    KeyBind(_currentLayer, Event.current.keyCode);
                    FinishListening();
                }
            }
        }

        void FinishListening()
        {
            onFinishListening.Invoke(_currentLayer);
            _currentLayer = -1;
            _awaitButton = false;
        }

        void Start()
        {
            LevelDataManager.instance.onLoadedSong.AddListener(OnSongLoaded);
        }

        void OnSongLoaded()
        {
            ClearAllBinds();
            foreach (BindKey bindKey in LevelDataManager.instance.levelInfo.levelEditorInfo.bindKeys)
            {
                ObjectBind(bindKey.beatObjectData.editorLayer, bindKey.beatObjectData);
                KeyBind(bindKey.beatObjectData.editorLayer, bindKey.key);
                onUpdate.Invoke();
            }
        }


        public Dictionary<int, BindKey> bind => _bind;
    }
}
