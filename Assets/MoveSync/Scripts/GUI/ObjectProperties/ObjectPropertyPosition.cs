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
        
        
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);
            
            noRandomToggle.onValueChanged.AddListener(x=> OnSwitchRandom(x, RandomSpawnType.None));
            lineToggle.onValueChanged.AddListener(x=> OnSwitchRandom(x, RandomSpawnType.Line));
            boxToggle.onValueChanged.AddListener(x=> OnSwitchRandom(x, RandomSpawnType.Rect));
            circleToggle.onValueChanged.AddListener(x=> OnSwitchRandom(x, RandomSpawnType.Circle));
            sphereToggle.onValueChanged.AddListener(x=> OnSwitchRandom(x, RandomSpawnType.Sphere));
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
    }
}