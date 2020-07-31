using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync
{
    public class InputData : Singleton<InputData>
    {
        public static bool shouldSnap => Input.GetKey(KeyCode.LeftShift);
        public static bool shouldOnlyLayer => Input.GetKey(KeyCode.LeftControl);
        public static bool shouldCopy => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C);
        public static bool shouldPaste => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V);
        public static bool shouldDelete => Input.GetKeyDown(KeyCode.Delete);
    }
}
