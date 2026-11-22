using StudentSurvival.World;
using UnityEngine;
namespace StudentSurvival.Characters
{
    public sealed class CampaignExamAdmission : PlayerInteractionTarget
    {
        private int stage;
        private BoxCollider2D trigger;
        private bool available;
        public override string PromptKey { get { return CampaignState.CanStartExam(stage)?"campaign.examStart":"campaign.examNotReady"; } }
        public void Configure(int examStage)
        {
            stage=examStage;transform.localPosition=new Vector3(-.9f,0,0);
            trigger=gameObject.AddComponent<BoxCollider2D>();trigger.isTrigger=true;trigger.offset=Vector2.up*1.4f;trigger.size=new Vector2(1.6f,2.8f);
            Refresh();
        }
        private void Update(){Refresh();}
        private void Refresh()
        {
            if(trigger==null)return;
            bool next=CampaignState.Stage==stage&&!CampaignState.ExamActive;
            if(next==available&&trigger.enabled==next)return;
            available=next;trigger.enabled=next;if(!next)base.OnDisable();
        }
        public override void Interact(PlayerInteractor player)
        {
            if(GameSession.BlocksGameplay||!HasPlayerNearby||player==null||ExamProgression.Instance==null)return;
            ExamProgression.Instance.StartExam(stage);Refresh();
        }
    }
}
