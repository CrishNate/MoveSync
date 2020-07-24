using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [HideInInspector] public static LevelEditor instance = null;

    void Awake()
    {
        if (!enabled) return;
        
        if (instance == null)
        {
            instance = this;
        } 
        else if (instance == this)
        {
            Destroy(gameObject);
        }
        
        LevelSequencer.instance.audioSource.Stop();
    }

    public void SetSongTime(float time)
    {
        time *= LevelSequencer.instance.toTime;
        LevelSequencer.instance.audioSource.time = time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LevelSequencer.instance.audioSource.isPlaying)
                LevelSequencer.instance.audioSource.Pause();
            else
                LevelSequencer.instance.audioSource.Play();
        }
    }
}
