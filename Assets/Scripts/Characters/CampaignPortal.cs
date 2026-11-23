using StudentSurvival.World;
using UnityEngine;
namespace StudentSurvival.Characters
{
    public sealed class CampaignPortal : PlayerInteractionTarget
    {
        [SerializeField] private Vector3 destination;
        [SerializeField] private int destinationStage;
        [SerializeField] private bool useForwardArrival;
        [SerializeField] private bool useToiletArrival;
        [SerializeField, Range(1, 3)] private int unlockedAt = 1;
        [SerializeField] private string interactionPromptKey;
        private SpriteRenderer status;
        private bool Available { get { return CampaignState.Stage>=unlockedAt; } }
        // Doors only lock until their exam is passed. During an exam they stay usable and
        // ExamProgression cancels the exam when the player arrives outside the exam room.
        private const string LockedKey="campaign.portalNeedsExam";
        public override string PromptKey { get { return !Available?LockedKey:CampaignState.ExamActive?"campaign.leaveExamPrompt":(string.IsNullOrEmpty(interactionPromptKey)?"campaign.portal":interactionPromptKey); } }
        public void Configure(Vector3 target,int minimumStage)
        {
            destination=target;unlockedAt=minimumStage;
            Transform marker=transform.Find("Door status");
            status=marker!=null?marker.GetComponent<SpriteRenderer>():null;
            RefreshStatus();
        }
        private void Awake(){Transform marker=transform.Find("Door status");status=marker!=null?marker.GetComponent<SpriteRenderer>():null;RefreshStatus();}
        private void OnEnable(){CampaignState.Changed+=RefreshStatus;RefreshStatus();}
        protected override void OnDisable(){CampaignState.Changed-=RefreshStatus;base.OnDisable();}
        private void RefreshStatus(){if(status!=null)status.color=Available?new Color(.48f,.84f,.62f):new Color(.75f,.34f,.23f);}
        public override void Interact(PlayerInteractor player)
        {
            if(GameSession.BlocksGameplay||!HasPlayerNearby||player==null)return;
            if(!Available){GameplayFeedback.Show(LockedKey);return;}
            Rigidbody2D body=player.GetComponent<Rigidbody2D>();if(body==null)return;
            Vector3 arrival=destination;
            if(destinationStage>0)
            {
                CampaignRooms room=CampaignRooms.Find(destinationStage);
                Transform point=room!=null?(useToiletArrival?room.ToiletArrival:useForwardArrival?room.ForwardArrival:room.BackArrival):null;
                if(point==null){Debug.LogError("Campaign door has no authored arrival marker.",this);return;}
                arrival=point.position;
            }
            body.linearVelocity=Vector2.zero;body.position=arrival;
            RunSaveService.CreateCheckpoint(arrival);
        }
    }
}
