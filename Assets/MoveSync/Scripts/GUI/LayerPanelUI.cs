using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveSync.UI
{
    public class LayerPanelUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GameObject _instance;
        private List<LayerUI> _layers = new List<LayerUI>();
        private static int layersCount = 0;

        
        void AddLayer(int layer)
        {
            GameObject obj = Instantiate(_instance, _instance.transform.parent);
            obj.SetActive(true);

            var layerUi = obj.GetComponent<LayerUI>();
            layerUi.layer = layer;
            
            _layers.Add(layerUi);
        }

        private void Update()
        {
            int requireCount = Mathf.CeilToInt(_content.rect.height / ((RectTransform) _instance.transform).rect.height);
            if (requireCount > _layers.Count)
            {
                for (int i = 0; i < requireCount - _layers.Count; i++)
                {
                    AddLayer(layersCount++);
                }
            }
        }

        bool TryGetLayerByData(BeatObjectData beatObjectData, out LayerUI layerUi)
        {
            foreach (var bindKey in BindingManager.instance.bind)
            {
                if (bindKey.Value.beatObjectData.id == beatObjectData.id)
                {
                    layerUi = _layers[beatObjectData.editorLayer];
                    return true;
                }
            }

            layerUi = null;
            return false;
        }

        void OnSelectedLayer(BeatObjectData beatObjectData)
        {
            if (TryGetLayerByData(beatObjectData, out var layerUi))
                layerUi.OnSelected();
        }
        
        void OnDeselectedLayer(BeatObjectData beatObjectData)
        {
            if (TryGetLayerByData(beatObjectData, out var layerUi))
                layerUi.OnDeselect();
        }

        void Start()
        {
            _instance.SetActive(false);

            BindingManager.instance.onStartListening.AddListener(x => _layers[x].OnStartListening());
            BindingManager.instance.onFinishListening.AddListener(x => _layers[x].OnFinishListening());
            BindingManager.instance.onStartAwaitConfirm.AddListener(x => _layers[x].OnStartObjectBind());
            BindingManager.instance.onFinishAwaitConfirm.AddListener(x => _layers[x].OnFinishObjectBind());
            BindingManager.instance.onClearBind.AddListener(x => _layers[x].Clear());
            
            ObjectProperties.instance.onSelected.AddListener(OnSelectedLayer);
            ObjectProperties.instance.onDeselected.AddListener(OnDeselectedLayer);
        }
    }
}