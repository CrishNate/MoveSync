using System.Collections;
using System.Collections.Generic;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class SpawnManager : MonoBehaviour
    {
        private int currentBeatIndex = -1;
        private bool playing;
        private float lastTimeBPM;


        public void ClearAll()
        {
            foreach (var beatObject in FindObjectsOfType<BeatObject>())
            {
                Destroy(beatObject.gameObject);
            }
            foreach (var projectile in FindObjectsOfType<BaseProjectile>())
            {
                Destroy(projectile.gameObject);
            }
        }

        void MoveBeatIndex()
        {
            lastTimeBPM = 0;
            currentBeatIndex = 0;
            foreach (var beatObject in LevelDataManager.instance.levelInfo.beatObjectDatas)
            {
                if (beatObject.time > LevelSequencer.instance.timeBPM) return;
                currentBeatIndex++;
            }
        }

        void TriggerBeatObject(BeatObjectData beatObjectData)
        {
            if (beatObjectData.hasModel(EVENT.TYPE))
            {
                EventManager.instance.InvokeEvent(beatObjectData.getModel<EVENT>(EVENT.TYPE).value, beatObjectData.time);
            }
            else
            {
                BeatObject beatObject =
                    Instantiate(ObjectManager.instance.objectModels[beatObjectData.objectTag].prefab)
                        .GetComponent<BeatObject>();
                beatObject.Init(beatObjectData);
                beatObject.gameObject.SetActive(true);
            }
        }
        
        void Update()
        {
            // update on replaying
            if (playing != LevelSequencer.instance.songPlaying)
            {
                if (playing = LevelSequencer.instance.songPlaying)
                    MoveBeatIndex();
            }

            if (LevelSequencer.instance.songPlaying)
            {
                // update on moving playmarker
                float timeBPM = LevelSequencer.instance.timeBPM;
                if (lastTimeBPM > timeBPM || (timeBPM - lastTimeBPM) > 2.0f)
                    MoveBeatIndex();

                if (currentBeatIndex < LevelDataManager.instance.levelInfo.beatObjectDatas.Count)
                {
                    BeatObjectData nextBeatObject =
                        LevelDataManager.instance.levelInfo.beatObjectDatas[currentBeatIndex];

                    if (timeBPM > nextBeatObject.spawnTime)
                    {
                        TriggerBeatObject(nextBeatObject);
                        currentBeatIndex++;
                    }
                }

                lastTimeBPM = timeBPM;
            }
        }
    }
}