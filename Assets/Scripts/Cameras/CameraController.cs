using UnityEngine;
using StudentSurvival.Controls;

namespace StudentSurvival.Cameras
{
    [RequireComponent(typeof(Camera))]

    public class CameraController : MonoBehaviour
    {

        private enum Mode { Player, Cursor };

        [SerializeField]
        private Mode face; // direction of the offset: where the character faces or where the cursor is
        [SerializeField]
        private float smooth = 2.5f; // smoothing while following the character
        [SerializeField]
        private float offset; // size of the offset (0 disables it)
        [SerializeField]
        private SpriteRenderer boundsMap; // sprite that limits where the camera can move
        [SerializeField]
        private bool useBounds = true; // whether the camera bounds are used

        private Transform player;
        private Vector3 min, max, direction;
        private static CameraController _use;
        private Camera cam;
        private SpriteRenderer playerSprite;
        private float lastAspect;
        private float lastSize;
        private CameraBoundsRegion region;
        private Vector3 previousPlayerPosition;
        private bool hasPreviousPlayerPosition, hasClamp;
        private int regionRevision;

        public static CameraController use
        {
            get { return _use; }
        }

        void Awake()
        {
            _use = this;
            cam = GetComponent<Camera>();
            cam.orthographic = true;
            FindPlayer();
            CalculateBounds();
        }

        // switch for use from another class
        public void UseCameraBounds(bool value)
        {
            useBounds = value;
        }

        // find the character again if it was destroyed and then respawned
        // to call it from another class: CameraController.use.FindPlayer();
        public void FindPlayer()
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            player = target != null ? target.transform : null;
            playerSprite = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
            hasPreviousPlayerPosition = false;
            if (player) transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }

        // call this again if the screen resolution or the "Orthographic Size" changes while the game is running
        public void CalculateBounds()
        {
            lastAspect = cam.aspect;
            lastSize = cam.orthographicSize;
            regionRevision = region != null ? region.Revision : 0;
            hasClamp = region != null || boundsMap != null;
            if (!hasClamp) return;
            SetClampLimits(region != null ? region.WorldBounds : boundsMap.bounds);
        }

        private void SetClampLimits(Bounds frame)
        {
            Bounds viewport = Camera2DBounds();
            min = viewport.max + frame.min;
            max = viewport.min + frame.max;
            // A viewport larger than the map should center on it, not invert the clamp.
            if (min.x > max.x) min.x = max.x = frame.center.x;
            if (min.y > max.y) min.y = max.y = frame.center.y;
        }

        Bounds Camera2DBounds()
        {
            float height = cam.orthographicSize * 2;
            return new Bounds(Vector3.zero, new Vector3(height * cam.aspect, height, 0));
        }

        Vector3 MoveInside(Vector3 current, Vector3 pMin, Vector3 pMax)
        {
            current = Vector3.Max(current, pMin);
            current = Vector3.Min(current, pMax);
            return current;
        }

        Vector3 Mouse()
        {
            Vector3 mouse = GameInput.PointerPosition;
            mouse.z = -transform.position.z;
            return cam.ScreenToWorldPoint(mouse);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            return useBounds && hasClamp ? MoveInside(position, new Vector3(min.x, min.y, position.z), new Vector3(max.x, max.y, position.z)) : position;
        }

        void Follow(bool snap)
        {
            if (face == Mode.Player) direction = playerSprite != null && playerSprite.flipX ? Vector3.left : Vector3.right;
            else direction = (Mouse() - player.position).normalized;
            Vector3 position = player.position + direction * offset;
            position.z = -10.0F;
            position.y += 2.8F;
            position = ClampPosition(position);
            transform.position = snap ? position : ClampPosition(Vector3.Lerp(transform.position, position, 1f - Mathf.Exp(-smooth * Time.deltaTime)));
        }

        void LateUpdate()
        {
            if (player)
            {
                CameraBoundsRegion next = CameraBoundsRegion.At(player.position);
                bool regionChanged = next != region;
                region = next;
                if (regionChanged || (region != null && region.Revision != regionRevision) || !Mathf.Approximately(lastAspect, cam.aspect) || !Mathf.Approximately(lastSize, cam.orthographicSize)) CalculateBounds();
                bool teleported = hasPreviousPlayerPosition && (player.position - previousPlayerPosition).sqrMagnitude > 100f;
                Follow(regionChanged || teleported || !hasPreviousPlayerPosition);
                previousPlayerPosition = player.position;
                hasPreviousPlayerPosition = true;
            }
        }
        private void OnDestroy() { if (_use == this) _use = null; }
    }
}
