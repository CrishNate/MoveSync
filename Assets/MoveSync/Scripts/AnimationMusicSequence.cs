using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMusicSequence : MonoBehaviour
{
    private float _timeBPM;
    
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.speed = LevelSequencer.instance.toBPM;
    }
    
    void Update()
    {
        Animator animator = GetComponent<Animator>();
        float difference = LevelSequencer.instance.timeBPM - _timeBPM;

        if (difference > 0.0f)
        {
            _timeBPM += 8.0f;
            
            animator.Play("Base Layer.Swim", 0, difference * LevelSequencer.instance.toTime);
        }
    }
}
