using UnityEngine;

namespace StudentSurvival.Characters
{
    /// <summary>Articulates the original student artwork without squashing its proportions.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class CrouchVisual : MonoBehaviour
    {
        private SpriteRenderer source;
        private Transform pose;
        private readonly SpriteRenderer[] parts = new SpriteRenderer[6];
        private readonly Sprite[] slices = new Sprite[6];
        private bool crouching, previousForceRenderingOff;
        private float movement, phase, unit, baseline;

        public void Configure(SpriteRenderer renderer)
        {
            if (source != null || renderer == null || renderer.sprite == null) return;
            source = renderer;
            previousForceRenderingOff = source.forceRenderingOff;
            Sprite sprite = source.sprite;
            unit = sprite.rect.height / (150f * sprite.pixelsPerUnit);
            baseline = -sprite.pivot.y / sprite.pixelsPerUnit;
            pose = new GameObject("Crouch pose").transform;
            pose.SetParent(source.transform, false);
            // Pixel coordinates refer to the original 76 × 150 idle PSD, including its padding.
            CreatePart(0, "Far thigh", new Rect(20, 30, 18, 28), new Vector2(.5f, 1));
            CreatePart(1, "Far shin", new Rect(20, 0, 18, 30), new Vector2(.5f, 1));
            CreatePart(2, "Near thigh", new Rect(38, 30, 20, 28), new Vector2(.5f, 1));
            CreatePart(3, "Near shin", new Rect(38, 0, 20, 30), new Vector2(.5f, 1));
            CreatePart(4, "Torso", new Rect(0, 58, 76, 50), new Vector2(.5f, 0));
            CreatePart(5, "Head", new Rect(0, 108, 76, 42), new Vector2(.5f, 0));
            pose.gameObject.SetActive(false);
        }

        private void CreatePart(int index, string label, Rect pixelRect, Vector2 pivot)
        {
            Sprite original = source.sprite;
            Rect r = original.rect;
            Rect slice = new Rect(r.x + pixelRect.x / 76f * r.width,
                r.y + pixelRect.y / 150f * r.height,
                pixelRect.width / 76f * r.width, pixelRect.height / 150f * r.height);
            slices[index] = Sprite.Create(original.texture, slice, pivot,
                original.pixelsPerUnit, 0, SpriteMeshType.FullRect);
            slices[index].name = "Student crouch " + label;
            var child = new GameObject(label);
            child.transform.SetParent(pose, false);
            parts[index] = child.AddComponent<SpriteRenderer>();
            parts[index].sprite = slices[index];
        }

        public void SetState(bool isCrouching, float horizontalMovement)
        {
            crouching = isCrouching;
            movement = Mathf.Abs(horizontalMovement);
        }

        private void LateUpdate()
        {
            if (source == null || pose == null) return;
            pose.gameObject.SetActive(crouching && source.enabled);
            source.forceRenderingOff = crouching || previousForceRenderingOff;
            if (!crouching) return;
            if (movement > .05f) phase += Time.deltaTime * 9f;
            float stride = movement > .05f ? Mathf.Sin(phase) : 0;
            float breath = Mathf.Sin(Time.time * 2.5f) * .45f;
            // Mirroring belongs to the articulated root; individual crops must keep their pivots.
            pose.localScale = new Vector3(source.flipX ? -1 : 1, source.flipY ? -1 : 1, 1);
            pose.localPosition = new Vector3(0, baseline, 0);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].color = source.color;
                parts[i].sharedMaterial = source.sharedMaterial;
                parts[i].sortingLayerID = source.sortingLayerID;
                parts[i].sortingOrder = source.sortingOrder;
                parts[i].maskInteraction = source.maskInteraction;
            }
            Vector2 hip = new Vector2(0, 16 + Mathf.Abs(stride) * 1.5f + breath);
            Leg(0, 1, hip + Vector2.left * 5, -75 + stride * 7, 78 - stride * 7);
            Leg(2, 3, hip + Vector2.right * 5, 75 + stride * 7, -78 - stride * 7);
            float lean = -55 + stride * 2;
            Place(4, hip, lean);
            Vector2 neck = hip + Rotate(new Vector2(0, 50), lean);
            Place(5, neck, -4 + stride);
        }

        private void Leg(int thigh, int shin, Vector2 hip, float upperAngle, float lowerAngle)
        {
            Place(thigh, hip, upperAngle);
            Place(shin, hip + Rotate(new Vector2(0, -28), upperAngle), lowerAngle);
        }

        private void Place(int index, Vector2 pixels, float angle)
        {
            parts[index].transform.localPosition = pixels * unit;
            parts[index].transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        private static Vector2 Rotate(Vector2 value, float degrees)
        {
            float a = degrees * Mathf.Deg2Rad;
            return new Vector2(value.x * Mathf.Cos(a) - value.y * Mathf.Sin(a),
                value.x * Mathf.Sin(a) + value.y * Mathf.Cos(a));
        }

        private void OnDisable()
        {
            if (source != null) source.forceRenderingOff = previousForceRenderingOff;
            if (pose != null) pose.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (source != null) source.forceRenderingOff = previousForceRenderingOff;
            if (pose != null) Destroy(pose.gameObject);
            foreach (Sprite sprite in slices) if (sprite != null) Destroy(sprite);
        }
    }
}
