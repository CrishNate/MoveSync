using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
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
        public float scale;
    }

    public class BeatObjectData
    {
        public BeatObjectData Clone()
        {
            BeatObjectData other = (BeatObjectData) MemberwiseClone();
            other.id = System.Guid.NewGuid();
            other.transformData = transformData.Clone();
            other.initialTransformData = initialTransformData.Clone();
            other.animation = new Dictionary<float, string>();
            foreach (var anim in animation)
            {
                other.animation.Add(anim.Key, anim.Value);
            }
            
            return other;
        }
        
        public PropertyName objectTag;
        public Guid id;
        public float time;
        public float appearDuration;
        public float duration;
        public ExTransformData transformData;
        public ExTransformData initialTransformData;
        public Dictionary<float, string> animation;

        // editor
        public int editorLayer;
    }

    public class LevelInfo
    {
        public string songName;
        public string songFile;

        public List<BeatObjectData> beatObjectDatas;
    }

    public class EventNewObject : UnityEvent<BeatObjectData> {};
    
    public class LevelDataManager : Singleton<LevelDataManager>
    {
        public string levelName;

        public EventNewObject onNewObject = new EventNewObject();
        [HideInInspector] public LevelInfo levelInfo;

        private List<LevelInfo> _history = new List<LevelInfo>();
        private int _historyMarker;

        
        /*
         * Object Spawning
         */
        public BeatObjectData NewBeatObjectAtMarker(PropertyName objectTag, int layer = 0)
        {
            BeatObjectData data = NewBeatObject(objectTag, LevelSequencer.instance.timeBPM, layer);
            return data;
        }
        
        public BeatObjectData NewBeatObject(PropertyName objectTag, float time = 0.0f, int layer = 0, bool history = true)
        {
            BeatObjectData data = new BeatObjectData
            {
                objectTag = objectTag, 
                id = System.Guid.NewGuid(), 
                time = time,
                editorLayer = layer,
                animation = new Dictionary<float, string>()
            };

            if (history)
                BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
            return data;
        }
        
        public BeatObjectData CopyBeatObject(BeatObjectData beatObjectData, bool history = true)
        {
            BeatObjectData data = beatObjectData.Clone();
            if (history)
                BackupInfo();

            levelInfo.beatObjectDatas.Add(data);
            onNewObject.Invoke(data);
            return data;
        }
        
        /*
         * Level Data
         */
        public void NewLevel()
        {
            levelInfo.beatObjectDatas.Clear();
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

        void Start()
        {
            levelInfo = new LevelInfo();
            levelInfo.beatObjectDatas = new List<BeatObjectData>();
        }
    }
}