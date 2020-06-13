using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Image _image;
    [SerializeField] private UnityEvent _onHit;

    public void OnHit()
    {
        if (PlayerBehaviour.instance.isMaxHealth)
        {
            _image.enabled = false;
        }
        else
        {
            _image.enabled = true;

            float dHealth = 1.0f - (float)PlayerBehaviour.instance.health / PlayerBehaviour.instance.maxHealth;
            int index = (int)(dHealth * (_sprites.Length - 1));
            _image.sprite = _sprites[index];

            _onHit.Invoke();
        }
    }

    public void OnDeath()
    {
        _image.enabled = false;
    }
}
