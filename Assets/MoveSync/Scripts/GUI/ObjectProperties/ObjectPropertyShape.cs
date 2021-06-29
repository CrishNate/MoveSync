using System;
using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MoveSync.UI
{
    public class ObjectPropertyShape : ObjectPropertyDropDown
    {
        [SerializeField] private Button randomize;
        
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            dropdown.options.AddRange(MoveSyncData.instance.shapeData.shapesNameList);

            base.Init(modelInput, parentObjectProperties);

            randomize.onClick.AddListener(OnRandomize);
        }
        // Randomizing positions based on beat object that is placed on the same level as the selected beat object
        void OnRandomize()
        {
            var shapeModelInput = modelInput as SHAPE;
            
            // update current
            shapeModelInput.value = MoveSyncData.instance.shapeData.shapesNameList[Random.Range(0, MoveSyncData.instance.shapeData.shapesNameList.Count)];
            LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
            
            // update all selected
            foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
            {
                if (selectedObject.Value.beatObjectData.tryGetModel(out SHAPE selectedShapeModels))
                {
                    selectedShapeModels.value = MoveSyncData.instance.shapeData.shapesNameList[Random.Range(0, MoveSyncData.instance.shapeData.shapesNameList.Count)];
                    LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                }
            }
        }
    }
}