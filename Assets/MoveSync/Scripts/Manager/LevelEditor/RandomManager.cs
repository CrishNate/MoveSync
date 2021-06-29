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
        private static Vector3 _lastSpawnPosition;

        public Vector3 GetRandomPoint(POSITION position)
        {
            Vector3 point1 = position.value;
            Vector3 point2 = point1 + position.pivot;
            float radius = position.pivot.magnitude;
            
            Vector3 newSpawnPoint = Vector3.zero;
            do
            {
                switch (position.randomSpawnType)
                {
                    case RandomSpawnType.Line:
                        newSpawnPoint = Vector3.Lerp(point1, point2, Random.Range(0.0f, 1.0f));
                        break;
                    case RandomSpawnType.Rect:
                        newSpawnPoint = RandomInRect(point1, point2);
                        break;
                    case RandomSpawnType.Circle:
                        newSpawnPoint = RandomInCircle(point1, point2);
                        break;
                    case RandomSpawnType.Sphere:
                        newSpawnPoint = RandomInSphere(point1, point2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } while (Vector3.Distance(newSpawnPoint, _lastSpawnPosition) < Mathf.Sin(Random.Range(0.0f, (float)Math.PI * 0.5f)) * radius);

            return _lastSpawnPosition = newSpawnPoint;
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
            if (beatObjectData.tryGetModel<POSITION>(out var position))
            {
                if (position.randomSpawnType != RandomSpawnType.None)
                {
                    position.value = GetRandomPoint(position);
                    position.pivot = Vector3.one;
                    position.randomSpawnType = RandomSpawnType.None;
                }
            }
        }
        
        void Start()
        {
            LevelDataManager.instance.onNewObject.AddListener(OnNewElement);
        }
    }
}
