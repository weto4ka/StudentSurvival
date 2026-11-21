using UnityEngine;
using System.Collections;
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
            if (player) transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }

        // call this again if the screen resolution or the "Orthographic Size" changes while the game is running
        public void CalculateBounds()
        {
            lastAspect = cam.aspect;
            lastSize = cam.orthographicSize;
            if (boundsMap == null) return;
            Bounds bounds = Camera2DBounds();
            min = bounds.max + boundsMap.bounds.min;
            max = bounds.min + boundsMap.bounds.max;
            // A viewport larger than the map should center on it, not invert the clamp.
            if (min.x > max.x) min.x = max.x = boundsMap.bounds.center.x;
            if (min.y > max.y) min.y = max.y = boundsMap.bounds.center.y;
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

        void Follow()
        {
            if (face == Mode.Player) direction = playerSprite != null && playerSprite.flipX ? Vector3.left : Vector3.right;
            else direction = (Mouse() - player.position).normalized;
            Vector3 position = player.position + direction * offset;
            position.z = -10.0F;
            position.y += 2.8F;
            // useBounds is the switch behind UseCameraBounds(); without a bounds sprite there is nothing to clamp to
            if (useBounds && boundsMap != null) position = MoveInside(position, new Vector3(min.x, min.y, position.z), new Vector3(max.x, max.y, position.z));
            transform.position = Vector3.Lerp(transform.position, position, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }

        void LateUpdate()
        {
            if (!Mathf.Approximately(lastAspect, cam.aspect) || !Mathf.Approximately(lastSize, cam.orthographicSize)) CalculateBounds();
            if (player)
            {
                Follow();
            }
        }
        private void OnDestroy() { if (_use == this) _use = null; }
    }
}
