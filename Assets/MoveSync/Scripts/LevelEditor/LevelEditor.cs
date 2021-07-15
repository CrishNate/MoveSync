using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoveSync
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        public UnityEventBoolParam onSimulationMode;
        
        [SerializeField] private GameObject _vrPlayer;
        [SerializeField] private GameObject _nonVrPlayer;
        [SerializeField] private GameObject _centerGizmo;
        
        private Dictionary<PropertyName, Sprite> _objectIcons = new Dictionary<PropertyName, Sprite>();
        private static float _scrollSpeed;
        private bool _isSimulation;
        private bool _block;
        private bool _centerGizmoActive;

        public static bool isEditor;
        private static string beatObjectIconsPath = "MoveSync/BeatObjects/Icons/";

        public void SimulationMode(bool simulation)
        {
            _isSimulation = simulation;
            if (_isSimulation)
            {
                ObjectProperties.instance.WipeSelections();
            }
            
            onSimulationMode.Invoke(_isSimulation);
            
            _vrPlayer.SetActive(_isSimulation);
            _nonVrPlayer.SetActive(!_isSimulation);

            _nonVrPlayer.SetActive(!_isSimulation);
            _centerGizmo.SetActive(!_isSimulation && _centerGizmoActive);
        }

        public void ActivateCenterGizmo(bool active)
        {
            _centerGizmoActive = active;
            _centerGizmo.SetActive(!_isSimulation && _centerGizmoActive);
        }

        void LoadIcons()
        {
            // loading editors sprites
            foreach (var objectModel in ObjectManager.instance.objectModels)
            {
                string objectTag = LevelDataManager.PropertyNameToString(objectModel.Key);
                var sprite = Resources.Load<Sprite>(beatObjectIconsPath + objectTag);
                if (sprite)
                    _objectIcons.Add(objectModel.Key, sprite);
            }
        }
        
        void Start()
        {
            isEditor = true;
            
            ObjectManager.instance.onObjectsLoaded.AddListener(LoadIcons);
            LevelSequencer.instance.blockLevelFinishing = true;
            
            StartCoroutine(AutoSave());
            
            LoadIcons();
        }

        IEnumerator AutoSave()
        {
            while (true)
            {
                yield return new WaitForSeconds(60.0f);
                LevelDataManager.instance.SaveFile(LevelDataManager.songPath + LevelDataManager.autosaveFileName + "." + LevelDataManager.levelFileType);
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (LevelSequencer.instance.audioSource.isPlaying)
                    LevelSequencer.instance.Pause();
                else
                    LevelSequencer.instance.Play();
            }
            
            //TODO: Fix it for new VR system
            /*    
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    LevelSequencer.instance.SetSongTime(0.0f);
                }
    
                if (!_block)
                {
                    if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
                    {
                        LevelSequencer.instance.SetSongTime(LevelSequencer.instance.time + _scrollSpeed);
                        _block = true;
                    }
    
                    if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
                    {
                        LevelSequencer.instance.SetSongTime(LevelSequencer.instance.time - _scrollSpeed);
                        _block = true;
                    }
                }
                else if (!OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) &&
                         !OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
                    _block = false;
                    
            */
        }
        
        public Dictionary<PropertyName, Sprite> objectIcons => _objectIcons;
    }
}