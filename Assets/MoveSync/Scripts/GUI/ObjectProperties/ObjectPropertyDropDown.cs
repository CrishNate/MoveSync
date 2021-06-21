using System;
using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyDropDown : ObjectProperty
    {
        [SerializeField] private DropDownWithSearth _dropdown;

        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);
            _dropdown.onValueChanged.AddListener(x=> OnUpdateValue());
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            _dropdown.SetValueWithoutNotify(modelInput.stringValue);
        }

        protected override void OnUpdateValue()
        {
            modelInput.stringValue = _dropdown.currentValue;

            base.OnUpdateValue();
        }

        protected DropDownWithSearth dropdown => _dropdown;
    }
}