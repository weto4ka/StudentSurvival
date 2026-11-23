using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public class LootBoxItem : PlayerInteractionTarget
    {
        private InventoryController controller;
        private InventoryItem item;
        private bool worldPickup, collected;
        [SerializeField] private string persistentId;
        public string PersistentId { get { return persistentId; } }
        public void SetHeldId(string id) { persistentId = id; }
        public bool IsAvailable { get { return worldPickup && !collected && item != null && controller != null && isActiveAndEnabled; } }
        public override string PromptKey { get { return !IsAvailable ? null : item.itemType == Item.Shaverma ? "pickup.foodPrompt" : "pickup.prompt"; } }
        public override string ItemKey { get { return item != null ? item.NameKey : null; } }
        private void Awake()
        {
            GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
            if (inventory != null) controller = inventory.GetComponent<InventoryController>();
            item = GetComponent<InventoryItem>();
            if (GetComponentInParent<InventoryController>() != null) { MarkCollected(); return; }
            worldPickup = true;
            if (string.IsNullOrEmpty(persistentId))
            {
                // Cache the authored path before pickup reparenting or runtime sibling changes.
                string path = "";
                for (Transform node = transform; node != null; node = node.parent)
                    path = "/" + node.name + "[" + node.GetSiblingIndex() + "]" + path;
                persistentId = gameObject.scene.path + path;
            }
            RunSaveService.RegisterPickup(persistentId, gameObject);
        }
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (IsAvailable) base.OnTriggerEnter2D(other);
        }
        private void OnEnable()
        {
            // Held resource clones may be activated by unrelated presentation code. They
            // must never turn back into a world pickup or advertise a depleted food spot.
            if (collected || GetComponentInParent<InventoryController>() != null) MarkCollected();
        }
        internal void MarkCollected()
        {
            collected = true;
            worldPickup = false;
            base.OnDisable(); // Clear the current prompt before inventory callbacks/reparenting.
            enabled = false;
            foreach (Collider2D trigger in GetComponentsInChildren<Collider2D>(true)) trigger.enabled = false;
        }
        public override void Interact(PlayerInteractor player)
        {
            if (player == null || GameSession.BlocksGameplay || !IsAvailable || !HasPlayerNearby) return;
            string nameKey = item.NameKey;
            bool food = item.itemType == Item.Shaverma;
            bool added = controller.Add(item);
            if (!added) GameplayFeedback.Show("feedback.full");
            else if (food) GameplayFeedback.Show("feedback.foodCollected");
            else GameplayFeedback.Show("feedback.pickedUp", nameKey);
        }
    }
}
