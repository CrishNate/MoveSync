using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
struct SongInfo
{
    public float bpm;
    public float offset;
}

[RequireComponent(typeof(AudioSource))]
public class LevelSequencer : MonoBehaviour
{
    [HideInInspector]
    public static LevelSequencer instance = null;

    // song settngs
    [SerializeField]
    private SongInfo _songInfo;

    private float _time = 0;
    private AudioSource _audioSource;
    public GameObject tetrahedron;

    // debug
    private float _timeMarker = 0;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (instance == null)
        {
            instance = this;
        } 
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        _time = _audioSource.time;
        
        if (timeBPM >= _timeMarker)
        {
            _timeMarker = timeBPM + 1 - (timeBPM - _timeMarker);
            
            BeatObject beatObject = Instantiate(tetrahedron, transform.position, Quaternion.identity).GetComponent<BeatObject>();
            
            TransformData finishTransform = new TransformData();
            finishTransform.position = transform.position;
            finishTransform.position += Random.insideUnitSphere * 10.0f;
            finishTransform.position += Vector3.up * 10.0f;
            finishTransform.rotation = Quaternion.LookRotation(Camera.main.transform.position - finishTransform.position);
            
            beatObject.Initialize(_timeMarker + 4, finishTransform);
        }
    }

    public AudioSource audioSource => _audioSource;
    public float timeBPM => _time * toBPM - _songInfo.offset;
    public float bpm => _songInfo.bpm;
    public float toBPM => _songInfo.bpm / 60.0f;
    public float toTime => 60.0f / _songInfo.bpm;
    public float time => _time;
}
