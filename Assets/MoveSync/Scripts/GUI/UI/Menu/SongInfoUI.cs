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

        var audioClip = Resources.Load<AudioClip>(songInfo.songFile);
        if (audioClip)
            _songLength.text = $"{audioClip.length / 60:#0}:{audioClip.length % 60:00}";
    }

    private void Start()
    {
        _buttonStart.onClick.AddListener(() => _onButtonStart.Invoke(_selectedSongInfo));
    }
}
