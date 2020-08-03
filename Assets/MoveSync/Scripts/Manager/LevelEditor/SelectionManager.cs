using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class SelectionManager : Singleton<SelectionManager>
    {
        public UnityEventBeatObjectData onSelected;
        public UnityEventBeatObjectData onDeselected;
        
        private BeatObjectData _selectedObject;
        private Dictionary<int, BeatObjectData> _selectedObjects;
        
        
        void SelectOne(BeatObjectData beatObjectData)
        {
            // remove old selection
            if (_selectedObject.id != null &&
                _selectedObject != _selectedObjects[beatObjectData.id])
            {
                RemoveSelection(_selectedObject);
                AddSelection(beatObjectData);
            } 
            
            _selectedObject = beatObjectData;
            onSelected.Invoke(_selectedObject);
        }
        
        bool AddSelection(BeatObjectData beatObjectData)
        {
            if (_selectedObjects.ContainsKey(beatObjectData.id)) return false;
            _selectedObjects.Add(beatObjectData.id, beatObjectData);

            return true;
        }
        
        void RemoveSelection(BeatObjectData beatObjectData)
        {
            _selectedObjects.Remove(beatObjectData.id);
            if(_selectedObject.id == beatObjectData.id) 
                onDeselected.Invoke(_selectedObject);
        }

        void WipeSelections()
        {
            _selectedObjects.Clear();
            onDeselected.Invoke(_selectedObject);
            _selectedObject.id = null;
        }


        public BeatObjectData selectedObject => _selectedObject;
    }
}
