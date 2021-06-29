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
            requireUpdateSort = false;
            
            _xInputTextField.onEndEdit.AddListener(x=> OnUpdateValue());
            _yInputTextField.onEndEdit.AddListener(x=> OnUpdateValue());
            _zInputTextField.onEndEdit.AddListener(x=> OnUpdateValue());
        }

        public override void UpdateUI()
        {
            base.UpdateUI();

            Vector3 value = ((Vector3ModelInput) modelInput).value;
            _xInputTextField.SetTextWithoutNotify(value.x.ToString());
            _yInputTextField.SetTextWithoutNotify(value.y.ToString());
            _zInputTextField.SetTextWithoutNotify(value.z.ToString());
        }
        
        protected override void OnUpdateValue()
        {
            var vector3ModelInput = modelInput as Vector3ModelInput;
            if (_xInputTextField.text != string.Empty &&
                _yInputTextField.text != string.Empty &&
                _zInputTextField.text != string.Empty)
            {
                vector3ModelInput.value = new Vector3(
                    float.Parse(_xInputTextField.text),
                    float.Parse(_yInputTextField.text),
                    float.Parse(_zInputTextField.text));
            }

            base.OnUpdateValue();
        }
    }
}