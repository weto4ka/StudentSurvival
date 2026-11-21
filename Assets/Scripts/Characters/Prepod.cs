using System;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Characters
{
    public class Prepod : Monster
    {
        [SerializeField, Min(0.2f)] private float rate = 2f;
        [SerializeField, Min(1)] private int needLabs = 5;
        [SerializeField] private Color bulletColor = Color.white;
        private BulletOcenka bullet;
        private Character player;
        private Collider2D bodyCollider;
        private SpriteRenderer visual;
        private Color restingColor;
        private LineRenderer warning;
        private Material warningMaterial;
        private int remainingLabs;
        private bool defeated, finalExam, nextHigh, highAttack;
        private float timer = 1.2f, flashUntil;
        private int shotsRemaining;
        private Vector3 attackOrigin, attackDirection;
        public int RemainingLabs { get { return remainingLabs; } }
        public int RequiredLabs { get { return needLabs; } }
        public float ShotInterval { get { return rate; } }
        public bool IsTelegraphing { get; private set; }
        public string AttackWarningKey { get { return IsTelegraphing ? highAttack ? "combat.crouch" : "combat.jump" : string.Empty; } }
        public float TelegraphSecondsRemaining { get { return IsTelegraphing ? Mathf.Max(0f, timer) : 0f; } }
        public event Action<Prepod> Defeated;
        public event Action HealthChanged;
        public event Action AttackChanged;

        protected override void Awake()
        {
            remainingLabs = Mathf.Max(1, needLabs);
            bullet = Resources.Load<BulletOcenka>("BulletOcenka");
            player = FindFirstObjectByType<Character>();
            bodyCollider = GetComponent<Collider2D>();
            visual = GetComponentInChildren<SpriteRenderer>();
            restingColor = visual != null ? visual.color : Color.white;
            // Cloning the first teacher can already supply this child.
            Transform existing = transform.Find("Attack telegraph");
            warning = existing != null ? existing.GetComponent<LineRenderer>() : new GameObject("Attack telegraph").AddComponent<LineRenderer>();
            warning.transform.SetParent(transform, false);
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null) { warningMaterial = new Material(shader); warning.sharedMaterial = warningMaterial; }
            warning.useWorldSpace = true;
            warning.positionCount = 2;
            warning.startWidth = warning.endWidth = 0.045f;
            warning.sortingOrder = visual != null ? visual.sortingOrder + 1 : 1;
            if (visual != null) warning.sortingLayerID = visual.sortingLayerID;
            warning.enabled = false;
        }

        public void Configure(int labs, float shotInterval, Color color, bool isFinalExam = false)
        {
            needLabs = Mathf.Max(1, labs);
            rate = Mathf.Max(0.2f, shotInterval);
            bulletColor = color;
            finalExam = isFinalExam;
            if (finalExam)
            {
                ExaminerAppearance appearance = Resources.Load<ExaminerAppearance>("Combat/ExaminerAppearance");
                if (appearance != null) visual = appearance.Apply(gameObject, visual, bodyCollider as BoxCollider2D);
                restingColor = Color.white;
            }
            RestoreHealth(needLabs);
        }

        public void RestoreHealth(int health)
        {
            remainingLabs = Mathf.Clamp(health, 1, needLabs);
            defeated = false;
            nextHigh = false;
            shotsRemaining = 0;
            timer = 1.2f;
            EndWarning();
            HealthChanged?.Invoke();
        }

        protected override void Update()
        {
            if (visual != null) visual.color = Time.time < flashUntil ? new Color(1f, 0.35f, 0.3f) : restingColor;
            if (GameSession.BlocksGameplay || defeated || bullet == null || player == null) return;
            bool nearby = Mathf.Abs(player.transform.position.x - transform.position.x) <= 14f &&
                          Mathf.Abs(player.transform.position.y - transform.position.y) <= 4f;
            if (!nearby) { shotsRemaining = 0; EndWarning(); timer = 1.2f; return; }
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            if (IsTelegraphing)
            {
                EndWarning();
                shotsRemaining = finalExam ? 2 : 1;
                Fire();
            }
            else if (shotsRemaining > 0) Fire();
            else BeginWarning();
        }

        private void BeginWarning()
        {
            highAttack = finalExam && nextHigh;
            if (finalExam) nextHigh = !nextHigh;
            attackDirection = player.transform.position.x < transform.position.x ? Vector3.left : Vector3.right;
            float feet = bodyCollider != null ? bodyCollider.bounds.min.y : transform.position.y;
            attackOrigin = new Vector3(transform.position.x, feet + (highAttack ? 2.2f : 0.8f), transform.position.z);
            timer = finalExam ? 0.9f : 1f;
            IsTelegraphing = true;
            Color cue = highAttack ? new Color(0.3f, 0.85f, 1f, 0.65f) : new Color(1f, 0.8f, 0.2f, 0.65f);
            warning.startColor = warning.endColor = cue;
            warning.SetPosition(0, attackOrigin);
            warning.SetPosition(1, attackOrigin + attackDirection * 14f);
            warning.enabled = true;
            AttackChanged?.Invoke();
        }

        private void Fire()
        {
            BulletOcenka instance = Instantiate(bullet, attackOrigin, bullet.transform.rotation);
            instance.Parent = gameObject;
            instance.Direction = attackDirection;
            instance.Color = highAttack ? new Color(0.3f, 0.85f, 1f) : bulletColor;
            instance.ConfigureLane();
            shotsRemaining--;
            timer = shotsRemaining > 0 ? 0.18f : Mathf.Max(finalExam ? 1.4f : 1f, rate);
        }

        private void EndWarning()
        {
            bool wasVisible = IsTelegraphing;
            IsTelegraphing = false;
            if (warning != null) warning.enabled = false;
            if (wasVisible) AttackChanged?.Invoke();
        }

        public override void ReceiveDamage()
        {
            if (GameSession.BlocksGameplay || defeated) return;
            remainingLabs = Mathf.Max(0, remainingLabs - 1);
            flashUntil = Time.time + 0.16f;
            HealthChanged?.Invoke();
            if (remainingLabs > 0) { RunSaveService.RequestSave(); return; }
            defeated = true;
            shotsRemaining = 0;
            EndWarning();
            foreach (BulletOcenka grade in FindObjectsByType<BulletOcenka>(FindObjectsSortMode.None))
                if (grade.Parent == gameObject) { grade.gameObject.SetActive(false); Destroy(grade.gameObject); }
            if (Defeated != null) Defeated(this); else GameSession.Win();
            Destroy(gameObject);
        }

        protected override void OnTriggerEnter2D(Collider2D collider) { }
        private void OnDisable() { EndWarning(); }
        private void OnDestroy() { if (warningMaterial != null) Destroy(warningMaterial); }
    }
}
