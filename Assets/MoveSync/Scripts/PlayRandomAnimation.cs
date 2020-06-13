using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animation))]
public class PlayRandomAnimation : MonoBehaviour
{
    [SerializeField] private bool _playOnAwake;
    private Animation _animation;
    
    void Start()
    {
        _animation = GetComponent<Animation>();

        if (_playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        _animation.Stop();
        _animation.Play();
    }
}
