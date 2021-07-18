using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.UI;

public class SongInfoUI : MonoBehaviour
{
    [SerializeField] private Image _coverImage;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _authorText;
    [SerializeField] private Text _bpmText;
    [SerializeField] private Text _songLength;
    [SerializeField] private Button _buttonStart;
    [SerializeField] private Animator _animator;

    [SerializeField] private UnityEventSongInfoParam _onButtonStart;

    private SongInfo _selectedSongInfo;
    private Coroutine _songPreviewCoroutine;
    
    private static readonly int Appear = Animator.StringToHash("Appear");

    
    public void Init(SongInfo songInfo)
    {
        _selectedSongInfo = songInfo;

        gameObject.SetActive(true);
        _animator.SetTrigger(Appear);
        
        string filePath = LevelDataManager.coverPath + songInfo.coverFile;
        filePath = filePath.Replace(LevelDataManager.resourcePath, "");
        Sprite coverSprite = Resources.Load<Sprite>(filePath);
        
        if (coverSprite)
            _coverImage.sprite = coverSprite;

        _nameText.text = songInfo.songName;
        _authorText.text = songInfo.songAuthor;
        _bpmText.text = songInfo.songParams.bpm.ToString("#.00");

        var audioClip = Resources.Load<AudioClip>(LevelDataManager.songPath + songInfo.songFile);
        if (audioClip)
            _songLength.text = $"{audioClip.length / 60:#0}:{audioClip.length % 60:00}";

        // Preview song
        LevelSequencer.instance.blockLevelFinishing = true;
        LevelSequencer.instance.audioSource.clip = audioClip;
        LevelSequencer.instance.songParams = songInfo.songParams;

        if (_songPreviewCoroutine != null)
            StopCoroutine(_songPreviewCoroutine);
        
        _songPreviewCoroutine = StartCoroutine(SongPreviewTime());
    }

    IEnumerator SongPreviewTime()
    {
        LevelSequencer.instance.StartSong();
        LevelSequencer.instance.SetSongTime(_selectedSongInfo.songPreviewStart);
        
        yield return new WaitForSeconds(10.0f);
        
        LevelSequencer.instance.SongFadeOut();
    }

    private void Start()
    {
        _buttonStart.onClick.AddListener(() =>
        {
            _onButtonStart.Invoke(_selectedSongInfo);
            StopCoroutine(_songPreviewCoroutine);
            LevelSequencer.instance.audioSource.Stop();
            LevelSequencer.instance.blockLevelFinishing = false;
        });
    }
}
