using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Characters
{
    // Wardrobe in the hostel bedroom (background art): Interact cycles through unlocked outfit
    // colours. The player art has a single outfit, so outfits are shirt/jeans recolours done by
    // Resources/Shaders/OutfitSwap on every player sprite renderer (all animation frames and the
    // crouch pose parts). Two outfits at the start, one more per passed exam; saved with the run.
    public sealed class HostelWardrobe : PlayerInteractionTarget, IRunSaveParticipant
    {
        private struct Outfit
        {
            public string NameKey; public bool SwapShirt, SwapJeans; public Color Shirt, Jeans;
            public Outfit(string key, bool shirt, Color shirtColor, bool jeans, Color jeansColor)
            { NameKey = key; SwapShirt = shirt; Shirt = shirtColor; SwapJeans = jeans; Jeans = jeansColor; }
        }
        private static readonly Outfit[] Outfits =
        {
            new Outfit("outfit.classic", false, Color.white, false, Color.white),
            new Outfit("outfit.red", true, new Color(.86f, .22f, .2f), false, Color.white),
            new Outfit("outfit.green", true, new Color(.32f, .7f, .36f), true, new Color(.22f, .22f, .26f)),
            new Outfit("outfit.yellow", true, new Color(.96f, .8f, .26f), true, new Color(.55f, .55f, .58f)),
            new Outfit("outfit.purple", true, new Color(.58f, .38f, .84f), true, new Color(.14f, .19f, .4f)),
        };
        // Measured from level.png (107 px/unit) and mirrored (Main "level" renderer centred at
        // x=-64.15 with flipX). Upstairs bedroom floor y≈2.76; PC x≈-85.35, bed x≈-82.4.
        private static readonly Vector3 DefaultPosition = new Vector3(-89.26f, 2.76f, 0f);
        private int current;
        private Material material;
        private Character player;
        public int RestoreOrder => 35;
        public override string PromptKey => "wardrobe.prompt";
        private static int Unlocked
        {
            get
            {
                int passed = CampaignState.Completed ? 3 : Mathf.Clamp(CampaignState.Stage - 1, 0, 2);
                return Mathf.Min(Outfits.Length, 2 + passed);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<HostelWardrobe>(FindObjectsInactive.Include) != null) return;
            var wardrobe = new GameObject("Hostel wardrobe - outfits");
            wardrobe.transform.position = DefaultPosition;
            wardrobe.AddComponent<HostelWardrobe>();
        }
        private void Awake()
        {
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<BoxCollider2D>();
                trigger.size = new Vector2(1.4f, 2.4f); trigger.offset = new Vector2(0f, 1.2f);
            }
            trigger.isTrigger = true;
        }
        private void Start() { Apply(); }

        public override void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || !HasPlayerNearby || GameSession.BlocksGameplay) return;
            int unlocked = Unlocked;
            current = (Mathf.Clamp(current, 0, unlocked - 1) + 1) % unlocked;
            Apply();
            GameplayFeedback.Show(unlocked < Outfits.Length ? "wardrobe.changedMore" : "wardrobe.changed", Outfits[current].NameKey);
            RunSaveService.RequestSave();
        }
        private void Apply()
        {
            if (player == null) player = FindFirstObjectByType<Character>();
            if (player == null) return;
            if (material == null)
            {
                Shader shader = Resources.Load<Shader>("Shaders/OutfitSwap");
                if (shader == null || !shader.isSupported) { Debug.LogWarning("Outfit shader unavailable; wardrobe keeps the original outfit.", this); return; }
                material = new Material(shader) { name = "Player outfit" };
            }
            Outfit outfit = Outfits[Mathf.Clamp(current, 0, Outfits.Length - 1)];
            material.SetColor("_ShirtColor", outfit.Shirt);
            material.SetColor("_JeansColor", outfit.Jeans);
            material.SetFloat("_SwapShirt", outfit.SwapShirt ? 1f : 0f);
            material.SetFloat("_SwapJeans", outfit.SwapJeans ? 1f : 0f);
            foreach (SpriteRenderer renderer in player.GetComponentsInChildren<SpriteRenderer>(true))
            {
                // Only the student's own body art: never held inventory items or effects with their own materials.
                if (renderer.GetComponentInParent<StudentSurvival.World.PlayerInteractionTarget>(true) != null) continue;
                if (UnderInventory(renderer.transform, player.transform)) continue;
                renderer.sharedMaterial = material;
            }
        }
        private static bool UnderInventory(Transform node, Transform root)
        {
            for (; node != null && node != root; node = node.parent)
                if (node.CompareTag("Inventory") || node.CompareTag("InventoryItems")) return true;
            return false;
        }
        public void Capture(RunSnapshot snapshot) { snapshot.outfit = current; }
        public void Restore(RunSnapshot snapshot)
        {
            current = Mathf.Clamp(snapshot.outfit, 0, Outfits.Length - 1);
            Apply();
        }
        private void OnDestroy() { if (material != null) Destroy(material); }
    }
}
