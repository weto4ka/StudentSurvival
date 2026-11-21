using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    public sealed class ScrollSelectionIntoView : MonoBehaviour, ISelectHandler
    {
        public ScrollRect Scroll;
        public void OnSelect(BaseEventData data)
        {
            if (Scroll == null || Scroll.content == null) return;
            Canvas.ForceUpdateCanvases();
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(Scroll.viewport, transform);
            Rect view = Scroll.viewport.rect;
            float change = bounds.min.y < view.yMin ? view.yMin - bounds.min.y : bounds.max.y > view.yMax ? view.yMax - bounds.max.y : 0;
            Vector2 position = Scroll.content.anchoredPosition;
            position.y = Mathf.Clamp(position.y + change, 0, Mathf.Max(0, Scroll.content.rect.height - view.height));
            Scroll.content.anchoredPosition = position;
        }
    }
}
