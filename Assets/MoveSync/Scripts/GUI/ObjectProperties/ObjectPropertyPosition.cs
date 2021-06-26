using System;
using System.Globalization;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyPosition : ObjectPropertyVector3
    {
        [SerializeField] private Toggle noRandomToggle;
        [SerializeField] private Toggle lineToggle;
        [SerializeField] private Toggle boxToggle;
        [SerializeField] private Toggle circleToggle;
        [SerializeField] private Toggle sphereToggle;
        [SerializeField] private Button randomize;
        
        
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);

            if (!parentObjectProperties.selectedObjectInLevel)
            {
                noRandomToggle.onValueChanged.AddListener(x => OnSwitchRandom(x, RandomSpawnType.None));
                lineToggle.onValueChanged.AddListener(x => OnSwitchRandom(x, RandomSpawnType.Line));
                boxToggle.onValueChanged.AddListener(x => OnSwitchRandom(x, RandomSpawnType.Rect));
                circleToggle.onValueChanged.AddListener(x => OnSwitchRandom(x, RandomSpawnType.Circle));
                sphereToggle.onValueChanged.AddListener(x => OnSwitchRandom(x, RandomSpawnType.Sphere));
                
                randomize.gameObject.SetActive(false);
            }
            else
            {
                randomize.onClick.AddListener(OnRandomize);

                noRandomToggle.gameObject.SetActive(false);
                lineToggle.gameObject.SetActive(false);
                boxToggle.gameObject.SetActive(false);
                circleToggle.gameObject.SetActive(false);
                sphereToggle.gameObject.SetActive(false);
            }
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            
            var positionModelInput = modelInput as POSITION;
            switch (positionModelInput.randomSpawnType)
            {
                case RandomSpawnType.None:
                    noRandomToggle.isOn = true;
                    break;
                case RandomSpawnType.Line:
                    lineToggle.isOn = true;
                    break;
                case RandomSpawnType.Rect:
                    boxToggle.isOn = true;
                    break;
                case RandomSpawnType.Circle:
                    circleToggle.isOn = true;
                    break;
                case RandomSpawnType.Sphere:
                    sphereToggle.isOn = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnSwitchRandom(bool on, RandomSpawnType type)
        {
            if (!on) return;
            
            var positionModelInput = modelInput as POSITION;
            positionModelInput.randomSpawnType = type;
            
            OnUpdateValue();
        }

        // Randomizing positions based on beat object that is placed on the same level as the selected beat object
        void OnRandomize()
        {
            if (BindingManager.instance.bind.TryGetValue(ParentObjectProperties.selectedObject.editorLayer,
                out BindKey bindKey))
            {
                if (bindKey.beatObjectData.tryGetModel<POSITION>(out var positionModel))
                {
                    if (positionModel.randomSpawnType == RandomSpawnType.None) return;
                    
                    var positionModelInput = modelInput as POSITION;
                    
                    // update current
                    positionModelInput.value = RandomManager.instance.GetRandomPoint(positionModel);
                    LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
                    
                    // update all selected
                    foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
                    {
                        if (selectedObject.Value.beatObjectData.tryGetModel(out POSITION selectedPositionModels))
                        {
                            selectedPositionModels.value = RandomManager.instance.GetRandomPoint(positionModel);
                            LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                        }
                    }
                }
            }
        }
    }
}