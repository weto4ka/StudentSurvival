using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    /// <summary>Opt-in study action; CampaignState owns the preparation budget and persistence.</summary>
    public sealed class PreparationDesk : PlayerInteractionTarget
    {
        private BoxCollider2D interaction;
        private Sprite pixel;
        private SpriteRenderer pages;
        private SpriteRenderer playerArt;
        public override string PromptKey { get { return "preparation.prompt"; } }

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
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<PreparationDesk>(FindObjectsInactive.Include) != null) return;
            var desk = new GameObject("Preparation desk — study by choice");
            desk.transform.position = new Vector3(46.5f, 3.04f, 0);
            desk.AddComponent<PreparationDesk>();
        }
        private void Awake()
        {
            interaction = gameObject.AddComponent<BoxCollider2D>();
            interaction.isTrigger = true;
            interaction.size = new Vector2(1.7f, 2.1f);
            interaction.offset = Vector2.up * 1.05f;
            pixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width,
                Texture2D.whiteTexture.height), Vector2.one * .5f, Texture2D.whiteTexture.width);
            pixel.name = "Preparation desk rectangle";
            Character player = FindFirstObjectByType<Character>();
            playerArt = player != null ? player.GetComponentInChildren<SpriteRenderer>() : null;
            // Flat rectangles keep the desk in the existing game's pixel style. No physics obstacle.
            Part("Left leg", new Vector2(-.62f, .55f), new Vector2(.14f, 1.1f), new Color32(70, 48, 43, 255));
            Part("Right leg", new Vector2(.62f, .55f), new Vector2(.14f, 1.1f), new Color32(70, 48, 43, 255));
            Part("Desk top", new Vector2(0, 1.13f), new Vector2(1.85f, .18f), new Color32(139, 89, 58, 255));
            Part("Book cover", new Vector2(0, 1.27f), new Vector2(.84f, .14f), new Color32(42, 86, 114, 255));
            pages = Part("Open pages", new Vector2(0, 1.35f), new Vector2(.72f, .1f), new Color32(242, 225, 187, 255));
            Part("Book spine", new Vector2(0, 1.37f), new Vector2(.035f, .12f), new Color32(139, 89, 58, 255));
            RefreshAvailability();
        }
        private SpriteRenderer Part(string label, Vector2 position, Vector2 size, Color color)
        {
            var art = new GameObject(label).AddComponent<SpriteRenderer>();
            art.transform.SetParent(transform, false);
            art.transform.localPosition = position;
            art.transform.localScale = new Vector3(size.x, size.y, 1);
            art.sprite = pixel;
            art.color = color;
            art.sortingLayerID = playerArt != null ? playerArt.sortingLayerID : 0;
            // Room backgrounds use 10 and floor foreground 30; desk details start at 40.
            art.sortingOrder = 40 + transform.childCount - 1;
            return art;
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
            interaction.enabled = CampaignState.IsPreparationStage && CampaignState.CanSpendPreparation;
            if (!interaction.enabled) base.OnDisable();
            if (pages != null) pages.color = CampaignState.PreparedPoints > 0
                ? new Color32(172, 226, 184, 255) : new Color32(242, 225, 187, 255);
        }
        public override void Interact(PlayerInteractor player)
        {
            if (player == null || GameSession.BlocksGameplay || !HasPlayerNearby) return;
            if (!CampaignState.TryPrepare()) { GameplayFeedback.Show("preparation.exhausted"); return; }
            GameplayFeedback.Show(CampaignState.HasRelationship ? "preparation.friendReady" : "preparation.success");
            RunSaveService.RequestSave();
        }
        private void OnDestroy() { if (pixel != null) Destroy(pixel); }
    }
}
