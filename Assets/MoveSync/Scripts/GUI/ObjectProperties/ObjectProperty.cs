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

        protected ModelInput modelInput;
        protected bool requireUpdateSort = true;

        [SerializeField] private Text _title;
        

        public virtual void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            this.modelInput = modelInput;
            ParentObjectProperties = parentObjectProperties;

            string title = LevelDataManager.PropertyNameToString(modelInput.typeNew).ToLower();
            title = title.First().ToString().ToUpper() + title.Substring(1); // converts string from TEXT to this Text
            _title.text = title;

            UpdateUI();
        }

        public void CopyPropertyToSelected()
        {
            foreach (var selectedObject in ObjectProperties.instance.SelectedObjects)
            {
                if (selectedObject.Value.beatObjectData.tryGetModel(modelInput.GetType(), out var oModelInput))
                {
                    oModelInput.stringValue = modelInput.stringValue;
                    LevelDataManager.instance.UpdateBeatObject(selectedObject.Value.beatObjectData.id);
                }
            }
            
            if (requireUpdateSort)
                LevelDataManager.instance.SortBeatObjects();
        }
        
        public virtual void UpdateUI() { }

        protected virtual void OnUpdateValue()
        {
            LevelDataManager.instance.UpdateBeatObject(ParentObjectProperties.selectedObject.id);
            
            if (requireUpdateSort)
                LevelDataManager.instance.SortBeatObjects();
        }
    }
}