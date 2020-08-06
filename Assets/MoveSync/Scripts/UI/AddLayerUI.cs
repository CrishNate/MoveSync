using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.UI
{
    public class AddLayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject _instance;

        void Start()
        {
            _instance.SetActive(false);
        }

        public void AddLayer()
        {
            GameObject obj = Instantiate(_instance, _instance.transform.parent);
            obj.GetComponent<LayerUI>().layer = LevelDataManager.instance.levelInfo.levelEditorInfo.layersCount++;
            obj.SetActive(true);
            
            transform.SetAsLastSibling();
        }
    }
}