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
                foreach (var m in modelInputsData)
                {
                    modelInputs.Add(m.type, m);
                }
            }

            return modelInputs.ContainsKey(type);
        }

        public ModelInput getModel(PropertyName type)
        {
            if (modelInputs == null)
            {
                modelInputs = new Dictionary<PropertyName, ModelInput>();
                foreach (var m in modelInputsData)
                {
                    modelInputs.Add(m.type, m);
                }
            }

            return modelInputs[type];
        }
    }

    public class LevelInfo
    {
        public string songName;
        public string songFile;
        public SongInfo songInfo;

        public List<BeatObjectData> beatObjectDatas;
    }

    public class EventBeatObject : UnityEvent<BeatObjectData> {};
    
    public class LevelDataManager : Singleton<LevelDataManager>
    {
        public static string levelFileType = "mslevel";
        
        public string levelName;

        public EventBeatObject onNewObject = new EventBeatObject();
        public EventBeatObject onRemoveObject = new EventBeatObject();
        public UnityEvent onLoadedSong = new UnityEvent();
        [HideInInspector] public LevelInfo levelInfo = new LevelInfo();

        private List<LevelInfo> _history = new List<LevelInfo>();
        private int _historyMarker;

        
        /*
         * Object Spawning
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

        /*
         * History
         */
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
            this.levelInfo = levelInfo;
            LevelSequencer.instance.songInfo = levelInfo.songInfo;
            LevelSequencer.instance.audioSource.clip = Resources.Load<AudioClip>(levelInfo.songFile);
            
            onLoadedSong.Invoke();
        }

        void Start()
        {
            levelInfo.beatObjectDatas = new List<BeatObjectData>();
        }
    }
}