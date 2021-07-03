using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoveSync.ModelData;
using MoveSync.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [Serializable]
    public struct SongInfo
    {
        public string songName;
        public string songAuthor;
        public string songFile;
        public string coverFile;
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
    public class BeatObjectData : ISerializationCallbackReceiver
    {
        public PropertyName objectTag;
        public SerializableGuid id;
        public float time = -1;
        // editor
        public int editorLayer;
        // custom data
        public ModelInput[] modelInputsData;
        private Dictionary<Type, ModelInput> modelInputs;

        public BeatObjectData()
        { }
        
        public void OnBeforeSerialize()
        { }

        public void OnAfterDeserialize()
        {
            FixModelInputs();
        }
        
        public BeatObjectData(PropertyName objectTag, SerializableGuid id, int editorLayer, ModelInput[] modelInputsData)
        {
            this.objectTag = objectTag;
            this.id = id;
            this.editorLayer = editorLayer;
            this.modelInputsData = modelInputsData;
            
            modelInputs = new Dictionary<Type, ModelInput>();
            foreach (var m in modelInputsData)
            {
                modelInputs.Add(m.GetType(), m);
            }
        }
        public BeatObjectData Clone()
        {
            BeatObjectData other = new BeatObjectData
            {
                objectTag = objectTag,
                id = SerializableGuid.NewGuid(),
                modelInputsData = ModelInput.CloneInputs(modelInputsData)
            };
            
            other.modelInputs = new Dictionary<Type, ModelInput>();
            for (int i = 0; i < other.modelInputsData.Length; i++)
            {
                ModelInput m = other.modelInputsData[i];
                other.modelInputs.Add(m.GetType(), m);
            }
            
            return other;
        }
        
        void FixModelInputs()
        {
            if (modelInputs != null) return;
            
            modelInputs = new Dictionary<Type, ModelInput>();
            for (int i = 0; i < modelInputsData.Length; i++)
            {
                ModelInput m = modelInputsData[i] = ModelInput.RecreateRealModel(modelInputsData[i]);
                modelInputs.Add(m.GetType(), m);
            }
        }
        
        public bool hasModel<T>() where T : ModelInput
        {
            return modelInputs.ContainsKey(typeof(T));
        }
        public bool hasModel(Type type)
        {
            return modelInputs.ContainsKey(type);
        }
        public T getModel<T>() where T : ModelInput
        {
            return (T)modelInputs[typeof(T)];
        }
        public bool tryGetModel<T>(out T modelInput) where T : ModelInput
        {
            bool result = modelInputs.TryGetValue(typeof(T), out var tempModelInput);
            modelInput = (T)tempModelInput;
            return result;
        }
        public bool tryGetModel(Type type, out ModelInput modelInput)
        {
            return modelInputs.TryGetValue(type, out modelInput);
        }
        
        public float spawnTime
        {
            get
            {
                if (tryGetModel<APPEAR>(out var appear)) return time - appear.value;

                return time;
            }
        }
        public float durationTime
        {
            get
            {
                if (tryGetModel<DURATION>(out var durationModel)) return time + durationModel.value;

                return time;
            }
        }
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
        public static string songPath => resourcePath + "Songs/";
        public static string coverPath => songPath + "Cover/";
        public static string levelFileType = "mslevel";
        public static string autosaveFileName = "autosave";

        public UnityEventBeatObjectData onNewObject = new UnityEventBeatObjectData();
        public UnityEventIntParam onUpdateObject = new UnityEventIntParam();
        public UnityEventIntParam onRemoveObject = new UnityEventIntParam();
        public UnityEvent onUpdateObjects = new UnityEvent();
        public UnityEvent onLoadedSong = new UnityEvent();
        [HideInInspector] public LevelInfo levelInfo = new LevelInfo();

        // history
        private List<LevelInfo> _history = new List<LevelInfo>();
        private int _historyMarker;


        public static string PropertyNameToString(PropertyName objectTag)
        {
            string str = objectTag.ToString();
            str = str.Substring(0, str.IndexOf(':'));
            return str;
        }
        
        /*
         * Object controll
         */
        
        public BeatObjectData NewBeatObject(ObjectModel objectModel, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = new BeatObjectData(objectModel.objectTag, SerializableGuid.NewGuid(), layer,
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
        
        /*
         * Level File save
         */
        public void ExploreLevelFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", songPath, levelFileType);
            if (path.Length != 0) LoadFile(path);
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
        
        /*
         * Level Data
         */
        public void NewLevel()
        {
            levelInfo.beatObjectDatas.Clear();
        }
        
        public void LoadFile(string filePath)
        {
            string levelJson = File.ReadAllText(filePath);
            LoadInfo(JsonUtility.FromJson<LevelInfo>(levelJson));
        }

        public void SaveFile(string filePath)
        {
            levelInfo.songInfo.songParams = LevelSequencer.instance.songParams;
            levelInfo.levelEditorInfo.bindKeys = BindingManager.instance.bind.Values.ToList();
            
            string levelJson = JsonUtility.ToJson(levelInfo);
            File.WriteAllText(filePath, levelJson);
        }

        public void LoadSong(string filePath)
        {
            levelInfo.songInfo.songFile = Path.GetFileNameWithoutExtension(filePath.Replace(resourcePath, ""));
            LevelSequencer.instance.songParams = levelInfo.songInfo.songParams;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(levelInfo.songInfo.songFile);

            onLoadedSong.Invoke();
        }

        public static List<SongInfo> GetLevels()
        {
            DirectoryInfo directorySongInfo = new DirectoryInfo(songPath);
            FileInfo[] fileInfo = directorySongInfo.GetFiles("*." + levelFileType);
            List<SongInfo> songInfos = new List<SongInfo>();

            foreach (FileInfo file in fileInfo)
            {
                if (file.Name == autosaveFileName + "." + levelFileType)
                    continue;

                SongInfo songInfo = JsonUtility.FromJson<LevelInfoOnlySongInfo>(File.ReadAllText(file.FullName)).songInfo;
                songInfo.filePath = songPath + file.Name;
                songInfos.Add(songInfo);
            }

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
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(levelInfo.songInfo.songFile);
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