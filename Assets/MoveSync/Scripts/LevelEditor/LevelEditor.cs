using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.UI;
using UnityEngine;

namespace MoveSync
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        private static float _scrollSpeed;
        private bool _isSimulation;
        [SerializeField] private GameObject _vrPlayer;
        
        
        public void SimulationMode(bool simulation)
        {
            _isSimulation = simulation;
            if (_isSimulation)
            {
                ObjectProperties.instance.WipeSelections();
            }
            else
            {
            }
            
            _vrPlayer.SetActive(_isSimulation);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
            {
                if (LevelSequencer.instance.audioSource.isPlaying)
                    LevelSequencer.instance.Pause();
                else
                    LevelSequencer.instance.Play();
            }

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                LevelSequencer.instance.SetSongTime(0.0f);
            }
            
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight))
            {
                LevelSequencer.instance.SetSongTime(LevelSequencer.instance.time + _scrollSpeed);
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft))
            {
                LevelSequencer.instance.SetSongTime(LevelSequencer.instance.time - _scrollSpeed);
            }
        }
    }
}