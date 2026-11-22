using System;
using StudentSurvival.Controls;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StudentSurvival.Characters
{
    public enum CharState { Idle, Run, Jump }
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInteractor))]
    public partial class Character : Unit, IRunSaveParticipant
    {
        [SerializeField, Min(1)] private int sleep = 10;
        [SerializeField, Min(1)] private int intelligence = 5;
        [SerializeField, Min(0)] public float speed = 3f;
        [SerializeField, Min(0)] private float jumpForce = 15f;
        [SerializeField] private LayerMask groundLayers = Physics2D.DefaultRaycastLayers;
        [SerializeField, Min(0.01f)] private float groundDistance = 0.08f;
        [SerializeField, Range(0, 0.3f)] private float coyoteTime = 0.1f;
        [SerializeField, Range(0, 0.3f)] private float jumpBuffer = 0.12f;
        [SerializeField, Min(0)] private float damageGraceTime = 0.6f;
        [SerializeField, Range(0.35f, 0.8f)] private float crouchHeight = 0.75f;
        [SerializeField, Range(0.1f, 1f)] private float crouchSpeed = 0.45f;
        private BoxCollider2D stanceCollider;
        private Vector2 standingSize, standingOffset;
        private Vector3 safePosition;
        private bool safeCrouching;
        [SerializeField] private Sprite articulationSprite;
        private CrouchVisual crouchVisual;
        private Color normalColor = Color.white;
        public bool IsInvulnerable => Time.time < invulnerableUntil;
        public int RestoreOrder => 10;
        private readonly Collider2D[] headroomHits = new Collider2D[8];
        public bool IsCrouching { get; private set; }
        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[16];
        private ContactFilter2D groundFilter;
        private Rigidbody2D body;
        private Animator animator;
        private SpriteRenderer sprite;
        private BulletLab bullet;
        private bool suppressPointerUntilRelease;
        private float nextThrowAt;
        private const float ThrowInterval = .18f;
        private float horizontal;
        private bool airborne;
        private float lastGrounded = float.NegativeInfinity;
        private float jumpUntil = float.NegativeInfinity;
        private float invulnerableUntil;
        private int animationState = -1;
        private static readonly int StateId = Animator.StringToHash("State");
        public event Action StatsChanged;
        public int Sleep
        {
            get { return sleep; }
            set { SetSleep(value); }
        }
        public int Intelligence
        {
            get { return intelligence; }
            set { intelligence = Mathf.Max(0, value); StatsChanged?.Invoke(); RunSaveService.RequestSave(); if (intelligence == 0) GameSession.Lose("result.noIntelligence"); }
        }
        public bool FacingLeft { get { return sprite != null && sprite.flipX; } }
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sprite = GetComponentInChildren<SpriteRenderer>();
            crouchVisual = GetComponent<CrouchVisual>();
            if (crouchVisual == null) crouchVisual = gameObject.AddComponent<CrouchVisual>(); crouchVisual.Configure(sprite, articulationSprite);
            stanceCollider = GetComponent<BoxCollider2D>();
            if (stanceCollider != null) { standingSize = stanceCollider.size; standingOffset = stanceCollider.offset; }
            safePosition = transform.position;
            RunSaveService.PlayerRelocated += OnRelocated;
            if (sprite != null) normalColor = sprite.color;
            // RequireComponent does not retroactively add dependencies to old serialized scenes.
            if (GetComponent<PlayerInteractor>() == null) gameObject.AddComponent<PlayerInteractor>();
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.freezeRotation = true;
            groundFilter = new ContactFilter2D { useTriggers = false };
            groundFilter.SetLayerMask(groundLayers);
            bullet = Resources.Load<BulletLab>("BulletLab");
            if (bullet == null) Debug.LogError("Missing Resources/BulletLab prefab.", this);
        }
        private void Update()
        {
            if (GameSession.BlocksGameplay) { if (GameInput.PointerFireHeld) suppressPointerUntilRelease = true; horizontal = 0; jumpUntil = float.NegativeInfinity; return; }
            TickEnergy();
            if (GameSession.IsOver) return;
            horizontal = GameInput.Horizontal;
            if (GameInput.JumpPressed) jumpUntil = Time.time + jumpBuffer;
            if (!Mathf.Approximately(horizontal, 0) && sprite != null) sprite.flipX = horizontal < 0;
            if (!GameInput.PointerFireHeld) suppressPointerUntilRelease = false;
            bool pointerOverUI = GameInput.PointerFireHeld && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (pointerOverUI) suppressPointerUntilRelease = true;
            pointerOverUI |= suppressPointerUntilRelease;
            if (GameInput.FirePressed && !pointerOverUI && Sleep == 0) GameplayFeedback.Show("energy.noThrow");
            if (Sleep > 0 && !pointerOverUI && (GameInput.FirePressed || (GameInput.FireHeld && Time.time >= nextThrowAt))) { Shoot(); nextThrowAt = Time.time + ThrowInterval; }
        }
        private void FixedUpdate()
        {
            if (GameSession.BlocksGameplay) return;
            UpdateCrouch();
            bool grounded = CheckGround();
            if (grounded && body.linearVelocity.y <= 0.1f) { lastGrounded = Time.time; safePosition = new Vector3(body.position.x, body.position.y, transform.position.z); safeCrouching = IsCrouching; }
            Vector2 velocity = body.linearVelocity;
            float targetSpeed = horizontal * (speed + Sleep) * MovementMultiplier * (IsCrouching ? crouchSpeed : 1f);
            velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed, ref horizontalSmoothing, MovementResponse, Mathf.Infinity, Time.fixedDeltaTime);
            body.linearVelocity = velocity;
            if (!IsCrouching && Time.time <= jumpUntil && Time.time - lastGrounded <= coyoteTime)
            {
                velocity.y = 0;
                body.linearVelocity = velocity;
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpUntil = lastGrounded = float.NegativeInfinity;
                grounded = false;
            }
            airborne = !grounded;
            int nextState = (int)(grounded ? IsCrouching || Mathf.Approximately(horizontal, 0) ? CharState.Idle : CharState.Run : CharState.Jump);
            if (animator != null && nextState != animationState)
            { animator.SetInteger(StateId, nextState); animationState = nextState; }
        }
        private void UpdateCrouch()
        {
            if (stanceCollider == null) return;
            bool crouch = GameInput.CrouchHeld;
            if (!crouch && IsCrouching && !CanStand()) crouch = true;
            if (crouch != IsCrouching) ApplyStance(crouch);
        }
        private void ApplyStance(bool crouch)
        {
            IsCrouching = crouch;
            if (stanceCollider == null) return;
            float ratio = crouch ? crouchHeight : 1f;
            stanceCollider.size = new Vector2(standingSize.x, standingSize.y * ratio);
            stanceCollider.offset = new Vector2(standingOffset.x, standingOffset.y - standingSize.y * (1f - ratio) * 0.5f);

        }
        private void LateUpdate()
        {
            if (crouchVisual != null) crouchVisual.SetState(IsCrouching, GameSession.BlocksGameplay ? 0 : horizontal, airborne, body != null ? body.linearVelocity.y : 0);
            if (sprite == null) return;
            sprite.color = IsInvulnerable ? new Color(1, 0.45f, 0.35f, Mathf.PingPong(Time.time * 12, 0.6f) + 0.4f) : normalColor;
        }
        public void Capture(RunSnapshot snapshot)
        {
            snapshot.playerPosition = safePosition; snapshot.crouching = safeCrouching; snapshot.sleep = sleep; snapshot.intelligence = intelligence;
            snapshot.baseSpeed = speed; snapshot.facingLeft = FacingLeft;
            CaptureEnergy(snapshot);
        }
        public void Restore(RunSnapshot snapshot)
        {
            sleep = Mathf.Max(0, snapshot.sleep); intelligence = Mathf.Max(snapshot.semesterCompleted ? 0 : 1, snapshot.intelligence); speed = Mathf.Max(0, snapshot.baseSpeed);
            RestoreEnergy(snapshot); airborne = false;
            safePosition = snapshot.playerPosition; body.position = safePosition; transform.position = safePosition; body.linearVelocity = Vector2.zero;
            safeCrouching = snapshot.crouching;
            ApplyStance(snapshot.crouching);
            if (sprite != null) sprite.flipX = snapshot.facingLeft;
            invulnerableUntil = Time.time + 1.25f; horizontal = 0; jumpUntil = lastGrounded = float.NegativeInfinity;
            StatsChanged?.Invoke();
        }
        private void OnRelocated(Vector3 position) { safePosition = position; safeCrouching = IsCrouching; }
        private void OnDestroy() { RunSaveService.PlayerRelocated -= OnRelocated; }
        private bool CanStand()
        {
            float addedHeight = standingSize.y * (1f - crouchHeight);
            Vector2 localCenter = standingOffset + Vector2.up * (standingSize.y - addedHeight) * 0.5f;
            Vector2 scale = transform.lossyScale;
            Vector2 size = new Vector2(standingSize.x * Mathf.Abs(scale.x) * 0.94f, addedHeight * Mathf.Abs(scale.y) * 0.96f);
            int count = Physics2D.OverlapBox(transform.TransformPoint(localCenter), size, transform.eulerAngles.z, groundFilter, headroomHits);
            for (int i = 0; i < count; i++)
                if (headroomHits[i] != null && headroomHits[i].attachedRigidbody != body) return false;
            return true;
        }
        private bool CheckGround()
        {
            int count = body.Cast(Vector2.down, groundFilter, groundHits, groundDistance);
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = groundHits[i];
                if (hit.normal.y < 0.65f || hit.collider == null || hit.collider.isTrigger) continue;
                if (hit.collider.GetComponentInParent<PlayerInteractionTarget>() != null ||
                    hit.collider.GetComponentInParent<Unit>() != null ||
                    hit.collider.GetComponentInParent<BulletLab>() != null ||
                    hit.collider.GetComponentInParent<BulletOcenka>() != null) continue;
                return true;
            }
            return false;
        }
        private void Shoot()
        {
            if (Sleep == 0) { GameplayFeedback.Show("energy.noThrow"); return; }
            if (SpawnLab()) Sleep--;
        }
        public bool ThrowLab() { return !GameSession.BlocksGameplay && SpawnLab(); }
        private bool SpawnLab()
        {
            if (bullet == null) { GameplayFeedback.Show("feedback.noProjectile"); return false; }
            BulletLab instance = Instantiate(bullet, transform.position + Vector3.up * 1.5f, bullet.transform.rotation);
            instance.Parent = gameObject;
            instance.Direction = FacingLeft ? Vector3.left : Vector3.right;
            return true;
        }
        public override void ReceiveDamage()
        {
            if (GameSession.BlocksGameplay || Time.time < invulnerableUntil) return;
            invulnerableUntil = Time.time + damageGraceTime;
            Intelligence--;
            GameplayFeedback.Show("feedback.damage");
            if (!GameSession.IsOver) body.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
        }
    }
}
