using StudentSurvival.World;
using UnityEngine;
namespace StudentSurvival.Characters
{
    public sealed class CampaignPortal : PlayerInteractionTarget
    {
        private Vector3 destination;
        private int unlockedAt;
        private bool Available { get { return CampaignState.Stage>=unlockedAt&&!CampaignState.ExamActive; } }
        public override string PromptKey { get { return Available?"campaign.portal":"campaign.portalLocked"; } }
        public void Configure(Vector3 target,int minimumStage)
        {
            destination=target;unlockedAt=minimumStage;
            BoxCollider2D trigger=gameObject.AddComponent<BoxCollider2D>();trigger.isTrigger=true;trigger.size=new Vector2(1.1f,2.6f);trigger.offset=Vector2.up*1.3f;
        }
        public override void Interact(PlayerInteractor player)
        {
            if(GameSession.BlocksGameplay||!HasPlayerNearby||player==null)return;
            if(!Available){GameplayFeedback.Show("campaign.portalLocked");return;}
            Rigidbody2D body=player.GetComponent<Rigidbody2D>();if(body==null)return;
            body.linearVelocity=Vector2.zero;body.position=destination;
            RunSaveService.CreateCheckpoint(destination);
        }
    }
}
