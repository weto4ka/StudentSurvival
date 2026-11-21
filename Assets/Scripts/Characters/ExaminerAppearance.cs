using UnityEngine;

namespace StudentSurvival.Characters
{
    [CreateAssetMenu(menuName = "Student Survival/Examiner appearance")]
    public sealed class ExaminerAppearance : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        // Coordinates inside the sprite's source rectangle, measured from its bottom left.
        [SerializeField] private Rect visiblePixels = new Rect(239, 218, 122, 384);

        public SpriteRenderer Apply(GameObject owner, SpriteRenderer previous, BoxCollider2D body)
        {
            if (sprite == null || body == null || visiblePixels.height <= 0f) return previous;
            foreach (Animator animator in owner.GetComponentsInChildren<Animator>(true)) animator.enabled = false;
            Transform existing = owner.transform.Find("Final examiner artwork");
            SpriteRenderer renderer = existing != null ? existing.GetComponent<SpriteRenderer>() : null;
            if (renderer == null)
            {
                renderer = new GameObject("Final examiner artwork").AddComponent<SpriteRenderer>();
                renderer.transform.SetParent(owner.transform, false);
            }
            if (previous != null && previous != renderer)
            {
                renderer.sharedMaterial = previous.sharedMaterial;
                renderer.sortingLayerID = previous.sortingLayerID;
                renderer.sortingOrder = previous.sortingOrder;
                renderer.maskInteraction = previous.maskInteraction;
                previous.enabled = false;
            }
            renderer.sprite = sprite;
            renderer.flipX = renderer.flipY = false;
            renderer.color = Color.white;
            float pixelsPerUnit = sprite.pixelsPerUnit;
            float scale = body.size.y / (visiblePixels.height / pixelsPerUnit);
            float artFeet = (visiblePixels.yMin - sprite.pivot.y) / pixelsPerUnit;
            float artCenterX = (visiblePixels.center.x - sprite.pivot.x) / pixelsPerUnit;
            renderer.transform.localRotation = Quaternion.identity;
            renderer.transform.localScale = Vector3.one * scale;
            renderer.transform.localPosition = new Vector3(body.offset.x - artCenterX * scale,
                body.offset.y - body.size.y * 0.5f - artFeet * scale, 0);
            return renderer;
        }
    }
}
