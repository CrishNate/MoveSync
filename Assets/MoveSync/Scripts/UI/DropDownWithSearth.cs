using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoveSync.UI
{
    [Serializable]
    public class UnityEventDropDownChoose : UnityEvent<string> { };

    public class DropDownWithSearth : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        protected internal class DropdownItem : MonoBehaviour
        {
            [SerializeField]
            private Text m_Text;
            [SerializeField]
            private RectTransform m_RectTransform;
            [SerializeField]
            private Toggle m_Toggle;

            public Text          text          { get { return m_Text;          } set { m_Text = value;           } }
            public RectTransform rectTransform { get { return m_RectTransform; } set { m_RectTransform = value;  } }
            public Toggle        toggle        { get { return m_Toggle;        } set { m_Toggle = value;         } }
        }
        
        [Header("Drop Down /w/ Searth")] [SerializeField]
        private Canvas _canvas;

        [SerializeField] private Text _rootText;
        [SerializeField] private GameObject _dropDown;
        [SerializeField] private RectTransform _itemTemplate;
        [SerializeField] private InputField _inputField;
        [SerializeField] private UnityEventDropDownChoose _onValueChanged;

        [HideInInspector]
        public List<string> options
        {
            get { return _options; }
            set { _options = value; UpdateList(); }
        }
        [SerializeField] private List<string> _options;

        private List<DropdownItem> _items = new List<DropdownItem>();
        private GameObject _blocker;

        private Text _itemText;
        private Toggle _item;

        void Start()
        {
            _dropDown.SetActive(false);
            _itemTemplate.gameObject.SetActive(false);

            _inputField.onValueChanged.AddListener(FilterList);
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ShowList();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            ShowList();
        }

        public void OnCancel(BaseEventData eventData)
        {
            HideList();
        }

        protected virtual GameObject CreateBlocker(Canvas rootCanvas)
        {
            // Create blocker GameObject.
            GameObject blocker = new GameObject("Blocker");

            // Setup blocker RectTransform to cover entire root canvas area.
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.SetParent(rootCanvas.transform, false);
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
            Canvas blockerCanvas = blocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            Canvas dropdownCanvas = _dropDown.GetComponent<Canvas>();
            blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

            // Find the Canvas that this dropdown is a part of
            Canvas parentCanvas = null;
            Transform parentTransform = _itemTemplate.parent;
            while (parentTransform != null)
            {
                parentCanvas = parentTransform.GetComponent<Canvas>();
                if (parentCanvas != null)
                    break;

                parentTransform = parentTransform.parent;
            }

            // If we have a parent canvas, apply the same raycasters as the parent for consistency.
            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (blocker.GetComponent(raycasterType) == null)
                    {
                        blocker.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                // Add raycaster since it's needed to block.
                GetOrAddComponent<GraphicRaycaster>(blocker);
            }


            // Add image since it's needed to block, but make it clear.
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(HideList);

            return blocker;
        }
        protected virtual void DestroyBlocker()
        {
            Destroy(_blocker);
        }

        public void ShowList()
        {
            _dropDown.SetActive(true);

            UpdateList();

            if (!_blocker)
                _blocker = CreateBlocker(_canvas);
        }

        public void HideList()
        {
            _dropDown.SetActive(false);

            ClearList();

            DestroyBlocker();
        }

        public void UpdateList()
        {
            FilterList(_inputField.text);
        }
        
        public void UpdateList(List<string> newOptions)
        {
            ClearList();

            if (!_dropDown.activeInHierarchy)
                return; 

            for (int i = 0; i < newOptions.Count; i++)
            {
                DropdownItem item = AddItem(newOptions[i]);
                item.toggle.onValueChanged.AddListener(x => ValueChanged(item.text.text));
                
                Vector3 position = item.rectTransform.localPosition;
                position.y = -item.rectTransform.sizeDelta.y * i;
                item.rectTransform.localPosition = position;
            }

            RectTransform content = (RectTransform) _itemTemplate.parent;
            Vector2 size = content.sizeDelta;
            size.y = _itemTemplate.sizeDelta.y * newOptions.Count;
            content.sizeDelta = size;

        }

        public void FilterList(string input)
        {
            List<string> filterOptions = _options.FindAll(option => option.IndexOf(input) >= 0);
            UpdateList(filterOptions);
        }

        public void ValueChanged(string value)
        {
            _rootText.text = value;
            _onValueChanged.Invoke(value);
        }

        private DropdownItem AddItem(string name)
        {
            GameObject item = Instantiate(_itemTemplate.gameObject, _itemTemplate.parent);
            item.SetActive(true);

            DropdownItem dropdownItem = item.AddComponent<DropdownItem>();
            dropdownItem.rectTransform = (RectTransform) item.transform;
            dropdownItem.text = item.GetComponentInChildren<Text>();
            dropdownItem.toggle = item.GetComponent<Toggle>();

            dropdownItem.text.text = name;
            
            _items.Add(dropdownItem);
            return dropdownItem;
        }

        public void ClearList()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();
        }
    }
}
