using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;

public class HeartBeatSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _heartbeatStart;
    [SerializeField] private AudioClip _heartbeatEnd;
    
    private void Start()
    {
        PlayerBehaviour.instance.onHit.AddListener(OnHit);
    }

    private void OnHit(int health)
    {
        if (health == 1)
        {
            StartCoroutine(HeartBeat());
        }
    }


    IEnumerator HeartBeat()
    {
        while (PlayerBehaviour.instance.health == 1)
        {
            yield return new WaitForSeconds((2 - LevelSequencer.instance.timeBPM % 2.0f) * LevelSequencer.instance.toTime - 0.05f);
            _audioSource.PlayOneShot(_heartbeatStart);
            yield return new WaitForSeconds(LevelSequencer.instance.toTime);
            _audioSource.PlayOneShot(_heartbeatEnd);
        }
    }
}
