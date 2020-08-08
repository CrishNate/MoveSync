using System.Globalization;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyVector3 : ObjectProperty
    {
        [SerializeField] private InputField _xInputTextField;
        [SerializeField] private InputField _yInputTextField;
        [SerializeField] private InputField _zInputTextField;


        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            base.Init(modelInput, parentObjectProperties);
            
            _xInputTextField.onValueChanged.AddListener(x=> OnUpdateValue());
            _yInputTextField.onValueChanged.AddListener(x=> OnUpdateValue());
            _zInputTextField.onValueChanged.AddListener(x=> OnUpdateValue());
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            Vector3 value = ((Vector3ModelInput) _modelInput).value;
            _xInputTextField.text = value.x.ToString();
            _yInputTextField.text = value.y.ToString();
            _zInputTextField.text = value.z.ToString();
        }
        
        protected override void OnUpdateValue()
        {
            var modelInput = _modelInput as Vector3ModelInput;
            if (_xInputTextField.text != string.Empty &&
                _yInputTextField.text != string.Empty &&
                _zInputTextField.text != string.Empty)
            {
                modelInput.value = new Vector3(
                    float.Parse(_xInputTextField.text),
                    float.Parse(_yInputTextField.text),
                    float.Parse(_zInputTextField.text));
            }

            base.OnUpdateValue();
        }
    }
}