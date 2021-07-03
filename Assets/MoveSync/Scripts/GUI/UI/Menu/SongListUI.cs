using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;
using UnityEngine.UI;

public class SongListUI : MonoBehaviour
{
    [SerializeField] private UnityEventSongInfoParam _songInfoParam;
    [SerializeField] private SongElementUI _songElementUI;
    [SerializeField] private ToggleGroup _toggleGroup;
    
    void Start()
    {
        List<SongInfo> songInfos = LevelDataManager.GetLevels();

        foreach (var songInfo in songInfos)
        {
            SongElementUI songInstance = Instantiate(_songElementUI, transform);
            songInstance.Init(songInfo);
            
            songInstance.toggle.onValueChanged.AddListener(x => OnSelectedSong(songInfo));
        }
        _songElementUI.gameObject.SetActive(false);
    }

    void OnSelectedSong(SongInfo songInfo)
    {
        songInfoParam.Invoke(songInfo);
    }

    public UnityEventSongInfoParam songInfoParam => _songInfoParam;
}
