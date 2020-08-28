using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public abstract class ObjectProperty : MonoBehaviour
    {
        [HideInInspector] public ObjectProperties ParentObjectProperties;

        protected ModelInput _modelInput;

        [SerializeField] private Text _title;
        

        public virtual void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            _modelInput = modelInput;
            ParentObjectProperties = parentObjectProperties;

            string title = LevelDataManager.PropertyNameToString(modelInput.type).ToLower();
            title = title.First().ToString().ToUpper() + title.Substring(1); // converts string from TEXT to this Text
            _title.text = title;

            UpdateUI();
        }

        public void CopyPropertyToSelected()
        {
            ModelInput modelInput;
            foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
            {
                if (selectedObject.Value.beatObjectData.tryGetModel(_modelInput.GetType(), out modelInput))
                {
                    modelInput.stringValue = _modelInput.stringValue;
                    LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                }
            }
        }
        
        public virtual void UpdateUI() { }

        protected virtual void OnUpdateValue()
        {
            LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
        }
    }
}