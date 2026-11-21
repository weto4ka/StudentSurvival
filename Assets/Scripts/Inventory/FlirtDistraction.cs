using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    // Deliberately opt-in: a nearby student never spends an item just by being seen.
    public sealed class FlirtDistraction : PlayerInteractionTarget, IRunSaveParticipant
    {
        private InventoryController inventory;
        private BoxCollider2D interaction;
        private LineRenderer heart;
        private bool flirted, hasAppearance;
        private float heartUntil;
        public override string PromptKey { get { return "flirt.prompt"; } }
        public int RestoreOrder { get { return 40; } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<FlirtDistraction>(FindObjectsInactive.Include) != null) return;
            // Between the authored start (-7.45,-3.87) and starter pickups near x=-1.5.
            // Their trigger footprints remain separate; walk past to collect a lab, then return by choice.
            Vector3 position = new Vector3(-4.6f, -3.87f, 0);
            RaycastHit2D[] floors = Physics2D.RaycastAll(new Vector2(position.x, position.y + 0.3f), Vector2.down, 2f);
            foreach (RaycastHit2D hit in floors)
            {
                if (hit.collider == null || hit.collider.isTrigger || hit.normal.y < 0.65f ||
                    hit.collider.GetComponentInParent<Unit>() != null || hit.collider.GetComponentInParent<PlayerInteractionTarget>() != null) continue;
                position.y = hit.point.y;
                break;
            }
            var student = new GameObject("Friendly student — optional distraction");
            student.transform.position = position;
            student.AddComponent<FlirtDistraction>();
        }

        private void Awake()
        {
            inventory = FindFirstObjectByType<InventoryController>();
            interaction = gameObject.AddComponent<BoxCollider2D>();
            interaction.isTrigger = true;
            interaction.size = new Vector2(1.1f, 2.65f);
            interaction.offset = Vector2.up * 1.325f;
            ExaminerAppearance appearance = Resources.Load<ExaminerAppearance>("Combat/FriendlyStudentAppearance");
            SpriteRenderer art = appearance != null ? appearance.Apply(gameObject, null, interaction) : null;
            hasAppearance = art != null;
            if (art != null)
            {
                Character player = FindFirstObjectByType<Character>();
                SpriteRenderer playerArt = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
                art.sortingLayerID = playerArt != null ? playerArt.sortingLayerID : 0;
                art.sortingOrder = playerArt != null ? playerArt.sortingOrder - 1 : 2;
                BuildHeart(art);
            }
            else interaction.enabled = false; // An unavailable art reference must never create an invisible paid target.
        }

        public override void Interact(PlayerInteractor player)
        {
            if (player == null || flirted || GameSession.BlocksGameplay || !HasPlayerNearby || inventory == null) return;
            if (!inventory.SpendCollectedLab()) { GameplayFeedback.Show("flirt.noLab"); return; }
            flirted = true;
            SetAvailable(false);
            heartUntil = Time.time + 1.2f;
            if (heart != null) heart.enabled = true;
            GameplayFeedback.Show("flirt.success");
            RunSaveService.RequestSave();
        }

        private void SetAvailable(bool available)
        {
            interaction.enabled = available && hasAppearance;
            if (!interaction.enabled) base.OnDisable(); // Clear targeting immediately, while retaining the visible NPC/effect.
        }
        private void BuildHeart(SpriteRenderer art)
        {
            heart = new GameObject("Brief heart").AddComponent<LineRenderer>();
            heart.transform.SetParent(transform, false);
            heart.transform.localPosition = Vector3.up * 2.95f;
            heart.useWorldSpace = false;
            heart.loop = true;
            heart.sharedMaterial = art.sharedMaterial;
            heart.sortingLayerID = art.sortingLayerID;
            heart.sortingOrder = art.sortingOrder + 2;
            heart.startWidth = heart.endWidth = 0.055f;
            heart.startColor = heart.endColor = new Color(1f, 0.3f, 0.55f);
            const int points = 32;
            heart.positionCount = points;
            for (int i = 0; i < points; i++)
            {
                float angle = i * (Mathf.PI * 2f / points);
                float sine = Mathf.Sin(angle);
                heart.SetPosition(i, new Vector3(sine * sine * sine,
                    (13f * Mathf.Cos(angle) - 5f * Mathf.Cos(2f * angle) - 2f * Mathf.Cos(3f * angle) - Mathf.Cos(4f * angle)) / 16f) * 0.3f);
            }
            heart.enabled = false;
        }
        private void Update()
        {
            if (heart == null || !heart.enabled || GameSession.BlocksGameplay) return;
            float remaining = heartUntil - Time.time;
            if (remaining <= 0f) { heart.enabled = false; return; }
            heart.transform.localPosition = Vector3.up * (2.95f + (1.2f - remaining) * 0.4f);
            Color color = new Color(1f, 0.3f, 0.55f, Mathf.Clamp01(remaining / 0.5f));
            heart.startColor = heart.endColor = color;
        }
        public void Capture(RunSnapshot snapshot) { snapshot.flirted = flirted; }
        public void Restore(RunSnapshot snapshot)
        {
            flirted = snapshot.flirted;
            SetAvailable(!flirted);
            if (heart != null) heart.enabled = false;
        }
    }
}
