using UnityEngine;

namespace MoveSync
{
    public class Billboard : MonoBehaviour
    {
        void LateUpdate() 
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}