using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Slider _slider;

    
    void UpdateValue(float volume)
    {
        _text.text = Mathf.RoundToInt(volume * 100.0f) + "%";
    }
    
    void Start()
    {
        _slider.onValueChanged.AddListener(UpdateValue);
        _slider.onValueChanged.Invoke(_slider.value);
    }
}
