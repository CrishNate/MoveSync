using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.UI;

public class SoundSpeedSlider : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Slider _slider;

    
    void UpdateValue(float speed)
    {
        if (speed < 1.0f)
        {
            _slider.value = 1.0f;
            speed = 1.0f;
        }

        _text.text = Mathf.RoundToInt(speed * 25) + "%";
        speed /= 4.0f;

        LevelSequencer.instance.audioSource.pitch = speed;
        //LevelSequencer.instance.audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / speed);
        
    }
    
    void Start()
    {
        _slider.onValueChanged.AddListener(UpdateValue);
        _slider.onValueChanged.Invoke(_slider.value);
    }
}
