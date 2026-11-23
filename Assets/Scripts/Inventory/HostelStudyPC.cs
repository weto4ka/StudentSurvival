using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    // The PC on the hostel bedroom desk (background art) lets the student prepare lab reports.
    // Balance: once per exam stage (saved with the run), not during an exam, costs 2 Sleep and
    // needs more than 2 Sleep, gives 2 collected (free) lab reports.
    public sealed class HostelStudyPC : PlayerInteractionTarget
    {
        // Measured from level.png (107 px/unit) and mirrored, because the Main "level" renderer
        // is centred at x=-64.15 with flipX on. Upstairs bedroom floor y≈2.76; bed is at x≈-82.4.
        private static readonly Vector3 DefaultPosition = new Vector3(-85.35f, 2.76f, 0f);
        private const int SleepCost = 2, LabsGiven = 2;
        private InventoryController inventory;
        private Character player;
        public override string PromptKey => CampaignState.CanUseHostelPc ? "pc.prompt" : CampaignState.Completed ? "pc.nothingLeft" : CampaignState.ExamActive ? "bed.examActive" : "pc.used";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<HostelStudyPC>(FindObjectsInactive.Include) != null) return;
            var pc = new GameObject("Hostel PC - prepare labs");
            pc.transform.position = DefaultPosition;
            pc.AddComponent<HostelStudyPC>();
        }
        private void Awake()
        {
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<BoxCollider2D>();
                trigger.size = new Vector2(1.6f, 2.4f); trigger.offset = new Vector2(0f, 1.2f);
            }
            trigger.isTrigger = true;
        }
        private void Start()
        {
            inventory = FindFirstObjectByType<InventoryController>();
            player = FindFirstObjectByType<Character>();
        }
        public override void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || !HasPlayerNearby || GameSession.BlocksGameplay || player == null || inventory == null) return;
            if (!CampaignState.CanUseHostelPc) { GameplayFeedback.Show(PromptKey); return; }
            if (player.Sleep <= SleepCost) { GameplayFeedback.Show("pc.tired"); return; }
            InventoryItem prefab = Resources.Load<InventoryItem>("Inventory/Items/Lab");
            if (prefab == null) { GameplayFeedback.Show("trade.unavailable"); return; }
            // Same path as the friend's gift: created under the inventory, never a world pickup.
            InventoryItem labs = Instantiate(prefab, inventory.transform, false);
            labs.SetCount(LabsGiven);
            labs.sprite = labs.GetComponentInChildren<SpriteRenderer>(true);
            LootBoxItem pickup = labs.GetComponent<LootBoxItem>();
            if (pickup != null) pickup.SetHeldId("hostel.pc.labs");
            if (!inventory.Add(labs)) { Destroy(labs.gameObject); GameplayFeedback.Show("feedback.full"); return; }
            player.Sleep -= SleepCost;
            CampaignState.TryUseHostelPc();
            GameplayFeedback.Show("pc.done");
            RunSaveService.RequestSave();
        }
    }
}
