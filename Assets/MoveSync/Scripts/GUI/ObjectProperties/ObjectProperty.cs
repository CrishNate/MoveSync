using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public abstract class ObjectProperty : MonoBehaviour
    {
        [HideInInspector] public ObjectProperties _parentObjectProperties;

        protected ModelInput _modelInput;

        [SerializeField] private Text _title;
        

        public virtual void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            _modelInput = modelInput;
            _parentObjectProperties = parentObjectProperties;

            string title = LevelDataManager.PropertyNameToString(modelInput.type).ToLower();
            title = title.First().ToString().ToUpper() + title.Substring(1); // converts string from TEXT to this Text
            _title.text = title;

            UpdateUI();
        }
        
        public virtual void UpdateUI() { }

        protected virtual void OnUpdateValue()
        {
            LevelDataManager.instance.UpdateBeatObject(_parentObjectProperties.selectedObject.id);
        }
    }
}