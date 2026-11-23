using StudentSurvival.Controls;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public enum StudentTradeOffer { Drink, Tip }

    // Authored corridor actors use the same explicit interaction/targeting path as pickups and doors.
    [DisallowMultipleComponent]
    public sealed class StudentTrade : PlayerInteractionTarget
    {
        [SerializeField] private StudentTradeOffer offer;
        private InventoryController inventory;
        private CampaignState campaign;
        private Collider2D interaction;
        private SpriteRenderer artwork;
        private bool configured, interacting;
        private bool Available
        {
            get { return configured && interaction != null && interaction.enabled && interaction.isTrigger &&
                artwork != null && artwork.sprite != null && artwork.enabled && !artwork.forceRenderingOff && artwork.gameObject.activeInHierarchy; }
        }
        private bool GivesDrink { get { return offer == StudentTradeOffer.Drink; } }
        private bool Claimed { get { return GivesDrink ? CampaignState.DrinkTradeCompleted : CampaignState.TipTradeCompleted; } }
        public override string PromptKey
        {
            get
            {
                if (Claimed) return GivesDrink ? "trade.already" : "trade.tipRepeat";
                if (!Available || inventory == null || campaign == null) return "trade.unavailable";
                if (!HasCollectedLab()) return "trade.noLab";
                return GivesDrink ? "trade.drinkPrompt" : "trade.tipPrompt";
            }
        }

        private void Awake()
        {
            inventory = FindFirstObjectByType<InventoryController>();
            interaction = GetComponent<Collider2D>();
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
                if (renderer.sprite != null && renderer.enabled && !renderer.forceRenderingOff && renderer.gameObject.activeInHierarchy)
                { artwork = renderer; break; }
            configured = interaction != null && interaction.enabled && interaction.isTrigger && artwork != null;
            if (!configured)
                Debug.LogWarning("StudentTrade needs an enabled trigger collider on the actor and visible authored student artwork. Trading is unavailable until the actor is configured.", this);
        }
        private void Start()
        {
            // CampaignState is installed after the authored scene's Awake callbacks.
            campaign = FindFirstObjectByType<CampaignState>();
        }
        private bool HasCollectedLab()
        {
            if (inventory == null) return false;
            for (int i = 0; i < GameInput.SlotCount; i++)
            {
                InventoryItem item = inventory.ItemAt(i);
                if (item != null && item.itemType == Item.Lab && item.Count > 0) return true;
            }
            return false;
        }
        public override void Interact(PlayerInteractor player)
        {
            if (interacting || player == null || !HasPlayerNearby || GameSession.BlocksGameplay) return;
            if (Claimed) { GameplayFeedback.Show(GivesDrink ? "trade.already" : "trade.tipReceived"); return; }
            if (!Available || inventory == null || campaign == null) { GameplayFeedback.Show("trade.unavailable"); return; }
            if (!HasCollectedLab()) { GameplayFeedback.Show("trade.noLab"); return; }
            interacting = true;
            try
            {
                if (GivesDrink)
                {
                    if (!inventory.TryTradeCollectedLabForEnergy(out string failureKey))
                    { GameplayFeedback.Show(failureKey ?? "trade.unavailable"); return; }
                }
                else if (!inventory.SpendCollectedLab()) { GameplayFeedback.Show("trade.noLab"); return; }
                // No async work or gameplay/stat effects occur between delivery and the receipt.
                // Both offers use the shared per-run receipt, including multiple actors of one offer.
                if (!CampaignState.TryCompleteStudentTrade(GivesDrink))
                {
                    Debug.LogError("StudentTrade delivered an exchange but could not record its campaign receipt. Check CampaignState lifetime and inventory callbacks.", this);
                    return;
                }
                GameplayFeedback.Show(GivesDrink ? "trade.drinkReceived" : "trade.tipReceived");
            }
            finally { interacting = false; }
        }
    }
}
