using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    // Deliberately opt-in: a nearby student never spends an item just by being seen.
    public sealed class FlirtDistraction : PlayerInteractionTarget
    {
        private InventoryController inventory;
        private BoxCollider2D interaction;
        private SpriteRenderer heart;
        private Sprite heartSprite;
        private Texture2D heartTexture;
        private bool hasAppearance, actionable;
        private float heartUntil;
        public override string PromptKey { get { return CampaignState.CanClaimRelationshipHelp ? "flirt.helpPrompt" : CampaignState.CanFlirt ? "flirt.stage3Prompt" : WaitingKey; } }
        private bool Actionable { get { return CampaignState.CanFlirt || CampaignState.CanClaimRelationshipHelp; } }
        // The student stays approachable and explains herself instead of silently ignoring E.
        private static string WaitingKey
        {
            get
            {
                if (!CampaignState.Completed && CampaignState.Stage < 3) return "flirt.notYet";
                if (CampaignState.IsPreparationStage && CampaignState.HasRelationship && !CampaignState.RelationshipHelpClaimed &&
                    CampaignState.PreparedPoints < 1 && CampaignState.PreparationRemaining > 0) return "flirt.studyFirst";
                return "flirt.nothingLeft";
            }
        }

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
            // The optional social choice belongs to the third preparation room only.
            Debug.LogError("Main scene needs its authored friendly student.");
        }

        private void Awake()
        {
            inventory = FindFirstObjectByType<InventoryController>();
            interaction = GetComponent<BoxCollider2D>();
            if (interaction == null) { Debug.LogError("Friendly student needs its authored interaction collider.", this); enabled = false; return; }
            ExaminerAppearance appearance = Resources.Load<ExaminerAppearance>("Combat/FriendlyStudentAppearance");
            SpriteRenderer art = appearance != null ? appearance.Apply(gameObject, GetComponentInChildren<SpriteRenderer>(true), interaction) : null;
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
            RefreshAvailability();
        }

        private void OnEnable() { CampaignState.Changed += RefreshAvailability; RefreshAvailability(); }
        protected override void OnDisable()
        {
            CampaignState.Changed -= RefreshAvailability;
            base.OnDisable();
        }
        private void RefreshAvailability()
        {
            if (interaction == null) return;
            SetAvailable(Actionable);
        }

        public override void Interact(PlayerInteractor player)
        {
            if (player == null || GameSession.BlocksGameplay || !HasPlayerNearby || inventory == null) return;
            if (!Actionable) { GameplayFeedback.Show(WaitingKey); return; }
            if (CampaignState.CanClaimRelationshipHelp)
            {
                ReceiveFriendHelp();
                return;
            }
            if (!CampaignState.CanFlirt) { GameplayFeedback.Show("preparation.exhausted"); return; }
            if (!inventory.SpendCollectedLab()) { GameplayFeedback.Show("flirt.noLab"); return; }
            if (!CampaignState.TryFlirt()) return;
            Celebrate();
            GameplayFeedback.Show(CampaignState.PreparedPoints > 0 ? "flirt.helpReady" : "flirt.preparationSuccess");
            RunSaveService.RequestSave();
        }

        private void ReceiveFriendHelp()
        {
            InventoryItem prefab = Resources.Load<InventoryItem>("Inventory/Items/Lab");
            if (prefab == null) { Debug.LogError("Missing collected-lab resource for friend help.", this); return; }
            // Instantiate under Inventory so LootBoxItem never registers a generated transform path.
            // The stable source ID survives stacking, saving, and restoration like other collected labs.
            InventoryItem gift = Instantiate(prefab, inventory.transform, false);
            gift.SetCount(2);
            // The inactive held prefab has not run Awake; initialize its HUD icon explicitly.
            gift.sprite = gift.GetComponentInChildren<SpriteRenderer>(true);
            LootBoxItem pickup = gift.GetComponent<LootBoxItem>();
            if (pickup != null) pickup.SetHeldId("campaign.stage3.friend-help");
            if (!inventory.Add(gift))
            {
                Destroy(gift.gameObject);
                GameplayFeedback.Show("feedback.full");
                return; // Full inventory never consumes the one-time help claim.
            }
            CampaignState.TryClaimRelationshipHelp();
            Celebrate();
            GameplayFeedback.Show("flirt.helpReceived");
            RunSaveService.RequestSave();
        }

        private void Celebrate()
        {
            heartUntil = Time.time + 1.2f;
            if (heart != null) heart.enabled = true;
        }

        // The heart marks a choice that can be taken now; the trigger stays on (with visible art)
        // so pressing E always explains when the choice becomes available.
        private void SetAvailable(bool available)
        {
            actionable = available && hasAppearance;
            interaction.enabled = hasAppearance;
            if (heart != null) heart.enabled = actionable;
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
            float remaining = heartUntil - Time.time;
            if (remaining > 0f)
            {
                heart.enabled = true;
                heart.transform.localPosition = Vector3.up * (3.03f + (1.2f - remaining) * 0.4f);
                heart.color = new Color(1f, 1f, 1f, Mathf.Clamp01(remaining / 0.5f));
                return;
            }
            heart.enabled = actionable;
            heart.transform.localPosition = Vector3.up * (3.03f + Mathf.Sin(Time.time * 2f) * 0.06f);
            heart.color = Color.white;
        }
        private void OnDestroy()
        {
            if (heartSprite != null) Destroy(heartSprite);
            if (heartTexture != null) Destroy(heartTexture);
        }
    }
}
