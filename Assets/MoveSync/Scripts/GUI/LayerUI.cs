using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [RequireComponent(typeof(ButtonEx))]
    public class LayerUI : MonoBehaviour
    {
        [HideInInspector] public int layer = -1;
        public ButtonEx bindButton;
        
        [SerializeField] private Button _clearLayerButton;
        [SerializeField] private Outline _bindButtonOutline;
        [SerializeField] private Outline _layerOuntline;
        [SerializeField] private Text _text;
        [SerializeField] private Text _objectTagText;


        public void OnStartListening()
        {
            Clear();
            _bindButtonOutline.enabled = true;
        }

        public void OnFinishListening()
        {
            if (BindingManager.instance.bind.ContainsKey(layer))
                UpdateUI(BindingManager.instance.bind[layer]);
            else
                Clear();
            
            _bindButtonOutline.enabled = false;
        }

        void Clear()
        {
            _text.text = "";
        }

        public void OnSelected()
        {
            _layerOuntline.enabled = true;
        }
        
        public void OnDeselect()
        {
            _layerOuntline.enabled = false;
        }
        
        void OnClearLayer()
        {
            LevelDataManager.instance.ClearLayer(layer);
        }

        void UpdateUI(BindKey bindKey)
        {
            string str = bindKey.beatObjectData.objectTag.ToString();
            _objectTagText.text = str.Substring(0, str.IndexOf(':'));
            _text.text = bindKey.key.ToString();
        }

        void SelectProperties()
        {
            if (BindingManager.instance.bind.TryGetValue(layer, out var bindKey))
                ObjectProperties.instance.Select(bindKey.beatObjectData);
        }

        void Start()
        {
            bindButton.onLeftClick.AddListener(() => BindingManager.instance.StartListening(layer));
            bindButton.onMiddleClick.AddListener(() => BindingManager.instance.RemoveKeyBind(layer));
            bindButton.onRightClick.AddListener(SelectProperties);
            
            _clearLayerButton.onClick.AddListener(OnClearLayer);
        }
    }
}