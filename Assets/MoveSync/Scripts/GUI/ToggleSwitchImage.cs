using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSwitchImage : MonoBehaviour
{
    [SerializeField] private Image _imageOn;
    [SerializeField] private Image _imageOff;

    public void OnSwitchToggle(bool toggle)
    {
        _imageOn.gameObject.SetActive(toggle);
        _imageOff.gameObject.SetActive(!toggle);
    }
}
