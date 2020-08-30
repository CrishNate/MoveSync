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
        
        [SerializeField] private Button _bindKeyButton;
        [SerializeField] private Button _clearLayerButton;
        [SerializeField] private GameObject _bindButtonSelected;
        [SerializeField] private GameObject _objectButtonSelected;
        [SerializeField] private Outline _layerOuntline;
        [SerializeField] private Text _text;
        [SerializeField] private Text _objectTagText;


        public void OnStartListening()
        {
            _text.text = "";
            _bindButtonSelected.SetActive(true);
        }
        
        public void OnFinishListening()
        {
            if (BindingManager.instance.bind.ContainsKey(layer))
                UpdateUI(BindingManager.instance.bind[layer]);
            else
                _text.text = "";
            
            _bindButtonSelected.SetActive(false);
        }
        
        public void OnStartObjectBind()
        {
            _objectTagText.text = "";
            _objectButtonSelected.SetActive(true);
        }
        
        public void OnFinishObjectBind()
        {
            if (BindingManager.instance.bind.ContainsKey(layer))
                UpdateUI(BindingManager.instance.bind[layer]);
            else
                _objectTagText.text = "";
            
            _objectButtonSelected.SetActive(false);
        }

        public void Clear()
        {
            _text.text = "";
            _objectTagText.text = "";
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
            if (bindKey.beatObjectData != null)
            {
                string str = bindKey.beatObjectData.objectTag.ToString();
                _objectTagText.text = LevelDataManager.PropertyNameToString(bindKey.beatObjectData.objectTag);
            }
            
            if (bindKey.key != KeyCode.None)
                _text.text = bindKey.key.ToString();
        }

        void SelectProperties()
        {
            if (BindingManager.instance.bind.TryGetValue(layer, out var bindKey))
                ObjectProperties.instance.Select(bindKey.beatObjectData);
        }

        void Start()
        {
            _bindKeyButton.onClick.AddListener(() => BindingManager.instance.StartListening(layer));
            bindButton.onLeftClick.AddListener(() => BindingManager.instance.ObjectBind(layer));
            bindButton.onMiddleClick.AddListener(() => BindingManager.instance.RemoveKeyBind(layer));
            bindButton.onRightClick.AddListener(SelectProperties);
            
            _clearLayerButton.onClick.AddListener(OnClearLayer);
        }
    }
}