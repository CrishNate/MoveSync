using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MoveSync.ModelData;
using MoveSync.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoveSync
{
    [Serializable]
    public struct SongsList
    {
        public List<string> songs;
    }
    
    [Serializable]
    public struct SongInfo
    {
        public string songName;
        public string songAuthor;
        public string songFile;
        public string coverFile;
        public float songPreviewStart;
        public SongParams songParams;
        [NonSerialized] public string filePath;
    }
    
    [Serializable]
    public struct SongParams
    {
        public float bpm;
        public float offset;
    }
    
    [Serializable]
    public class BeatAnimationData
    {
        public float time;
        public string animation;
    }

    [Serializable]
    public class LevelEditorInfo
    {
        public List<BindKey> bindKeys;
    }
    
    public class LevelInfo
    {
        public SongInfo songInfo;
        public LevelEditorInfo levelEditorInfo;

        public List<BeatObjectData> beatObjectDatas;
    }
    
    class LevelInfoOnlySongInfo
    {
        public SongInfo songInfo;
    }
    
    public class LevelDataManager : Singleton<LevelDataManager>
    {
        public static string resourcePath => Application.dataPath + "/Resources/";
        public static string syncRoot => "MoveSync/";
        public static string songPath => syncRoot + "Songs/";
        public static string coverPath => songPath + "Cover/";
        public static string levelFileType = "json";
        public static string autosaveFileName = "autosave";
        public static string allSongsFilename = "songs";

        public UnityEventBeatObjectData onNewObject = new UnityEventBeatObjectData();
        public UnityEventIntParam onUpdateObject = new UnityEventIntParam();
        public UnityEventIntParam onRemoveObject = new UnityEventIntParam();
        public UnityEvent onUpdateObjects = new UnityEvent();
        public UnityEvent onLoadedSong = new UnityEvent();
        [HideInInspector] public LevelInfo levelInfo = new LevelInfo();

        // history
        private List<LevelInfo> _history = new List<LevelInfo>();
        private int _historyMarker;

        /*
         * Object controll
         */
        
        public BeatObjectData NewBeatObject(ObjectModel objectModel, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = new BeatObjectData(objectModel.ObjectTag, SerializableGuid.NewGuid(), layer,
                ModelInput.CloneInputs(objectModel.modelInput))
            {
                time = time
            };

            if (history) 
                BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
            SortBeatObjects();

            return data;
        }
        
        public void CopyBeatObject(BeatObjectData beatObjectData, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = beatObjectData.Clone();
            data.time = time;
            data.editorLayer = layer;
            
            if (history) 
                BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
        }

        public bool RemoveBeatObject(BeatObjectData beatObjectData, bool history = true)
        {
            if (history) 
                BackupInfo();
            
            onRemoveObject.Invoke(beatObjectData.id);
            return levelInfo.beatObjectDatas.Remove(beatObjectData);
        }

        public void ClearLayer(int layer)
        {
            levelInfo.beatObjectDatas.RemoveAll(data => data.editorLayer == layer);
            onUpdateObjects.Invoke();
        }

        public void SortBeatObjects()
        {
            levelInfo.beatObjectDatas.Sort((p1,p2)=>p1.spawnTime.CompareTo(p2.spawnTime));
        }

        public void UpdateBeatObject(int id)
        {
            onUpdateObject.Invoke(id);
        }
        
#if UNITY_EDITOR
        /*
         * Level File save
         */
        public void ExploreLevelFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", songPath, levelFileType);
            if (path.Length != 0) LoadFile(songPath + Path.GetFileNameWithoutExtension(path));
        }
        
        public void ExploreSaveLevelFile()
        {
            string path = EditorUtility.SaveFilePanel("Save Level", songPath, "MoveSyncLevel", levelFileType);
            if (path.Length != 0) SaveFile(path);
        }
                
        public void ExploreSongFile()
        {
            string path = EditorUtility.OpenFilePanel("Song File", resourcePath, "mp3,wave");
            if (path.Length != 0) LoadSong(path);
        }
#endif
        /*
         * Level Data
         */
        public void NewLevel()
        {
            levelInfo.beatObjectDatas.Clear();
        }
        
        public void LoadFile(string fileName)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            
            //new ModelInputJsonConverter(typeof(LevelInfo))
            LoadInfo(JsonConvert.DeserializeObject<LevelInfo>(textAsset.text));
        }

        public void SaveFile(string filePath)
        {
            levelInfo.songInfo.songParams = LevelSequencer.instance.songParams;
            levelInfo.levelEditorInfo.bindKeys = BindingManager.instance.bind.Values.ToList();
            
            string levelJson = JsonConvert.SerializeObject(levelInfo);
            File.WriteAllText(filePath, levelJson);
        }

        public void LoadSong(string filePath)
        {
            //levelInfo.songInfo.songFile = Path.GetFileNameWithoutExtension(filePath.Replace(songPath, ""));
            LevelSequencer.instance.songParams = levelInfo.songInfo.songParams;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(songPath + levelInfo.songInfo.songFile);

            onLoadedSong.Invoke();
        }

        public static List<SongInfo> GetLevels()
        {
            TextAsset allSongsFile = Resources.Load<TextAsset>(syncRoot + allSongsFilename);
            List<string> songsList = JsonConvert.DeserializeObject<SongsList>(allSongsFile.text).songs;
            
            List<SongInfo> songInfos = new List<SongInfo>();

            foreach (string songName in songsList)
            {
                TextAsset songFile = Resources.Load<TextAsset>(songPath + songName);
                SongInfo songInfo = JsonConvert.DeserializeObject<LevelInfoOnlySongInfo>(songFile.text).songInfo;
                songInfo.filePath = songPath + songName;
                songInfos.Add(songInfo);
                
                Resources.UnloadAsset(songFile);
            }

            Resources.UnloadAsset(allSongsFile);
            return songInfos;
        }
        
        /*
         * History
         */
        public void BackupInfo()
        {
            int diff = (_history.Count - _historyMarker) - 1;
            if (diff > 0) 
                _history.RemoveRange(_historyMarker, diff);

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
            this.levelInfo = levelInfo;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(songPath + levelInfo.songInfo.songFile);
            LevelSequencer.instance.songParams = levelInfo.songInfo.songParams;

            if (levelInfo.levelEditorInfo == null)
                levelInfo.levelEditorInfo = new LevelEditorInfo();
            
            onLoadedSong.Invoke();
        }

        void Start()
        {
            levelInfo.beatObjectDatas = new List<BeatObjectData>();
            levelInfo.levelEditorInfo = new LevelEditorInfo();
        }
    }
}