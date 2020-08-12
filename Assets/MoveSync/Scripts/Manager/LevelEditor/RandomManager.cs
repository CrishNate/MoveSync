using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync.LevelGizmos;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class RandomManager : Singleton<RandomManager>
    {
        public Vector3 GetRandomPoint(POSITION position)
        {
            Vector3 point1 = position.value;
            Vector3 point2 = point1 + position.pivot;
            
            switch (position.randomSpawnType)
            {
                case RandomSpawnType.Line:
                    return Vector3.Lerp(point1, point2, Random.Range(0.0f, 1.0f));
                case RandomSpawnType.Rect:
                    return RandomInRect(point1, point2);
                case RandomSpawnType.Circle:
                    return RandomInCircle(point1, point2);
                case RandomSpawnType.Sphere:
                    return RandomInSphere(point1, point2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        Vector3 RandomInRect(Vector3 point1, Vector3 point2)
        {
            return new Vector3(
                Random.Range(point1.x, point2.x),
                Random.Range(point1.y, point2.y),
                Random.Range(point1.z, point2.z));
        }
        
        Vector3 RandomInCircle(Vector3 point1, Vector3 point2)
        {
            Vector3 offset = Random.insideUnitCircle * Vector3.Distance(point1, point2);
            offset = new Vector3(offset.x, 0, offset.y);
            return point1 + offset;
        }
        
        Vector3 RandomInSphere(Vector3 point1, Vector3 point2)
        {
            return point1 + Random.insideUnitSphere * Vector3.Distance(point1, point2);
        }
        
        void OnNewElement(BeatObjectData beatObjectData)
        {
            POSITION position = beatObjectData.getModel<POSITION>();

            if (position.randomSpawnType != RandomSpawnType.None)
            {
                position.value = GetRandomPoint(position);
                position.pivot = Vector3.one;
                position.randomSpawnType = RandomSpawnType.None;
            }
        }
        
        void Start()
        {
            LevelDataManager.instance.onNewObject.AddListener(OnNewElement);
        }
    }
}
