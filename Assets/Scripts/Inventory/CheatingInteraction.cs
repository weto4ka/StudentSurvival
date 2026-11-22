using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Inventory
{
    public enum CheatingInteractionKind { Notes, Study, UseNotes }

    public sealed class CheatingInteraction : PlayerInteractionTarget
    {
        private CheatingInteractionKind kind;
        private BoxCollider2D interaction;
        public override string PromptKey { get { return kind == CheatingInteractionKind.Notes ? "cheat.notesPrompt" : kind == CheatingInteractionKind.Study ? "cheat.studyPrompt" : "cheat.usePrompt"; } }
        private void Awake()
        {
            interaction = gameObject.AddComponent<BoxCollider2D>();
            interaction.isTrigger = true;
            interaction.size = new Vector2(1.35f, 2.8f);
            interaction.offset = Vector2.up * .7f;
        }
        public void Configure(CheatingInteractionKind value) { kind = value; }
        public void SetAvailable(bool available)
        {
            if (interaction == null || interaction.enabled == available) return;
            interaction.enabled = available;
            if (!available) base.OnDisable();
        }
        public override void Interact(PlayerInteractor player)
        {
            if (player == null || GameSession.BlocksGameplay || !HasPlayerNearby || CampaignState.Stage != 2) return;
            bool accepted = kind == CheatingInteractionKind.Notes ? CampaignState.TryCollectCheatNotes()
                : kind == CheatingInteractionKind.Study ? CampaignState.TryStudyStage2() : CampaignState.TryUseCheatNotes();
            // Applying notes may synchronously finish the exam; preserve its stage-passed feedback.
            if (accepted && CampaignState.Stage == 2) GameplayFeedback.Show(kind == CheatingInteractionKind.Notes ? "cheat.collected" : kind == CheatingInteractionKind.Study ? "cheat.studied" : "cheat.used");
        }
    }
}
