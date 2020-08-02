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
    [Serializable]
    public enum RandomSpawnType
    {
        Point,
        Line,
        Rect,
        Circle,
        Sphere
    }

    public class RandomSpawn
    {
        public Vector3 point1;
        public Vector3 point2;
        public RandomSpawnType type;
    }

    public class EventRandomSpawnType : UnityEvent<RandomSpawnType> { };

    public class RandomManager : Singleton<RandomManager>
    {
        public EventRandomSpawnType onRandomSpawnTypeChanged = new EventRandomSpawnType();

        [SerializeField] private GameObject _gizmo;
        [SerializeField] private List<GameObject> _gizmoInstances;
        private Dictionary<PropertyName, RandomSpawn> _randomSpawns = new Dictionary<PropertyName, RandomSpawn>();
        private List<GameObject> _gizmos = new List<GameObject>();
        private RandomSpawn _randomSpawn;
        
        public void OnSelectedTag(PropertyName objectTag)
        {
            if (!_randomSpawns.ContainsKey(objectTag))
            {
                _randomSpawns.Add(objectTag, new RandomSpawn());
            }

            _randomSpawn = _randomSpawns[objectTag];
            onRandomSpawnTypeChanged.Invoke(_randomSpawns[objectTag].type);

            SpawnGizmo();
        }

        public void OnChangeType(RandomSpawnType type)
        {
            _randomSpawns[ObjectManager.instance.currentObjectModel.objectTag].type = type;

            SpawnGizmo();
        }

        public void SpawnGizmo()
        {
            foreach (var gizmo in _gizmos)
            {
                Destroy(gizmo);
            }
            _gizmos.Clear();

            BaseGizmo gizmo1 = Instantiate(_gizmo, _randomSpawn.point1, Quaternion.identity).GetComponent<BaseGizmo>();
            gizmo1.onGizmoMoved.AddListener(x=> _randomSpawn.point1 = x);
            _gizmos.Add(gizmo1.gameObject);

            if (_randomSpawn.type != RandomSpawnType.Point)
            {
                BaseGizmo gizmo2 = Instantiate(_gizmo, _randomSpawn.point2, Quaternion.identity).GetComponent<BaseGizmo>();
                gizmo2.onGizmoMoved.AddListener(x => _randomSpawn.point2 = x);
                _gizmos.Add(gizmo2.gameObject);

                BaseGizmoConnection gizmoConnection = Instantiate(_gizmoInstances[(int)_randomSpawn.type]).GetComponent<BaseGizmoConnection>();
                gizmoConnection.Initialize(gizmo1, gizmo2);
                _gizmos.Add(gizmoConnection.gameObject);
            }
        }

        public Vector3 GetRandomPoint()
        {
            switch (_randomSpawn.type)
            {
                case RandomSpawnType.Point:
                    return _randomSpawn.point1;
                case RandomSpawnType.Line:
                    return Vector3.Lerp(_randomSpawn.point1, _randomSpawn.point2, Random.Range(0.0f, 1.0f));
                case RandomSpawnType.Rect:
                    return RandomInRect();
                case RandomSpawnType.Circle:
                    return RandomInCircle();
                case RandomSpawnType.Sphere:
                    return RandomInSphere();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Vector3 RandomInRect()
        {
            return new Vector3(
                Random.Range(_randomSpawn.point1.x, _randomSpawn.point2.x),
                Random.Range(_randomSpawn.point1.y, _randomSpawn.point2.y),
                Random.Range(_randomSpawn.point1.z, _randomSpawn.point2.z));
        }

        Vector3 RandomInCircle()
        {
            Vector3 offset = Random.insideUnitCircle * Vector3.Distance(_randomSpawn.point1, _randomSpawn.point2);
            offset = new Vector3(offset.x, 0, offset.y);
            return _randomSpawn.point1 + offset;
        }

        Vector3 RandomInSphere()
        {
            return _randomSpawn.point1 + Random.insideUnitSphere * Vector3.Distance(_randomSpawn.point1, _randomSpawn.point2);
        }

        void OnNewElement(BeatObjectData beatObjectData)
        {
            beatObjectData.getModel<TRANSFORM>(TRANSFORM.TYPE).value = new ExTransformData
            {
                position = GetRandomPoint(),
                rotation = Quaternion.identity
            };
        }

        void Start()
        {
            ObjectManager.instance.onSelectedCurrentObject.AddListener(OnSelectedTag);
            LevelDataManager.instance.onNewObjectCreated.AddListener(OnNewElement);
        }
    }
}
