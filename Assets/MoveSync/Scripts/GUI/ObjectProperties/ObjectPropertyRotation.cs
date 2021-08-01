using System.Globalization;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyRotation : ObjectPropertyVector3
    {
        [SerializeField] private Button _targetPlayer;
        [SerializeField] private Button _randomizeNearPlayer;
        
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);
            
            _targetPlayer.onClick.AddListener(OnTarget);
            _randomizeNearPlayer.onClick.AddListener(OnRandomize);
        }

        void ApplyRotation(ROTATION rotationModel, Vector3 direction)
        {
            Quaternion unrolledRotation = Quaternion.LookRotation(rotationModel.value * Vector3.forward);
            Quaternion rolledLocalRotation = Quaternion.Inverse(rotationModel.value) * unrolledRotation;
            rotationModel.value = Quaternion.LookRotation(direction);
            rotationModel.value = Quaternion.AngleAxis(-rolledLocalRotation.eulerAngles.z, rotationModel.value * Vector3.forward) * rotationModel.value;
        }
        
        // Randomizing rotation so any selected beat object will target player but with some deviation
        void OnRandomize()
        {
            // update current
            var rotationModel = modelInput as ROTATION;
            if (ParentObjectProperties.selectedObject.tryGetModel(out POSITION positionModel))
            {
                ApplyRotation(rotationModel, PlayerBehaviour.GetRandomPointNearPlayer() - positionModel.value);
                LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
            }
            
            // update all selected
            foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
            {
                if (selectedObject.Value.beatObjectData.tryGetModel(out POSITION selectedPositionModel)
                    && selectedObject.Value.beatObjectData.tryGetModel(out ROTATION selectedRotationModel))
                {
                    ApplyRotation(selectedRotationModel, PlayerBehaviour.GetRandomPointNearPlayer() - selectedPositionModel.value);
                    LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                }
            }
        }
        
        void OnTarget()
        {
            // update current
            var rotationModel = modelInput as ROTATION;
            if (ParentObjectProperties.selectedObject.tryGetModel(out POSITION positionModel))
            {
                ApplyRotation(rotationModel, PlayerBehaviour.instance.transform.position - positionModel.value);
                LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
            }
            
            // update all selected
            foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
            {
                if (selectedObject.Value.beatObjectData.tryGetModel(out POSITION selectedPositionModels)
                    && selectedObject.Value.beatObjectData.tryGetModel(out ROTATION selectedRotationModel))
                {
                    ApplyRotation(selectedRotationModel, PlayerBehaviour.instance.transform.position - selectedPositionModels.value);
                    LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                }
            }
        }
    }
}