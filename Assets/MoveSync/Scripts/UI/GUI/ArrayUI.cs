using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoveSync.UI;

namespace UnityEditorEx.UI
{
    // copy
    public class ArrayUI : MonoBehaviour
    {
        [SerializeField] private GameObject _instanceUI;
        [SerializeField] private int _copiesCount;
        [SerializeField] private Vector2 _offset;

        void Start()
        {
            for (int i = 1; i <= _copiesCount; i++)
            {
                GameObject instance = Instantiate(_instanceUI, _instanceUI.transform.parent);
                instance.transform.localPosition = _instanceUI.transform.localPosition + (Vector3) _offset * i;

                instance.GetComponent<BindUI>().layer = i;
            }
        }
    }
}