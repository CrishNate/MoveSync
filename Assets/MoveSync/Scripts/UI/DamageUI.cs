using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Image _image;
    [SerializeField] private PlayerBehaviour _playerBehaviour;

    public void OnHit()
    {
        if (_playerBehaviour.isMaxHealth)
        {
            _image.enabled = false;
        }
        else
        {
            _image.enabled = true;

            float dHealth = 1.0f - (float)_playerBehaviour.health / _playerBehaviour.maxHealth;
            int index = (int)(dHealth * (_sprites.Length - 1));
            _image.sprite = _sprites[index];
        }
    }
}
