using System;
using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyShape : ObjectPropertyDropDown
    {
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            dropdown.options.AddRange(MoveSyncData.instance.shapeData.shapesNameList);

            base.Init(modelInput, parentObjectProperties);
        }
    }
}