using System.Collections;
using System.Collections.Generic;
using MoveSync;
using MoveSync.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectListUI : MonoBehaviour
{
    [SerializeField] private DropDownWithSearth _dropDownWithSearth;

    
    public void UpdateObjectList()
    {
        foreach (var objectModel in ObjectManager.instance.objectModels)
        {
            _dropDownWithSearth.options.Add(objectModel.Value.objectName);
        }
    }
}
