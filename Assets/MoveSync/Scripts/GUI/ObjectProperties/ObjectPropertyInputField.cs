using System;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyInputField : ObjectProperty
    {
        [SerializeField] private InputField _inputTextField;


        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);
            
            _inputTextField.onValueChanged.AddListener(x=> OnUpdateValue());
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            _inputTextField.SetTextWithoutNotify(modelInput.stringValue);
        }

        protected override void OnUpdateValue()
        {
            if (_inputTextField.text != string.Empty)
                modelInput.stringValue = _inputTextField.text;

            base.OnUpdateValue();
        }
    }
}