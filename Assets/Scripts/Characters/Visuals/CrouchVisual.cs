using UnityEngine;
using UnityEngine.Rendering;

namespace StudentSurvival.Characters
{
    /// <summary>Measured, overlapping joints from the original student PSD. No texture or body scaling.</summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(100)]
    public sealed class CrouchVisual : MonoBehaviour
    {
        private const int UpperBody = 0, FarThigh = 1, FarShin = 2, FarFoot = 3,
            NearThigh = 4, NearShin = 5, NearFoot = 6;
        // Source pixel coordinates use the PSD's bottom-left origin. The shirt/pelvis
        // is continuous down to y44; thighs overlap it through y58, not a butt joint.
        private static readonly Vector2 Waist = new Vector2(38, 64);
        private static readonly Vector2 FarHip = new Vector2(32, 52), NearHip = new Vector2(48, 52);
        private static readonly Vector2 FarKnee = new Vector2(30, 28), NearKnee = new Vector2(48, 28);
        private static readonly Vector2 FarAnkle = new Vector2(30, 8), NearAnkle = new Vector2(48, 8);
        private SpriteRenderer source;
        private Sprite referenceSprite;
        private Transform pose;
        private SortingGroup poseSorting;
        private readonly SpriteRenderer[] parts = new SpriteRenderer[7];
        private readonly Sprite[] slices = new Sprite[7];
        private bool crouching, airborne, wasAirborne, wasCrouching, previousForceRenderingOff;
        private float movement, verticalVelocity, phase, unit, baseline, landingRemaining, standingRemaining;
        private PoseState displayed;

        private struct PoseState
        {
            // All limb angles are absolute in the facing-right pose. Child rotations
            // subtract their parent's angle, so each shared seam follows the parent.
            public float lean, farThigh, farShin, nearThigh, nearShin, feet;
            public static PoseState Blend(PoseState a, PoseState b, float t)
            {
                return new PoseState {
                    lean = Mathf.Lerp(a.lean, b.lean, t), farThigh = Mathf.Lerp(a.farThigh, b.farThigh, t),
                    farShin = Mathf.Lerp(a.farShin, b.farShin, t), nearThigh = Mathf.Lerp(a.nearThigh, b.nearThigh, t),
                    nearShin = Mathf.Lerp(a.nearShin, b.nearShin, t), feet = Mathf.Lerp(a.feet, b.feet, t) };
            }
        }

        public void Configure(SpriteRenderer renderer, Sprite originalPixelArtwork = null)
        {
            if (source != null || renderer == null) return;
            referenceSprite = originalPixelArtwork != null ? originalPixelArtwork : renderer.sprite;
            if (referenceSprite == null) return;
            referenceSprite.texture.filterMode = FilterMode.Point;
            source = renderer;
            previousForceRenderingOff = source.forceRenderingOff;
            unit = referenceSprite.rect.height / (150f * referenceSprite.pixelsPerUnit);
            baseline = -referenceSprite.pivot.y / referenceSprite.pixelsPerUnit;
            pose = new GameObject("Articulated student pose").transform;
            pose.SetParent(source.transform, false);
            poseSorting = pose.gameObject.AddComponent<SortingGroup>();
            CreatePart(UpperBody, "Intact head torso arms pelvis", new Rect(12, 44, 44, 106), Waist, pose);
            CreatePart(FarThigh, "Far thigh", new Rect(24, 24, 18, 34), FarHip, parts[UpperBody].transform);
            CreatePart(FarShin, "Far shin", new Rect(24, 8, 12, 22), FarKnee, parts[FarThigh].transform);
            CreatePart(FarFoot, "Far shoe", new Rect(24, 0, 12, 12), FarAnkle, parts[FarShin].transform);
            CreatePart(NearThigh, "Near thigh", new Rect(44, 24, 8, 34), NearHip, parts[UpperBody].transform);
            CreatePart(NearShin, "Near shin", new Rect(44, 8, 8, 22), NearKnee, parts[NearThigh].transform);
            CreatePart(NearFoot, "Near shoe", new Rect(44, 0, 12, 12), NearAnkle, parts[NearShin].transform);
            pose.gameObject.SetActive(false);
        }

        private void CreatePart(int index, string label, Rect pixels, Vector2 joint, Transform parent)
        {
            Rect r = referenceSprite.rect;
            Rect slice = new Rect(r.x + pixels.x / 76f * r.width, r.y + pixels.y / 150f * r.height,
                pixels.width / 76f * r.width, pixels.height / 150f * r.height);
            Vector2 pivot = new Vector2((joint.x - pixels.x) / pixels.width, (joint.y - pixels.y) / pixels.height);
            slices[index] = Sprite.Create(referenceSprite.texture, slice, pivot,
                referenceSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);
            slices[index].name = "Student articulated " + label;
            var child = new GameObject(label);
            child.transform.SetParent(parent, false);
            parts[index] = child.AddComponent<SpriteRenderer>();
            parts[index].sprite = slices[index];
        }

        public void SetState(bool isCrouching, float horizontalMovement)
        { SetState(isCrouching, horizontalMovement, false, 0); }

