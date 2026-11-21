using System;
using StudentSurvival.Controls;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StudentSurvival.Characters
{
    public enum CharState { Idle, Run, Jump }
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInteractor))]
    public class Character : Unit
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
        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[16];
        private ContactFilter2D groundFilter;
        private Rigidbody2D body;
        private Animator animator;
        private SpriteRenderer sprite;
        private BulletLab bullet;
        private float horizontal;
        private float lastGrounded = float.NegativeInfinity;
        private float jumpUntil = float.NegativeInfinity;
        private float invulnerableUntil;
        private int animationState = -1;
        private static readonly int StateId = Animator.StringToHash("State");
        public event Action StatsChanged;
        public int Sleep
        {
            get { return sleep; }
            set { sleep = Mathf.Max(0, value); StatsChanged?.Invoke(); if (sleep == 0) GameSession.Lose("result.noSleep"); }
        }
        public int Intelligence
        {
            get { return intelligence; }
            set { intelligence = Mathf.Max(0, value); StatsChanged?.Invoke(); if (intelligence == 0) GameSession.Lose("result.noIntelligence"); }
        }
        public bool FacingLeft { get { return sprite != null && sprite.flipX; } }
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sprite = GetComponentInChildren<SpriteRenderer>();
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
            if (GameSession.BlocksGameplay) { horizontal = 0; jumpUntil = float.NegativeInfinity; return; }
            horizontal = GameInput.Horizontal;
            if (GameInput.JumpPressed) jumpUntil = Time.time + jumpBuffer;
            if (!Mathf.Approximately(horizontal, 0) && sprite != null) sprite.flipX = horizontal < 0;
            bool pointerOverUI = GameInput.PointerFirePressed && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (GameInput.FirePressed && !pointerOverUI) Shoot();
        }
        private void FixedUpdate()
        {
            if (GameSession.BlocksGameplay) return;
            bool grounded = CheckGround();
            if (grounded && body.linearVelocity.y <= 0.1f) lastGrounded = Time.time;
            Vector2 velocity = body.linearVelocity;
            velocity.x = horizontal * (speed + Sleep);
            body.linearVelocity = velocity;
            if (Time.time <= jumpUntil && Time.time - lastGrounded <= coyoteTime)
            {
                velocity.y = 0;
                body.linearVelocity = velocity;
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpUntil = lastGrounded = float.NegativeInfinity;
                grounded = false;
            }
            int nextState = (int)(grounded ? Mathf.Approximately(horizontal, 0) ? CharState.Idle : CharState.Run : CharState.Jump);
            if (animator != null && nextState != animationState)
            { animator.SetInteger(StateId, nextState); animationState = nextState; }
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
        private void Shoot() { if (SpawnLab()) Sleep--; }
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
