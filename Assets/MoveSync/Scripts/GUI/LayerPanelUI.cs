using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.UI
{
    public class LayerPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject _instance;
        [SerializeField] private GameObject _addLayerButton;
        private List<GameObject> _layers = new List<GameObject>();

        
        public void OnSongLoaded()
        {
            Clear();
            
            for (int i = 0; i < LevelDataManager.instance.levelInfo.levelEditorInfo.layersCount; i++)
            {
                AddLayer(i);
            }
        }

        public void Clear()
        {
            foreach (var layer in _layers)
            {
                Destroy(layer);
            }
            _layers.Clear();
        }

        public void AddNewLayer()
        {
            AddLayer(LevelDataManager.instance.levelInfo.levelEditorInfo.layersCount++);
        }        
        
        void AddLayer(int layer)
        {
            GameObject obj = Instantiate(_instance, _instance.transform.parent);
            obj.GetComponent<LayerUI>().layer = layer;
            obj.SetActive(true);
            
            _addLayerButton.transform.SetAsLastSibling();
            _layers.Add(obj);
        }

        void Start()
        {
            _instance.SetActive(false);
            
            LevelDataManager.instance.onLoadedSong.AddListener(OnSongLoaded);
        }
    }
}