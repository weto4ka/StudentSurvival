using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public class LootBoxItem : PlayerInteractionTarget
    {
        private InventoryController controller;
        private InventoryItem item;
        [SerializeField] private string persistentId;
        public string PersistentId { get { return persistentId; } }
        public void SetHeldId(string id) { persistentId = id; }
        public override string PromptKey { get { return "pickup.prompt"; } }
        public override string ItemKey { get { return item != null ? item.NameKey : null; } }
        private void Awake()
        {
            GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
            if (inventory != null) controller = inventory.GetComponent<InventoryController>();
            item = GetComponent<InventoryItem>();
            if (GetComponentInParent<InventoryController>() != null) return;
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
        public override void Interact(PlayerInteractor player)
        {
            if (GameSession.BlocksGameplay || !HasPlayerNearby || controller == null || item == null) return;
            string nameKey = item.NameKey;
            GameplayFeedback.Show(controller.Add(item) ? "feedback.pickedUp" : "feedback.full", nameKey);
        }
    }
}
