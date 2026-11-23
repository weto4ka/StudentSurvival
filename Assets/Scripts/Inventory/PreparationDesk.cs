using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    // The authored desk supplies artwork/collision; this component only owns study interaction.
    public sealed class PreparationDesk : PlayerInteractionTarget
    {
        private BoxCollider2D interaction;
        public override string PromptKey => "preparation.prompt";
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if(mode==LoadSceneMode.Single)Attach(scene); }
        private static void Attach(Scene scene)
        {
            if(scene.path!="Assets/Main.unity"||FindFirstObjectByType<PreparationDesk>(FindObjectsInactive.Include)!=null)return;
            Debug.LogError("Main scene needs the authored oral-exam preparation desk.");
        }
        private void Awake()
        {
            interaction=GetComponent<BoxCollider2D>();
            if(interaction==null){Debug.LogError("Preparation desk needs its authored interaction collider.",this);enabled=false;return;}
            RefreshAvailability();
        }
        private void OnEnable(){CampaignState.Changed+=RefreshAvailability;RefreshAvailability();}
        protected override void OnDisable(){CampaignState.Changed-=RefreshAvailability;base.OnDisable();}
        private void RefreshAvailability()
        {
            if(interaction==null)return;
            interaction.enabled=CampaignState.IsPreparationStage&&CampaignState.CanSpendPreparation;
            if(!interaction.enabled)base.OnDisable();
        }
        public override void Interact(PlayerInteractor player)
        {
            if(player==null||GameSession.BlocksGameplay||!HasPlayerNearby)return;
            if(!CampaignState.TryPrepare()){GameplayFeedback.Show("preparation.exhausted");return;}
            GameplayFeedback.Show(CampaignState.HasRelationship?"preparation.friendReady":"preparation.success");
            RunSaveService.RequestSave();
        }
    }
}
