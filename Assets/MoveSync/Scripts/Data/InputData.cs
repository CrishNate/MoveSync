using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using UnityEngine;

namespace MoveSync
{
    public class InputData : MonoBehaviour
    {
        public static bool shouldSnap => Input.GetKey(KeyCode.LeftShift) ^ _snapOn;

        public static bool shouldOnlyLayer => Input.GetKey(KeyCode.LeftControl);
        public static bool shouldCopy => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C);
        public static bool shouldPaste => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V);
        public static bool shouldDelete => Input.GetKeyDown(KeyCode.Delete);
        public static bool shouldClose => Input.GetKeyDown(KeyCode.Escape);

        private static bool _snapOn;

    
        public void SetSnapBool(bool snapOn)
        {
            _snapOn = snapOn;
        }
    }
}
