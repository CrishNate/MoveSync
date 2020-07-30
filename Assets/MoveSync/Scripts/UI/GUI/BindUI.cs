using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [RequireComponent(typeof(Button))]
    public class BindUI : MonoBehaviour
    {
        [SerializeField] private BindingManager _bindingManager;
        [SerializeField] private Text _text;
        [SerializeField] private Text _objectTagText;
        public int layer = -1;
        
        private Button _button;
        private bool _awaitButton;


        void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            if (ObjectManager.instance.currentObjectModel.objectTag == null) return;
            
            _awaitButton = true;
            _text.text = "_";
        }

        void OnGUI()
        {
            if (_awaitButton && 
                Event.current.isKey && 
                Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Cancel();
                    return;
                };
                
                _bindingManager.AddKeyBind(layer, new BindKey
                {
                    key = Event.current.keyCode,
                    objectTag = ObjectManager.instance.currentObjectModel.objectTag
                });
                
                _awaitButton = false;
                UpdateUI(Event.current.keyCode, ObjectManager.instance.currentObjectModel.objectTag);
            }
        }

        void Cancel()
        {
            _awaitButton = false;
            _text.text = "";
            _objectTagText.text = "";
            _bindingManager.RemoveKeyBind(layer);
        }
        
        void UpdateUI(KeyCode key, PropertyName objectTag)
        {
            string str = objectTag.ToString();
            _objectTagText.text = str.Substring(0, str.IndexOf(':'));
            _text.text = key.ToString();
        }
    }
}