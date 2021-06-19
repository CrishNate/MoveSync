using System;
using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyShape : ObjectProperty
    {
        [SerializeField] private Dropdown _dropdown;

        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            _dropdown.ClearOptions();
            _dropdown.AddOptions(MoveSyncData.instance.shapeData.shapesNameList);

            base.Init(modelInput, parentObjectProperties);

            _dropdown.onValueChanged.AddListener(x=> OnUpdateValue());
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            _dropdown.SetValueWithoutNotify(MoveSyncData.instance.shapeData.shapesNameList.FindIndex(s => modelInput.stringValue == s));
        }

        protected override void OnUpdateValue()
        {
            modelInput.stringValue = MoveSyncData.instance.shapeData.shapesNameList[_dropdown.value];

            base.OnUpdateValue();
        }
    }
}