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
        private SpriteRenderer heart;
        private Sprite heartSprite;
        private Texture2D heartTexture;
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
            if (heart != null) heart.enabled = interaction.enabled;
            if (!interaction.enabled) base.OnDisable(); // Clear targeting immediately, while retaining the visible NPC/effect.
        }
        private void BuildHeart(SpriteRenderer art)
        {
            // A single point-filtered sprite: the same pixel heart marks the available choice
            // and floats away after success, rather than a second effect with different artwork.
            string[] rows = { "011000110", "111101111", "111111111", "111111111", "011111110", "001111100", "000111000", "000010000" };
            const int width = 9, height = 8;
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (rows[y][x] == '0') continue;
                    bool edge = x == 0 || x == width - 1 || y == 0 || y == height - 1 ||
                        rows[y][x - 1] == '0' || rows[y][x + 1] == '0' || rows[y - 1][x] == '0' || rows[y + 1][x] == '0';
                    pixels[(height - 1 - y) * width + x] = edge ? new Color32(80, 20, 52, 255)
                        : y < 3 && x < 3 ? new Color32(255, 180, 207, 255) : new Color32(255, 86, 139, 255);
                }
            heartTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            heartTexture.name = "Friendly student pixel heart";
            heartTexture.filterMode = FilterMode.Point; heartTexture.wrapMode = TextureWrapMode.Clamp;
            heartTexture.SetPixels32(pixels); heartTexture.Apply(false, true);
            heartSprite = Sprite.Create(heartTexture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 16f);
            heart = new GameObject("Pixel heart - optional flirt").AddComponent<SpriteRenderer>();
            heart.transform.SetParent(transform, false);
            heart.transform.localPosition = Vector3.up * 3.03f;
            heart.sprite = heartSprite; heart.sharedMaterial = art.sharedMaterial;
            heart.sortingLayerID = art.sortingLayerID; heart.sortingOrder = art.sortingOrder + 2;
        }
        private void Update()
        {
            if (heart == null || GameSession.BlocksGameplay) return;
            if (!flirted)
            {
                heart.enabled = hasAppearance && interaction.enabled;
                heart.transform.localPosition = Vector3.up * (3.03f + Mathf.Sin(Time.time * 2f) * 0.06f);
                heart.color = Color.white;
                return;
            }
            if (!heart.enabled) return;
            float remaining = heartUntil - Time.time;
            if (remaining <= 0f) { heart.enabled = false; return; }
            heart.transform.localPosition = Vector3.up * (3.03f + (1.2f - remaining) * 0.4f);
            heart.color = new Color(1f, 1f, 1f, Mathf.Clamp01(remaining / 0.5f));
        }
        private void OnDestroy()
        {
            if (heartSprite != null) Destroy(heartSprite);
            if (heartTexture != null) Destroy(heartTexture);
        }
        public void Capture(RunSnapshot snapshot) { snapshot.flirted = flirted; }
        public void Restore(RunSnapshot snapshot)
        {
            flirted = snapshot.flirted;
            SetAvailable(!flirted);
        }
    }
}
