using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.UI;
using UnityEngine;

namespace MoveSync
{
    public class LevelEditor : Singleton<LevelEditor>
    {
        [SerializeField] private GameObject _vrPlayer;
        [SerializeField] private GameObject _nonVrPlayer;
        private static float _scrollSpeed;
        private bool _isSimulation;
        private bool _block;


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
            _nonVrPlayer.SetActive(!_isSimulation);
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
        }
    }
}