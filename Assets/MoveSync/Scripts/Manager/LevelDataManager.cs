using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MoveSync.ModelData;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    [Serializable]
    public struct SongInfo
    {
        public float bpm;
        public float offset;
    }
    
    [Serializable]
    public class ExTransformData
    {
        public ExTransformData Clone()
        {
            ExTransformData other = (ExTransformData) MemberwiseClone();
            other.position = new Vector3(position.x, position.y, position.z);
            other.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
            return other;
        }

        public Vector3 position;
        public Quaternion rotation;
    }
    
    [Serializable]
    public class BeatAnimationData
    {
        public float time;
        public string animation;
    }

    [Serializable]
    public class BeatObjectData
    {
        public BeatObjectData Clone()
        {
            BeatObjectData other = (BeatObjectData) MemberwiseClone();
            other.id = SerializableGuid.NewGuid();
            other.modelInputsData = ModelInput.CloneInputs(modelInputsData);

            return other;
        }

        public PropertyName objectTag;
        public SerializableGuid id;

        public float time;

        // editor
        public int editorLayer;

        // custom data
        public ModelInput[] modelInputsData;
        private Dictionary<PropertyName, ModelInput> modelInputs;

        
        public bool hasModel(PropertyName type)
        {
            if (modelInputs == null)
            {
                modelInputs = new Dictionary<PropertyName, ModelInput>();
                for (int i = 0; i < modelInputsData.Length; i++)
                {
                    ModelInput m = modelInputsData[i] = ModelInput.RecreateRealModel(modelInputsData[i]);
                    modelInputs.Add(m.type, m);
                }
            }

            return modelInputs.ContainsKey(type);
        }
        public T getModel<T>(PropertyName type) where T : ModelInput
        {
            if (modelInputs == null)
            {
                modelInputs = new Dictionary<PropertyName, ModelInput>();
                for (int i = 0; i < modelInputsData.Length; i++)
                {
                    ModelInput m = modelInputsData[i] = ModelInput.RecreateRealModel(modelInputsData[i]);
                    modelInputs.Add(m.type, m);
                }
            }

            return (T)modelInputs[type];
        }


        public float spawnTime
        {
            get
            {
                if (hasModel(APPEAR.TYPE)) return time - getModel<APPEAR>(APPEAR.TYPE).value;

                return time;
            }
        }
    }

    public class LevelEditorInfo
    {
        public int layersCount;
    }
    
    public class LevelInfo
    {
        public string songName;
        public string songFile;
        public SongInfo songInfo;
        [NonSerialized] public LevelEditorInfo levelEditorInfo;

        public List<BeatObjectData> beatObjectDatas;
    }

    public class LevelDataManager : Singleton<LevelDataManager>
    {
        public static string resourcePath => Application.dataPath + "/Resources/";
        public static string levelFileType = "mslevel";

        // event call when even NEW object is created
        public UnityEventBeatObjectData onNewObjectCreated = new UnityEventBeatObjectData();
        // event call when ever new object is created or loaded from save
        public UnityEventBeatObjectData onNewObject = new UnityEventBeatObjectData();
        public UnityEventBeatObjectData onUpdateObject = new UnityEventBeatObjectData();
        public UnityEventBeatObjectData onRemoveObject = new UnityEventBeatObjectData();
        public UnityEvent onUpdateObjects = new UnityEvent();
        public UnityEvent onLoadedSong = new UnityEvent();
        [HideInInspector] public LevelInfo levelInfo = new LevelInfo();

        private List<LevelInfo> _history = new List<LevelInfo>();
        private int _historyMarker;

        
        /*
         * Object controll
         */
        public BeatObjectData NewBeatObjectAtMarker(PropertyName objectTag, int layer = 0)
        {
            float time = LevelSequencer.instance.timeBPM;
            if (InputData.shouldSnap) time = Mathf.Round(time);
            
            BeatObjectData data = NewBeatObject(objectTag, time, layer);
            return data;
        }
        
        public BeatObjectData NewBeatObject(PropertyName objectTag, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = new BeatObjectData
            {
                objectTag = objectTag, 
                id = SerializableGuid.NewGuid(), 
                time = time,
                editorLayer = layer,
                modelInputsData = ModelInput.CloneInputs(ObjectManager.instance.objectModels[objectTag].modelInput),
            };

            if (history) BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
            onNewObjectCreated.Invoke(data);

            SortBeatObjects();
            return data;
        }
        
        public void CopyBeatObject(BeatObjectData beatObjectData, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = beatObjectData.Clone();
            data.time = time;
            data.editorLayer = layer;
            if (history) BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
        }

        public bool RemoveBeatObject(BeatObjectData beatObjectData, bool history = true)
        {
            if (history) BackupInfo();
            onRemoveObject.Invoke(beatObjectData);
            SerializableGuid.RemoveId(beatObjectData.id);
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

        public void UpdateBeatObject(BeatObjectData beatObjectData)
        {
            onUpdateObject.Invoke(beatObjectData);
        }
        
        /*
         * Level File save
         */
        public void ExploreLevelFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", Application.persistentDataPath, levelFileType);
            if (path.Length != 0) LoadFile(path);
        }
        
        public void ExploreSaveLevelFile()
        {
            string path = EditorUtility.SaveFilePanel("Save Level", Application.persistentDataPath, "MoveSyncLevel", levelFileType);
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
            string levelJson = System.IO.File.ReadAllText(filePath);
            LoadInfo(JsonUtility.FromJson<LevelInfo>(levelJson));
        }

        public void SaveFile(string filePath)
        {
            string levelJson = JsonUtility.ToJson(levelInfo);
            System.IO.File.WriteAllText(filePath, levelJson);
        }

        public void LoadSong(string filePath)
        {
            levelInfo.songFile = Path.GetFileNameWithoutExtension(filePath.Replace(resourcePath, ""));
            LevelSequencer.instance.songInfo = levelInfo.songInfo;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(levelInfo.songFile);
            
            onLoadedSong.Invoke();
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
            LevelSequencer.instance.songInfo = levelInfo.songInfo;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(levelInfo.songFile);
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