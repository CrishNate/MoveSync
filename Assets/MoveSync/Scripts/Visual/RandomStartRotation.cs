using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class RandomStartRotation : MonoBehaviour
    {
        private void Start()
        {
            transform.rotation = Random.rotation;
        }
    }
}