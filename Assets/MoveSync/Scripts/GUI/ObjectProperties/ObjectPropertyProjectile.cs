using System;
using System.Collections.Generic;
using System.Linq;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class ObjectPropertyProjectile : ObjectPropertyDropDown
    {
        public override void Init(ModelInput modelInput, ObjectProperties parentObjectProperties)
        {
            dropdown.options.AddRange(MoveSyncData.instance.projectileData.projectilesNameList);

            base.Init(modelInput, parentObjectProperties);
        }
    }
}