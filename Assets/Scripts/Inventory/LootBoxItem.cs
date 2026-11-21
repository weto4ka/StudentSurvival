using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public class LootBoxItem : PlayerInteractionTarget
    {
        private InventoryController controller;
        private InventoryItem item;
        public override string PromptKey { get { return "pickup.prompt"; } }
        public override string ItemKey { get { return item != null ? item.NameKey : null; } }
        private void Awake()
        {
            GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
            if (inventory != null) controller = inventory.GetComponent<InventoryController>();
            item = GetComponent<InventoryItem>();
        }
        public override void Interact(PlayerInteractor player)
        {
            if (GameSession.BlocksGameplay || !HasPlayerNearby || controller == null || item == null) return;
            string nameKey = item.NameKey;
            GameplayFeedback.Show(controller.Add(item) ? "feedback.pickedUp" : "feedback.full", nameKey);
        }
    }
}