        public void SetState(bool isCrouching, float horizontalMovement, bool isAirborne, float yVelocity)
        {
            crouching = isCrouching; movement = Mathf.Abs(horizontalMovement);
            airborne = isAirborne; verticalVelocity = yVelocity;
        }

        private void LateUpdate()
        {
            if (source == null || pose == null) return;
            if (wasCrouching && !crouching && !airborne) standingRemaining = .18f;
            wasCrouching = crouching;
            if (wasAirborne && !airborne) landingRemaining = .24f;
            wasAirborne = airborne;
            if (airborne) landingRemaining = 0;
            if (airborne || crouching) standingRemaining = 0;
            bool articulated = crouching || airborne || landingRemaining > 0 || standingRemaining > 0;
            pose.gameObject.SetActive(articulated && source.enabled);
            source.forceRenderingOff = articulated || previousForceRenderingOff;
            if (!articulated) { displayed = default; return; }
            if (crouching && !airborne && movement > .05f) phase += Time.deltaTime * 9f;
            float stride = movement > .05f ? Mathf.Sin(phase) : 0;
            PoseState target;
            if (airborne)
            {
                PoseState rising = new PoseState { lean = -6, farThigh = 50, farShin = -80,
                    nearThigh = 65, nearShin = -85, feet = -10 };
                PoseState falling = new PoseState { lean = 3, farThigh = 25, farShin = -30,
                    nearThigh = 38, nearShin = -42 };
                target = PoseState.Blend(falling, rising, Mathf.InverseLerp(-2, 2, verticalVelocity));
            }
            else if (crouching)
                // Both knees point forward. Separate shoes counter-rotate to stay flat.
                target = new PoseState { lean = -15, farThigh = 74 + stride * 2, farShin = -74 - stride * 2,
                    nearThigh = 77 - stride * 2, nearShin = -77 + stride * 2 };
            else
            {
                PoseState landing = new PoseState { lean = -10, farThigh = 35, farShin = -35,
                    nearThigh = 40, nearShin = -40 };
                target = PoseState.Blend(landing, default,
                    Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.17f, 0, landingRemaining)));
            }
            displayed = PoseState.Blend(displayed, target, 1 - Mathf.Exp(-24 * Time.deltaTime));
            landingRemaining = Mathf.Max(0, landingRemaining - Time.deltaTime);
            standingRemaining = Mathf.Max(0, standingRemaining - Time.deltaTime);
            pose.localScale = new Vector3(source.flipX ? -1 : 1, source.flipY ? -1 : 1, 1);
            // The sorting group keeps every limb at the original character's scene
            // order. Internal overlaps must not send the legs behind scenery.
            pose.localPosition = Vector3.zero;
            poseSorting.sortingLayerID = source.sortingLayerID;
            poseSorting.sortingOrder = source.sortingOrder;
            // Keep overlapping pants behind the intact upper body, independent of hierarchy traversal.
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].color = source.color; parts[i].sharedMaterial = source.sharedMaterial;
                parts[i].sortingLayerID = source.sortingLayerID;
                parts[i].sortingOrder = i == UpperBody ? 2 : i <= FarFoot ? 0 : 1;
                parts[i].maskInteraction = source.maskInteraction;
            }
            float height = airborne ? Waist.y : GroundedWaistHeight(displayed);
            SetJoint(UpperBody, new Vector2(0, height), displayed.lean);
            parts[UpperBody].transform.localPosition += Vector3.up * baseline;
            SetLeg(FarThigh, FarShin, FarFoot, FarHip, FarKnee, FarAnkle,
                displayed.farThigh, displayed.farShin, displayed.feet);
            SetLeg(NearThigh, NearShin, NearFoot, NearHip, NearKnee, NearAnkle,
                displayed.nearThigh, displayed.nearShin, displayed.feet);
        }

        private void SetLeg(int thigh, int shin, int foot, Vector2 hip, Vector2 knee, Vector2 ankle,
            float upperAngle, float lowerAngle, float footAngle)
        {
            SetJoint(thigh, hip - Waist, upperAngle - displayed.lean);
            SetJoint(shin, knee - hip, lowerAngle - upperAngle);
            SetJoint(foot, ankle - knee, footAngle - lowerAngle);
        }

        private static float GroundedWaistHeight(PoseState state)
        {
            float far = FootBottom(FarHip, FarKnee, FarAnkle, state.lean, state.farThigh, state.farShin, state.feet, -6, 6);
            float near = FootBottom(NearHip, NearKnee, NearAnkle, state.lean, state.nearThigh, state.nearShin, state.feet, -4, 8);
            return -Mathf.Min(far, near);
        }

        private static float FootBottom(Vector2 hip, Vector2 knee, Vector2 ankle,
            float lean, float thigh, float shin, float foot, float left, float right)
        {
            Vector2 joint = Rotate(hip - Waist, lean) + Rotate(knee - hip, thigh) + Rotate(ankle - knee, shin);
            return joint.y + Mathf.Min(Rotate(new Vector2(left, -8), foot).y, Rotate(new Vector2(right, -8), foot).y);
        }

        private void SetJoint(int index, Vector2 pixels, float localAngle)
        {
            parts[index].transform.localPosition = pixels * unit;
            parts[index].transform.localRotation = Quaternion.Euler(0, 0, localAngle);
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
