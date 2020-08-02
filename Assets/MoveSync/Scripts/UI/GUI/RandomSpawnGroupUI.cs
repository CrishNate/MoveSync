using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [RequireComponent(typeof(ToggleGroup))]
    public class RandomSpawnGroupUI : MonoBehaviour
    {
        private ToggleGroup _toggleGroup;
        private Toggle[] _toggles;

        void Start()
        {
            _toggleGroup = GetComponent<ToggleGroup>();
            RandomManager.instance.onRandomSpawnTypeChanged.AddListener(ChangeToggle);
            _toggles = _toggleGroup.GetComponentsInChildren<Toggle>();

            for (int i = 0; i < _toggles.Length; i++)
            {
                var i1 = i;
                _toggles[i].onValueChanged.AddListener(arg0 => OnValueChanged(arg0, (RandomSpawnType)i1));
            }
        }

        void OnValueChanged(bool value, RandomSpawnType type)
        {
            if (value)
            {
                RandomManager.instance.OnChangeType(type);
            }
        }

        void ChangeToggle(RandomSpawnType type)
        {
            _toggles[(int) type].isOn = true;
        }
    }
}