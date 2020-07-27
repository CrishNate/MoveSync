using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [RequireComponent(typeof(Button))]
    public class BindUI : MonoBehaviour
    {
        [SerializeField] private BindingManager _bindingManager;
        [SerializeField] private Text _text;
        [SerializeField] private int _layer = -1;
        
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
                _bindingManager.AddKeyBind(_layer, new BindKey
                {
                    key = Event.current.keyCode,
                    objectTag = ObjectManager.instance.currentObjectModel.objectTag
                });
                
                _awaitButton = false;
                UpdateUI(Event.current.keyCode);
            }
        }
        
        void UpdateUI(KeyCode key)
        {
            _text.text = key.ToString();
        }
    }
}