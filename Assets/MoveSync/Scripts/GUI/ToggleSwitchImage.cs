using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSwitchImage : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _imageOn;
    [SerializeField] private Image _imageOff;

    void Start()
    {
        _toggle.onValueChanged.AddListener(OnSwitchToggle);
    }

    void OnSwitchToggle(bool toggle)
    {
        _imageOn.gameObject.SetActive(toggle);
        _imageOff.gameObject.SetActive(!toggle);
    }
}
