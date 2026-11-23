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
        private Transform warning;
        private Sprite warningPixel;
        private int remainingLabs;
        private bool defeated, finalExam, nextHigh, highAttack;
        private bool examActive;
        private int encounterStage = 1;
        private int appearanceStage;
        private const float OpeningDelay = 0.6f;
        private float timer = OpeningDelay, flashUntil, damageProtectedUntil;
        private int shotsRemaining;
        private Vector3 attackOrigin, attackDirection;
        // Segmented pixel bar above the head: one cell per hit the exam still needs.
        private Transform healthBar;
        private SpriteRenderer healthBack;
        private SpriteRenderer[] healthCells = new SpriteRenderer[0];
        private const float CellWidth = .22f, CellGap = .06f, CellHeight = .14f, BarMaxWidth = 1.9f, BarBorder = .05f;
        private static readonly Color CellFull = new Color(1f, .42f, .32f), CellEmpty = new Color(.16f, .16f, .2f, .85f), BarFrame = new Color(.05f, .05f, .07f, .9f);
        public int RemainingLabs { get { return remainingLabs; } }
        public int RequiredLabs { get { return needLabs; } }
        public float ShotInterval { get { return rate; } }
        public bool ExamActive { get { return examActive; } }
        public bool IsDamageProtected { get { return examActive && !defeated && Time.time < damageProtectedUntil; } }
        public float DamageProtectionRemaining { get { return IsDamageProtected ? damageProtectedUntil - Time.time : 0f; } }
        private float HitGrace { get { return encounterStage == 1 ? 0.45f : encounterStage == 2 ? 0.5f : 0.55f; } }
        private float Windup { get { return encounterStage == 1 ? 0.8f : encounterStage == 2 ? 0.7f : 0.65f; } }
        private float Recovery { get { return encounterStage == 1 ? 1f : encounterStage == 2 ? 1.1f : 1.2f; } }
        private float BurstSpacing { get { return encounterStage == 1 ? 0.24f : encounterStage == 2 ? 0.22f : 0.14f; } }
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
            BuildWarning();
        }

        private void BuildWarning()
        {
            // Compact attention marks identify who is preparing an attack; HUD text names the dodge.
            // Rebind cloned renderers to this teacher's own sprite, so defeating teacher one cannot
            // destroy the final examiner's warning resource.
            warning = transform.Find("Attack windup");
            if (warning == null) { warning = new GameObject("Attack windup").transform; warning.SetParent(transform, false); }
            warningPixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            for (int i = 0; i < 3; i++)
            {
                Transform mark = warning.Find("Attention " + i);
                if (mark == null) { mark = new GameObject("Attention " + i).transform; mark.SetParent(warning, false); }
                SpriteRenderer renderer = mark.GetComponent<SpriteRenderer>();
                if (renderer == null) renderer = mark.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = warningPixel;
                renderer.color = Color.white;
                renderer.sortingOrder = visual != null ? visual.sortingOrder + 1 : 1;
                if (visual != null) { renderer.sortingLayerID = visual.sortingLayerID; renderer.sharedMaterial = visual.sharedMaterial; }
                mark.localPosition = new Vector3((i - 1) * 0.22f, i == 1 ? 0.06f : 0, 0);
                mark.localRotation = Quaternion.Euler(0, 0, (1 - i) * 22f);
                mark.localScale = new Vector3(0.065f, 0.2f, 1);
            }
            warning.gameObject.SetActive(false);
            BuildHealthBar();
        }

        private void BuildHealthBar()
        {
            healthBar = transform.Find("Exam hits bar");
            if (healthBar == null) { healthBar = new GameObject("Exam hits bar").transform; healthBar.SetParent(transform, false); }
            healthBack = NewPixel("Frame", healthBar);
            healthBar.gameObject.SetActive(false);
            RefreshHealthBar();
        }
        private SpriteRenderer NewPixel(string label, Transform parent)
        {
            SpriteRenderer pixel = new GameObject(label).AddComponent<SpriteRenderer>();
            pixel.transform.SetParent(parent, false);
            pixel.sprite = warningPixel;
            return pixel;
        }
        private void NotifyHealth() { RefreshHealthBar(); HealthChanged?.Invoke(); }
        private void RefreshHealthBar()
        {
            if (healthBar == null || healthBack == null) return;
            int cells = Mathf.Clamp(needLabs, 1, 20);
            if (healthCells.Length != cells)
            {
                foreach (SpriteRenderer old in healthCells) if (old != null) Destroy(old.gameObject);
                healthCells = new SpriteRenderer[cells];
                for (int i = 0; i < cells; i++) healthCells[i] = NewPixel("Hit " + (i + 1), healthBar);
            }
            float width = Mathf.Min(CellWidth, (BarMaxWidth - CellGap * (cells - 1)) / cells);
            float total = cells * width + (cells - 1) * CellGap;
            int order = visual != null ? visual.sortingOrder + 2 : 52;
            int layer = visual != null ? visual.sortingLayerID : 0;
            healthBack.transform.localPosition = Vector3.zero;
            healthBack.transform.localScale = new Vector3(total + BarBorder * 2, CellHeight + BarBorder * 2, 1);
            healthBack.color = BarFrame; healthBack.sortingLayerID = layer; healthBack.sortingOrder = order;
            for (int i = 0; i < cells; i++)
            {
                SpriteRenderer cell = healthCells[i];
                cell.transform.localPosition = new Vector3(-total * .5f + width * .5f + i * (width + CellGap), 0, -.01f);
                cell.transform.localScale = new Vector3(width, CellHeight, 1);
                cell.color = i < remainingLabs ? CellFull : CellEmpty;
                cell.sortingLayerID = layer; cell.sortingOrder = order + 1;
            }
        }
        private void UpdateHealthBar()
        {
            if (healthBar == null) return;
            bool near = player != null && Mathf.Abs(player.transform.position.x - transform.position.x) < 15f &&
                        Mathf.Abs(player.transform.position.y - transform.position.y) < 6f;
            bool show = !defeated && (examActive || near);
            if (healthBar.gameObject.activeSelf != show) healthBar.gameObject.SetActive(show);
            if (!show) return;
            float head = bodyCollider != null ? bodyCollider.bounds.max.y : transform.position.y + 3f;
            healthBar.position = new Vector3(transform.position.x, head + .3f, transform.position.z);
            Vector3 scale = transform.lossyScale;
            healthBar.localScale = new Vector3(1f / Mathf.Max(.01f, Mathf.Abs(scale.x)), 1f / Mathf.Max(.01f, Mathf.Abs(scale.y)), 1);
        }

        public void Configure(int labs, float shotInterval, Color color, bool isFinalExam = false)
        {
            needLabs = Mathf.Max(1, labs);
            rate = Mathf.Max(0.2f, shotInterval);
            bulletColor = color;
            finalExam = isFinalExam;
            if (finalExam && appearanceStage != encounterStage)
            {
                string path = encounterStage == 3 ? "Combat/FinalExaminerAppearance" : "Combat/ExaminerAppearance";
                ExaminerAppearance appearance = Resources.Load<ExaminerAppearance>(path);
                if (appearance != null) visual = appearance.Apply(gameObject, visual, bodyCollider as BoxCollider2D);
                else Debug.LogWarning("Missing examiner artwork: " + path, this);
                appearanceStage = encounterStage;
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
            timer = OpeningDelay;
            flashUntil = damageProtectedUntil = 0;
            EndWarning();
            NotifyHealth();
        }
        public void ConfigureCampaign(int stage, int required)
        {
            encounterStage = Mathf.Clamp(stage, 1, 3);
            Configure(required, Recovery, Color.white, encounterStage > 1);
            // Every examiner must draw in front of authored doorway scenery.
            if (visual != null) visual.sortingOrder = 50;
            if (warning != null && visual != null)
                foreach (SpriteRenderer mark in warning.GetComponentsInChildren<SpriteRenderer>(true)) mark.sortingOrder = visual.sortingOrder + 1;
            RefreshHealthBar();
        }
        public void SetExamActive(bool active)
        {
            if (!active && examActive) ClearGrades();
            examActive = active;
            if (!active) { shotsRemaining = 0; flashUntil = damageProtectedUntil = 0; EndWarning(); }
            timer = OpeningDelay;
        }
        public void SetRequiredLabs(int required, bool preserveDamage = true)
        {
            int dealt = preserveDamage ? needLabs - remainingLabs : 0;
            needLabs = Mathf.Max(1, required);
            remainingLabs = Mathf.Max(0, needLabs - dealt);
            NotifyHealth();
            if (remainingLabs == 0 && !defeated) CompleteDefeat();
        }

        protected override void Update()
        {
            UpdateHealthBar();
            if (visual != null) visual.color = Time.time < flashUntil ? new Color(1f, 0.35f, 0.3f)
                : IsDamageProtected ? Color.Lerp(restingColor, new Color(1f, 0.55f, 0.5f, 0.55f), Mathf.PingPong(Time.time * 14f, 1f))
                : IsTelegraphing ? Color.Lerp(restingColor, new Color(0.65f, 0.9f, 1f), Mathf.PingPong(Time.time * 5f, 1f) * 0.3f) : restingColor;
            if (GameSession.BlocksGameplay || defeated || !examActive || bullet == null || player == null) return;
            bool nearby = Mathf.Abs(player.transform.position.x - transform.position.x) <= 14f &&
                          Mathf.Abs(player.transform.position.y - transform.position.y) <= 4f;
            if (!nearby) { shotsRemaining = 0; EndWarning(); timer = OpeningDelay; return; }
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            if (IsTelegraphing)
            {
                EndWarning();
                shotsRemaining = encounterStage == 3 ? 3 : 2;
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
            attackOrigin = new Vector3(transform.position.x, feet + (highAttack ? 2.55f : 0.8f), transform.position.z);
            timer = Windup;
            IsTelegraphing = true;
            float head = bodyCollider != null ? bodyCollider.bounds.max.y : transform.position.y + 3f;
            warning.position = new Vector3(transform.position.x, head + 0.75f, transform.position.z);
            warning.gameObject.SetActive(true);
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
            timer = shotsRemaining > 0 ? BurstSpacing : Mathf.Max(Recovery, rate);
        }

        private void EndWarning()
        {
            bool wasVisible = IsTelegraphing;
            IsTelegraphing = false;
            if (warning != null) warning.gameObject.SetActive(false);
            if (wasVisible) AttackChanged?.Invoke();
        }

        private float nextIdleHintAt;
        public override void ReceiveDamage()
        {
            if (GameSession.BlocksGameplay || defeated || IsDamageProtected) return;
            if (!examActive)
            {
                // A throw at a teacher whose exam has not started used to vanish silently while
                // the player kept paying Sleep. Explain how to begin instead.
                if (Time.time >= nextIdleHintAt)
                {
                    nextIdleHintAt = Time.time + 2.5f;
                    GameplayFeedback.Show(CampaignState.CanStartExam(encounterStage) ? "campaign.hitBeforeStart" : "campaign.hitNotReady");
                }
                return;
            }
            remainingLabs = Mathf.Max(0, remainingLabs - 1);
            damageProtectedUntil = Time.time + HitGrace;
            flashUntil = Time.time + 0.16f;
            NotifyHealth();
            if (remainingLabs > 0) { RunSaveService.RequestSave(); return; }
            CompleteDefeat();
        }
        private void CompleteDefeat()
        {
            defeated = true;
            examActive = false;
            shotsRemaining = 0;
            EndWarning();
            ClearGrades();
            if (Defeated != null) Defeated(this);
            else Debug.LogWarning("Examiner defeated without an ExamProgression listener; campaign completion was not awarded. Start the run through the campaign scene bootstrap.", this);
            Destroy(gameObject);
        }
        private void ClearGrades()
        {
            foreach (BulletOcenka grade in FindObjectsByType<BulletOcenka>(FindObjectsSortMode.None))
                if (grade.Parent == gameObject) { grade.gameObject.SetActive(false); Destroy(grade.gameObject); }
        }

        protected override void OnTriggerEnter2D(Collider2D collider) { }
        private void OnDisable() { EndWarning(); }
        private void OnDestroy() { if (warningPixel != null) Destroy(warningPixel); }
    }
}
