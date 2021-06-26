using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.UI;

public class InputSlider : MonoBehaviour
{
    public UnityEventFloatParam onValueChanged;
    
    [SerializeField] private Slider _slider;
    [SerializeField] private InputField _input;
    
    void UpdateValueBySlider(float value)
    {
        _input.SetTextWithoutNotify(value.ToString());
        onValueChanged.Invoke(value);
    }

    void UpdateValueByInputField(String text)
    {
        _slider.SetValueWithoutNotify(float.Parse(text));
        onValueChanged.Invoke(float.Parse(text));
    }
    
    void Start()
    {
        LevelDataManager.instance.onLoadedSong.AddListener(() => UpdateValueBySlider(LevelSequencer.instance.songOffset));
        
        _slider.onValueChanged.AddListener(UpdateValueBySlider);
        _slider.onValueChanged.Invoke(_slider.value);
        
        _input.onValueChanged.AddListener(UpdateValueByInputField);
    }
}
