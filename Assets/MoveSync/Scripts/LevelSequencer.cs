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
    public GameObject hexagon;

    private int _marker = 0;

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

        _marker = ++_marker % 2;
        if (timeBPM >= _timeMarker)
        {
            GameObject spawn = (_marker == 0) ? tetrahedron : hexagon;
            BeatObject beatObject = Instantiate(spawn, transform.position, Quaternion.identity).GetComponent<BeatObject>();
            
            TransformData finishTransform = new TransformData();
            finishTransform.position = transform.position;
            finishTransform.position += Random.insideUnitSphere * 10.0f;
            finishTransform.position += Vector3.up * 10.0f;
            
            beatObject.Initialize(_timeMarker + 8, finishTransform);

            _timeMarker += 2;
        }
    }

    public AudioSource audioSource => _audioSource;
    public float timeBPM => time * toBPM;
    public float bpm => _songInfo.bpm;
    public float toBPM => _songInfo.bpm / 60.0f;
    public float toTime => 60.0f / _songInfo.bpm;
    public float time => _time - _songInfo.offset;
}
