using UnityEngine;

namespace StudentSurvival.Characters
{
    /// <summary>Articulates the original student artwork without squashing its proportions.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class CrouchVisual : MonoBehaviour
    {
        private SpriteRenderer source;
        private Sprite referenceSprite;
        private Transform pose;
        private readonly SpriteRenderer[] parts = new SpriteRenderer[6];
        private readonly Sprite[] slices = new Sprite[6];
        private bool crouching, airborne, wasAirborne, previousForceRenderingOff;
        private float movement, verticalVelocity, phase, unit, baseline, landingRemaining;
        private PoseState displayed = PoseState.Standing;

        private struct PoseState
        {
            public float hipHeight, hipSpacing, farThigh, farShin, nearThigh, nearShin, lean, head;
            public static PoseState Standing => new PoseState { hipHeight = 58, hipSpacing = 9 };
            public static PoseState Blend(PoseState a, PoseState b, float t)
            {
                return new PoseState
                {
                    hipHeight = Mathf.Lerp(a.hipHeight, b.hipHeight, t),
                    hipSpacing = Mathf.Lerp(a.hipSpacing, b.hipSpacing, t),
                    farThigh = Mathf.Lerp(a.farThigh, b.farThigh, t),
                    farShin = Mathf.Lerp(a.farShin, b.farShin, t),
                    nearThigh = Mathf.Lerp(a.nearThigh, b.nearThigh, t),
                    nearShin = Mathf.Lerp(a.nearShin, b.nearShin, t),
                    lean = Mathf.Lerp(a.lean, b.lean, t), head = Mathf.Lerp(a.head, b.head, t)
                };
            }
        }

        public void Configure(SpriteRenderer renderer, Sprite originalPixelArtwork = null)
        {
            if (source != null || renderer == null) return;
            Sprite artwork = originalPixelArtwork != null ? originalPixelArtwork : renderer.sprite;
            if (artwork == null) return;
            // Keep one explicit pixel-art source even while Animator changes the visible frame.
            referenceSprite = artwork;
            source = renderer;
            previousForceRenderingOff = source.forceRenderingOff;
            Sprite sprite = referenceSprite;
            unit = sprite.rect.height / (150f * sprite.pixelsPerUnit);
            baseline = -sprite.pivot.y / sprite.pixelsPerUnit;
            pose = new GameObject("Articulated student pose").transform;
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
            Sprite original = referenceSprite;
            Rect r = original.rect;
            Rect slice = new Rect(r.x + pixelRect.x / 76f * r.width,
                r.y + pixelRect.y / 150f * r.height,
                pixelRect.width / 76f * r.width, pixelRect.height / 150f * r.height);
            slices[index] = Sprite.Create(original.texture, slice, pivot,
                original.pixelsPerUnit, 0, SpriteMeshType.FullRect);
            slices[index].name = "Student articulated " + label;
            var child = new GameObject(label);
            child.transform.SetParent(pose, false);
            parts[index] = child.AddComponent<SpriteRenderer>();
            parts[index].sprite = slices[index];
        }

        public void SetState(bool isCrouching, float horizontalMovement)
        {
            SetState(isCrouching, horizontalMovement, false, 0);
        }

        public void SetState(bool isCrouching, float horizontalMovement, bool isAirborne, float yVelocity)
        {
            crouching = isCrouching;
            movement = Mathf.Abs(horizontalMovement);
            airborne = isAirborne;
            verticalVelocity = yVelocity;
        }

        private void LateUpdate()
        {
            if (source == null || pose == null) return;
            if (wasAirborne && !airborne) landingRemaining = .24f;
            wasAirborne = airborne;
            if (airborne) landingRemaining = 0;
            bool articulated = crouching || airborne || landingRemaining > 0;
            pose.gameObject.SetActive(articulated && source.enabled);
            source.forceRenderingOff = articulated || previousForceRenderingOff;
            if (!articulated) { displayed = PoseState.Standing; return; }
            if (crouching && !airborne && movement > .05f) phase += Time.deltaTime * 9f;
            float stride = movement > .05f ? Mathf.Sin(phase) : 0;
            float breath = Mathf.Sin(Time.time * 2.5f) * .45f;
            PoseState target;
            if (airborne)
            {
                // Both knees tuck forward on ascent; lower legs fold back at the knee.
                PoseState rising = new PoseState { hipHeight = 58, hipSpacing = 7,
                    farThigh = 38, farShin = -65, nearThigh = 62, nearShin = -75, lean = -8, head = 2 };
                PoseState falling = new PoseState { hipHeight = 58, hipSpacing = 8,
                    farThigh = 15, farShin = -35, nearThigh = 30, nearShin = -40, lean = 3, head = -2 };
                // Extend gradually around the apex, before contact with the floor.
                target = PoseState.Blend(falling, rising, Mathf.InverseLerp(-2f, 2f, verticalVelocity));
            }
            else if (crouching)
                target = new PoseState { hipHeight = 16 + Mathf.Abs(stride) * 1.5f + breath, hipSpacing = 5,
                    farThigh = -75 + stride * 7, farShin = 78 - stride * 7,
                    nearThigh = 75 + stride * 7, nearShin = -78 - stride * 7,
                    lean = -55 + stride * 2, head = -4 + stride };
            else
            {
                PoseState landing = new PoseState { hipHeight = 44, hipSpacing = 9,
                    farThigh = -40, farShin = 42, nearThigh = 40, nearShin = -42, lean = -8 };
                float recovery = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.17f, 0, landingRemaining));
                target = PoseState.Blend(landing, PoseState.Standing, recovery);
            }
            displayed = PoseState.Blend(displayed, target, 1 - Mathf.Exp(-24f * Time.deltaTime));
            landingRemaining = Mathf.Max(0, landingRemaining - Time.deltaTime);
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
            Vector2 hip = new Vector2(0, displayed.hipHeight);
            Leg(0, 1, hip + Vector2.left * displayed.hipSpacing, displayed.farThigh, displayed.farShin);
            Leg(2, 3, hip + Vector2.right * displayed.hipSpacing, displayed.nearThigh, displayed.nearShin);
            Place(4, hip, displayed.lean);
            Vector2 neck = hip + Rotate(new Vector2(0, 50), displayed.lean);
            Place(5, neck, displayed.head);
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
