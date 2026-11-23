using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    public enum KitchenAppliance { Stove, Coffee }

    // The hostel kitchen (upstairs, orange room; left in level.png, right in game because the art is flipped) is background art, so
    // this bootstrap places invisible triggers over the stove and the coffee machine. Authored
    // HostelKitchen objects placed in Main replace the bootstrap.
    public sealed class HostelKitchen : PlayerInteractionTarget
    {
        // Measured from level.png (107 px/unit). The Main "level" renderer is centred at x=-64.15
        // with flipX on, so image positions are mirrored into the orange upstairs kitchen.
        private static readonly Vector3 StovePosition = new Vector3(-40.5f, 2.76f, 0f);
        private static readonly Vector3 CoffeePosition = new Vector3(-42.5f, 2.76f, 0f);
        // Cooking gives one carried shawarma (+1 Sleep when eaten); coffee is the energy-drink
        // effect (+3 borrowed Sleep and speed for 20 s, then a crash). Cooldowns keep both from
        // becoming an unlimited refill; they reset on scene reload, which is acceptable.
        private const float StoveCooldown = 90f, CoffeeCooldown = 150f;
        [SerializeField] private KitchenAppliance appliance;
        private float readyAt;
        private InventoryController inventory;
        private Character player;
        private bool Ready => Time.time >= readyAt;
        public override string PromptKey => !Ready ? "kitchen.tooSoon" : appliance == KitchenAppliance.Stove ? "kitchen.cookPrompt" : "kitchen.coffeePrompt";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<HostelKitchen>(FindObjectsInactive.Include) != null) return;
            Create("Hostel kitchen - stove", StovePosition, KitchenAppliance.Stove);
            Create("Hostel kitchen - coffee machine", CoffeePosition, KitchenAppliance.Coffee);
        }
        private static void Create(string label, Vector3 position, KitchenAppliance kind)
        {
            var target = new GameObject(label);
            target.transform.position = position;
            target.AddComponent<HostelKitchen>().appliance = kind;
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
        private void Start()
        {
            inventory = FindFirstObjectByType<InventoryController>();
            player = FindFirstObjectByType<Character>();
        }
        public override void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || !HasPlayerNearby || GameSession.BlocksGameplay || player == null) return;
            if (!Ready) { GameplayFeedback.Show("kitchen.tooSoon"); return; }
            if (appliance == KitchenAppliance.Coffee)
            {
                // TryDrinkEnergy explains itself when a boost is already running.
                if (!player.TryDrinkEnergy()) return;
                readyAt = Time.time + CoffeeCooldown;
                GameplayFeedback.Show("kitchen.coffeeDone");
                return;
            }
            InventoryItem prefab = inventory != null ? Resources.Load<InventoryItem>("Inventory/Items/ShavermaDropped") : null;
            if (prefab == null) { GameplayFeedback.Show("trade.unavailable"); return; }
            // Same path as the friend's gift: instantiate under the inventory so it is never a world pickup.
            InventoryItem meal = Instantiate(prefab, inventory.transform, false);
            meal.SetCount(1);
            meal.sprite = meal.GetComponentInChildren<SpriteRenderer>(true);
            LootBoxItem pickup = meal.GetComponent<LootBoxItem>();
            if (pickup != null) pickup.SetHeldId("hostel.kitchen.cooked");
            if (!inventory.Add(meal)) { Destroy(meal.gameObject); GameplayFeedback.Show("feedback.full"); return; }
            readyAt = Time.time + StoveCooldown;
            GameplayFeedback.Show("kitchen.cookDone");
        }
    }
}
