using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;

public class SongListUI : MonoBehaviour
{
    [SerializeField] private SongElementUI _songElementUI;
    
    void Start()
    {
        List<SongInfo> songInfos = LevelDataManager.instance.GetLevels();

        foreach (var songInfo in songInfos)
        {
            SongElementUI songInstance = Instantiate(_songElementUI, transform);
            
            Texture cover = Resources.Load<Texture>(LevelDataManager.coverPath + songInfo.coverFile);
            if (cover)
                songInstance.cover.image = cover;

            songInstance.songName.text = songInfo.songName;
            songInstance.songAuthor.text = songInfo.songAuthor;
        }
        _songElementUI.gameObject.SetActive(false);
    }
}
