using System.Collections;
using System.Collections.Generic;
using System.IO;
using MoveSync;
using UnityEngine;
using UnityEngine.UI;

public class SongElementUI : MonoBehaviour
{
    [SerializeField] private Image _cover;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _authorText;
    [SerializeField] private Toggle _toggle;

    public void Init(SongInfo songInfo)
    {
        string filePath = LevelDataManager.coverPath + songInfo.coverFile;
        filePath = filePath.Replace(LevelDataManager.resourcePath, "");
        
        Sprite coverSprite = Resources.Load<Sprite>(filePath);
        if (coverSprite)
            _cover.sprite = coverSprite;

        _nameText.text = songInfo.songName;
        _authorText.text = songInfo.songAuthor;
    }

    public Toggle toggle => _toggle;
}
