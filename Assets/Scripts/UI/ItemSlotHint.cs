using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StudentSurvival.UI
{
    public sealed class ItemSlotHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public Action<bool> Hover;
        public void OnPointerEnter(PointerEventData data) { Hover?.Invoke(true); }
        public void OnPointerExit(PointerEventData data) { Hover?.Invoke(false); }
        public void OnSelect(BaseEventData data) { Hover?.Invoke(true); }
        public void OnDeselect(BaseEventData data) { Hover?.Invoke(false); }
    }
}
