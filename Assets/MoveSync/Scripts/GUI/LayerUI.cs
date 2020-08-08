using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [RequireComponent(typeof(ButtonEx))]
    public class LayerUI : MonoBehaviour
    {
        public int layer = -1;

        [SerializeField] private BindingManager _bindingManager;
        [SerializeField] private ButtonEx _bindButton;
        [SerializeField] private Button _clearLayerButton;
        [SerializeField] private Text _text;
        [SerializeField] private Text _objectTagText;
        
        private bool _awaitButton;
        private static LayerUI _lastBindUi;

        
        public void ClearBind()
        {
            _awaitButton = false;
            _text.text = "";
            _objectTagText.text = "";
            _lastBindUi = null;

            _bindingManager.RemoveKeyBind(layer);
        }
        public void Cancel()
        {
            if (_bindingManager.bind.ContainsKey(layer))
                UpdateUI(_bindingManager.bind[layer]);
            else
                _text.text = "";

            _awaitButton = false;
            _lastBindUi = null;
        }
        
        void OnClearLayer()
        {
            LevelDataManager.instance.ClearLayer(layer);
        }
        
        void OnStartBind()
        {
            if (_lastBindUi != null)
                _lastBindUi.Cancel();
                
            if (ObjectManager.instance.currentObjectModel.objectTag == null) return;
            
            _text.text = "_";
            _awaitButton = true;
            _lastBindUi = this;
        }
        
        void OpenBindProperties()
        {
            if (_bindingManager.bind.TryGetValue(layer, out var bindKey))
                ObjectProperties.instance.OpenProperties(bindKey.objectModel.modelInput);
        }

        void UpdateUI(BindKey bindKey)
        {
            UpdateUI(bindKey.key, bindKey.objectModel.objectTag);
        }

        void UpdateUI(KeyCode key, PropertyName objectTag)
        {
            string str = objectTag.ToString();
            _objectTagText.text = str.Substring(0, str.IndexOf(':'));
            _text.text = key.ToString();
        }
    
        void Start()
        {
            _bindButton.onLeftClick.AddListener(OnStartBind);
            _bindButton.onRightClick.AddListener(OpenBindProperties);
            _clearLayerButton.onClick.AddListener(OnClearLayer);
        }

        void OnGUI()
        {
            if (_awaitButton && Event.current.isKey && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    ClearBind();
                    return;
                };
                
                _bindingManager.AddKeyBind(layer, new BindKey
                {
                    key = Event.current.keyCode,
                    objectModel = ObjectManager.instance.currentObjectModel.Clone()
                });

                _lastBindUi = null;
                _awaitButton = false;
                UpdateUI(Event.current.keyCode, ObjectManager.instance.currentObjectModel.objectTag);
            }
        }
    }
}