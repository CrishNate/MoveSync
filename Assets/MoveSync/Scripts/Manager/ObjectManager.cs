using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MoveSync.ModelData;

namespace MoveSync
{
    public class ObjectModel 
    {
        public ObjectModel(PropertyName _objectTag, ModelInput[] _modelInput, string _prefabPath)
        {
            objectTag = _objectTag;
            modelInput = _modelInput;
            prefabPath = _prefabPath;

            prefab = Resources.Load<GameObject>(ObjectManager.beatObjectsPath + prefabPath);
        }

        public ObjectModel(PropertyName _objectTag, ModelInput[] _modelInput)
        {
            objectTag = _objectTag;
            modelInput = _modelInput;
            prefabPath = "";
            prefab = null;
        }

        public PropertyName objectTag;
        public ModelInput[] modelInput;
        public string prefabPath;
        public GameObject prefab; 
    }

    public class ObjectManager : Singleton<ObjectManager>
    {
        public UnityEventPropertyName onSelectedCurrentObject = new UnityEventPropertyName();
        [SerializeField] private UnityEvent _onObjectsLoaded;

        private Dictionary<PropertyName, ObjectModel> _objectModels = new Dictionary<PropertyName, ObjectModel>();
        private ObjectModel _currentObjectModel;
        public static string beatObjectsPath = "MoveSync/BeatObjects/";

        public void SetCurrentObject(string objectTag)
        {
            SetCurrentObject(new PropertyName(objectTag));
        }
        
        public void SetCurrentObject(PropertyName objectTag)
        {
            _currentObjectModel = _objectModels[objectTag];
            onSelectedCurrentObject.Invoke(objectTag);
        }

        void AddSpawnTable(ObjectModel model)
        {
            _objectModels.Add(model.objectTag, model);
        }

        void Start()
        {
            AddSpawnTable(new ObjectModel("laser", new[] { ModelInput.APPEAR.defaultValue("3"), ModelInput.DURATION.defaultValue("1"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.POSITION }, "Laser"));
            AddSpawnTable(new ObjectModel("disco_ball", new[] { ModelInput.APPEAR.defaultValue("2"), ModelInput.DURATION.defaultValue("1"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.COUNT.defaultValue("5"), ModelInput.POSITION }, "DiscoBall"));
            AddSpawnTable(new ObjectModel("bullet", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("0"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.POSITION }, "Bullet"));
            AddSpawnTable(new ObjectModel("bullet_circle", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("0"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.POSITION }, "BulletCircle"));
            AddSpawnTable(new ObjectModel("bullet_prism", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("0"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.POSITION }, "BulletPrism"));
            AddSpawnTable(new ObjectModel("bullet_x", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("0"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.POSITION }, "BulletX"));
            AddSpawnTable(new ObjectModel("explosion", new [] { ModelInput.APPEAR.defaultValue("2"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.COUNT.defaultValue("10"), ModelInput.POSITION }, "ProjectileSphereExplosion"));
            AddSpawnTable(new ObjectModel("bone_wall", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("0"), ModelInput.SIZE.defaultValue("0.2"), ModelInput.SPEED.defaultValue("2"), ModelInput.POSITION }, "BoneWall"));
            AddSpawnTable(new ObjectModel("follow", new [] { ModelInput.APPEAR.defaultValue("6"), ModelInput.DURATION.defaultValue("6"), ModelInput.SIZE.defaultValue("0.5"), ModelInput.SPEED.defaultValue("1"), ModelInput.POSITION }, "FollowLine"));
            AddSpawnTable(new ObjectModel("boop", new [] { ModelInput.APPEAR.defaultValue("0.25"), ModelInput.DURATION.defaultValue("0.25"), ModelInput.SIZE.defaultValue("1"), ModelInput.SHAPE, ModelInput.POSITION }, "Boop"));
            
            AddSpawnTable(new ObjectModel("event", new [] { ModelInput.EVENT.defaultValue("event_none") }));
            
            _onObjectsLoaded.Invoke();
        }

        public Dictionary<PropertyName, ObjectModel> objectModels => _objectModels;
        public ObjectModel currentObjectModel => _currentObjectModel;
    }
}
