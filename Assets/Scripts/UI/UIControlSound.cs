using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    public sealed class UIControlSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ICancelHandler
    {
        public Action Cancel;
        private Selectable control;
        private GameObject focusBorder;
        private bool hovered, selected, wasAvailable;
        public void ConfigureFocus(Color color)
        {
            if (control == null) control = GetComponent<Selectable>();
            if (control == null || control.targetGraphic == null || focusBorder != null) return;
            var root = new GameObject("Focus border", typeof(RectTransform));
            var rect = root.GetComponent<RectTransform>(); rect.SetParent(control.targetGraphic.transform, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            focusBorder = root;
            // Separate graphics keep the border bright instead of multiplying it by the button's dark tint.
            Edge(rect, new Vector2(0, 1), Vector2.one, new Vector2(.5f, 1), new Vector2(0, 3), color);
            Edge(rect, Vector2.zero, new Vector2(1, 0), new Vector2(.5f, 0), new Vector2(0, 3), color);
            Edge(rect, Vector2.zero, new Vector2(0, 1), new Vector2(0, .5f), new Vector2(3, 0), color);
            Edge(rect, new Vector2(1, 0), Vector2.one, new Vector2(1, .5f), new Vector2(3, 0), color);
            RefreshFocus();
        }
        private static void Edge(Transform parent, Vector2 min, Vector2 max, Vector2 pivot, Vector2 size, Color color)
        {
            var node = new GameObject("Pixel border", typeof(RectTransform), typeof(Image));
            var rect = node.GetComponent<RectTransform>(); rect.SetParent(parent, false);
            rect.anchorMin = min; rect.anchorMax = max; rect.pivot = pivot; rect.anchoredPosition = Vector2.zero; rect.sizeDelta = size;
            Image image = node.GetComponent<Image>(); image.color = color; image.raycastTarget = false;
        }
        private void RefreshFocus() { if (focusBorder != null) focusBorder.SetActive(Available && (hovered || selected)); }
        private void Update() { if (wasAvailable != Available) { wasAvailable = Available; RefreshFocus(); } }
        private void OnDisable() { hovered = selected = false; RefreshFocus(); }
        private void Awake()
        {
            control = GetComponent<Selectable>();
            // Dropdown clones inherit the template's graphics, but private transient references are not copied.
            if (control != null && control.targetGraphic != null)
            {
                Transform border = control.targetGraphic.transform.Find("Focus border");
                if (border != null) focusBorder = border.gameObject;
            }
            RefreshFocus();
        }
        private bool Available { get { return control != null && control.IsInteractable(); } }
        public void OnPointerEnter(PointerEventData data)
        {
            hovered = true;
            if (Available) { control.Select(); UIAudio.Hover(); }
            RefreshFocus();
        }
        public void OnPointerExit(PointerEventData data) { hovered = false; RefreshFocus(); }
        public void OnSelect(BaseEventData data) { selected = true; if (Available) UIAudio.Hover(); RefreshFocus(); }
        public void OnDeselect(BaseEventData data) { selected = hovered = false; RefreshFocus(); }
        public void OnPointerClick(PointerEventData data) { if (Available && !(control is Button)) UIAudio.Click(); }
        public void OnCancel(BaseEventData data) { Cancel?.Invoke(); }
    }
}
