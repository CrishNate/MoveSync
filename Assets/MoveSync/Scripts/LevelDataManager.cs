using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ExTransformData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public int parentId;
}

public struct BeatSequence
{
    public Dictionary<float, string> animation;
    public Dictionary<float, Vector3> position;
    public Dictionary<float, Quaternion> rotation;
    public Dictionary<float, Vector3> scale;
}

public struct BeatElement
{
    public string elementTag;
    public int id;
    public float time;
    public ExTransformData transformData;
    public BeatSequence beatSequence;
    
    // editor
    public int editorPositionY;
}

public struct LevelInfo
{
    public string songName;
    public string songFile;

    public List<BeatElement> beatElements;
}

public class LevelDataManager : MonoBehaviour
{
    public string levelName;
    
    [HideInInspector] public LevelInfo levelInfo;
    public static LevelDataManager instance = null;

    private List<LevelInfo> _history;
    private int _historyMarker;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadData(string fileName)
    {
        string levelJson = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + fileName);
        LoadInfo(JsonUtility.FromJson<LevelInfo>(levelJson));
    }

    public void SaveInfo()
    {
        string levelJson = JsonUtility.ToJson(levelInfo);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + levelName, levelJson);
    }

    public void BackupInfo()
    {
        int diff = (_history.Count - _historyMarker) - 1;
        if (diff > 0) _history.RemoveRange(_historyMarker, diff);

        _history.Add(levelInfo);
        _historyMarker = _history.Count - 1;
    }

    public void UndoInfo()
    {
        if (_historyMarker == 0) return;
        LoadInfo(_history[--_historyMarker]);
    }

    public void RedoInfo()
    {
        if (_historyMarker == _history.Count - 1) return;
        LoadInfo(_history[++_historyMarker]);
    }
    
    void LoadInfo(LevelInfo levelInfo)
    {
        
    }
}
