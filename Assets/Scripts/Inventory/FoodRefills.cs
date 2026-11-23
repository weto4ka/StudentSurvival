using System.Collections.Generic;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    // Shawarma spots refill (user request 2026-11-24). Every world food pickup stays a normal
    // one-time pickup; once it is gone, an invisible spot at the same place hands out one fresh
    // shawarma every RefillSeconds. Remaining times are saved with the run, so menu/Continue
    // cannot skip the wait.
    public sealed class FoodRefills : MonoBehaviour, IRunSaveParticipant
    {
        public const float RefillSeconds = 180f;
        private readonly List<FoodRefillSpot> spots = new List<FoodRefillSpot>();
        private readonly Dictionary<string, float> savedSeconds = new Dictionary<string, float>();
        public int RestoreOrder => 37;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<FoodRefills>(FindObjectsInactive.Include) != null) return;
            new GameObject("Food refill spots").AddComponent<FoodRefills>();
        }
        private void Start()
        {
            // Campaign supplies are created during scene load, so every world pickup exists by Start.
            foreach (LootBoxItem pickup in FindObjectsByType<LootBoxItem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                InventoryItem item = pickup.GetComponent<InventoryItem>();
                if (item == null || item.itemType != Item.Shaverma || !pickup.IsWorldPickup || string.IsNullOrEmpty(pickup.PersistentId)) continue;
                var spot = new GameObject("Shawarma refill - " + pickup.name).AddComponent<FoodRefillSpot>();
                spot.transform.SetParent(transform, true);
                spot.Configure(this, pickup);
                spots.Add(spot);
            }
        }
        internal bool TryTakeSaved(string id, out float seconds)
        {
            if (!savedSeconds.TryGetValue(id, out seconds)) return false;
            savedSeconds.Remove(id); return true;
        }
        public void Capture(RunSnapshot snapshot)
        {
            var ids = new List<string>(); var seconds = new List<float>();
            foreach (FoodRefillSpot spot in spots)
                if (spot != null && spot.IsTracking) { ids.Add(spot.Id); seconds.Add(spot.CooldownSeconds); }
            snapshot.foodRefillIds = ids.ToArray(); snapshot.foodRefillSeconds = seconds.ToArray();
        }
        public void Restore(RunSnapshot snapshot)
        {
            savedSeconds.Clear();
            if (snapshot.foodRefillIds == null || snapshot.foodRefillSeconds == null) return;
            int count = Mathf.Min(snapshot.foodRefillIds.Length, snapshot.foodRefillSeconds.Length);
            for (int i = 0; i < count; i++)
                if (!string.IsNullOrEmpty(snapshot.foodRefillIds[i]))
                    savedSeconds[snapshot.foodRefillIds[i]] = Mathf.Clamp(snapshot.foodRefillSeconds[i], 0f, RefillSeconds);
        }
    }

    public sealed class FoodRefillSpot : PlayerInteractionTarget
    {
        private FoodRefills owner;
        private LootBoxItem pickup;
        private BoxCollider2D trigger;
        private InventoryController inventory;
        private bool seenAvailable;
        private float readyAt;
        public string Id { get; private set; }
        public bool IsTracking { get; private set; }
        private bool Ready => Time.time >= readyAt;
        public override float CooldownSeconds => IsTracking ? Mathf.Max(0f, readyAt - Time.time) : 0f;
        public override string PromptKey => Ready ? "food.refillPrompt" : "food.refillIn";

        internal void Configure(FoodRefills manager, LootBoxItem source)
        {
            owner = manager; pickup = source; Id = source.PersistentId;
            transform.position = source.transform.position;
            trigger = gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true; trigger.size = new Vector2(1.4f, 2.4f); trigger.offset = new Vector2(0f, .6f);
            trigger.enabled = false;
            inventory = FindFirstObjectByType<InventoryController>();
        }
        private void Update()
        {
            if (GameSession.BlocksGameplay || IsTracking || owner == null) return;
            bool available = pickup != null && pickup.IsAvailable;
            if (available) { seenAvailable = true; return; }
            // The original pickup is gone: a saved wait, a fresh wait if it was just taken in this
            // session, or ready at once for food collected in an older save.
            float wait = owner.TryTakeSaved(Id, out float saved) ? saved : seenAvailable ? FoodRefills.RefillSeconds : 0f;
            readyAt = Time.time + wait;
            IsTracking = true;
            trigger.enabled = true;
        }
        public override void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || !HasPlayerNearby || GameSession.BlocksGameplay || !IsTracking || inventory == null) return;
            if (!Ready) { GameplayFeedback.Show("food.refillWait"); return; }
            InventoryItem prefab = Resources.Load<InventoryItem>("Inventory/Items/ShavermaDropped");
            if (prefab == null) { GameplayFeedback.Show("trade.unavailable"); return; }
            InventoryItem meal = Instantiate(prefab, inventory.transform, false);
            meal.SetCount(1);
            meal.sprite = meal.GetComponentInChildren<SpriteRenderer>(true);
            LootBoxItem held = meal.GetComponent<LootBoxItem>();
            if (held != null) held.SetHeldId("refill." + Id);
            if (!inventory.Add(meal)) { Destroy(meal.gameObject); GameplayFeedback.Show("feedback.full"); return; }
            readyAt = Time.time + FoodRefills.RefillSeconds;
            GameplayFeedback.Show("food.refillTaken");
            RunSaveService.RequestSave();
        }
    }
}
