using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerSound : Singleton<LowerSound>
{
    [SerializeField] private AudioLowPassFilter _audioLowPassFilter;

    private void Start()
    {
        _audioLowPassFilter.enabled = false;
    }

    public void BeginLower()
    {
        StartCoroutine(Lower());
    }

    public void BeginHigher()
    {
        StartCoroutine(Higher());
    }

    IEnumerator Lower()
    {
        _audioLowPassFilter.enabled = true;
        while (_audioLowPassFilter.cutoffFrequency > 3e3)
        {
            _audioLowPassFilter.cutoffFrequency -= (22e3f - 2e3f) * Time.deltaTime * 5f;
            yield return null;
        }
    }
        
    IEnumerator Higher()
    {
        while (_audioLowPassFilter.cutoffFrequency < 22e3)
        {
            _audioLowPassFilter.cutoffFrequency += (22e3f - 2e3f) * Time.deltaTime * 2f;
            yield return null;
        }
        _audioLowPassFilter.enabled = false;
    }
}
