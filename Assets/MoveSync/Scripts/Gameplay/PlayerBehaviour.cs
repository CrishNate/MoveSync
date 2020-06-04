using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent _onDeath;
    [SerializeField] private int _health;
    [SerializeField] private float _invincibilityTime;

    private float _invincibilityTimeStamp;

    public void OnHit(GameObject instigator)
    {
        if (_invincibilityTimeStamp > Time.time) return;
        
        _health--;
        
        _invincibilityTimeStamp = Time.time + _invincibilityTime;

        if (_health == 0)
        {
            Death();
        }
    }

    void Death()
    {
        _onDeath.Invoke();
    }
}
