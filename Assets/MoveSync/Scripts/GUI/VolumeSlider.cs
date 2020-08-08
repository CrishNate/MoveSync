using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        _slider.onValueChanged.AddListener(UpdateVolume);
        UpdateVolume(_slider.value);
    }

    public void UpdateVolume(float volume)
    {
        _text.text = Mathf.RoundToInt(volume * 100.0f) + "%";
    }
}
