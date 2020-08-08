using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MoveSync.UI
{
    public class ButtonEx : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent onLeftClick;
        public UnityEvent onRightClick;
        public UnityEvent onMiddleClick;
        
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                onLeftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                onMiddleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                onRightClick.Invoke();
        }
    }
}