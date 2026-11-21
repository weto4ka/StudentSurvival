using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    public sealed class UIControlSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ICancelHandler
    {
        public Action Cancel;
        private Selectable control;
        private void Awake() { control = GetComponent<Selectable>(); }
        private bool Available { get { return control != null && control.IsInteractable(); } }
        public void OnPointerEnter(PointerEventData data) { if (Available) UIAudio.Hover(); }
        public void OnSelect(BaseEventData data) { if (Available) UIAudio.Hover(); }
        public void OnPointerClick(PointerEventData data) { if (Available && !(control is Button)) UIAudio.Click(); }
        public void OnCancel(BaseEventData data) { Cancel?.Invoke(); }
    }
}
