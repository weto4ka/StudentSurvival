using StudentSurvival.Controls;
using StudentSurvival.Inventory;
using StudentSurvival.Localization;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.UI
{
    public sealed partial class GameUI
    {
        private int previousGuardSeconds = -1;
        private void UpdateCampaignRoute(string warningKey)
        {
            if (routeText == null || player == null || route == null || GameSession.BlocksGameplay || Time.unscaledTime < nextRouteUpdate) return;
            nextRouteUpdate = Time.unscaledTime + .35f;
            bool transientFeedback = Time.unscaledTime < GameplayFeedback.ExpiresAt;
            bool hide = exams == null || GameSession.IsOver || !string.IsNullOrEmpty(warningKey) || transientFeedback ||
                (interactor != null && interactor.Target is FlirtDistraction);
            routeText.gameObject.SetActive(!hide);
            if (hide) { markedLab = null; if (labMarker != null) labMarker.gameObject.SetActive(false); return; }
            if (CampaignState.Completed)
            {
                routeText.text = GameLocalization.Get("campaign.explorationHint"); markedLab = null;
                if (labMarker != null) labMarker.gameObject.SetActive(false);
                return;
            }
            int stage = CampaignState.Stage;
            bool movingRooms = stage == 2 && player.transform.position.x < 17f || stage == 3 && player.transform.position.x < 37f;
            Transform lab = stage == 1 && !CampaignState.ExamActive ? FindNearestLab() : null;
            bool nearFirst = stage == 1 && teacher != null && Mathf.Abs(player.transform.position.y - teacher.transform.position.y) < 3 &&
                Mathf.Abs(player.transform.position.x - teacher.transform.position.x) < 10;
            Vector3 goal = exams.NextObjectivePosition;
            string key;
            if (movingRooms) key = "campaign.moveOn";
            else if (CampaignState.ExamActive) key = "campaign.objectiveFight";
            else if (stage == 1)
            {
                key = "campaign.objective1";
                if (lab != null && !nearFirst) goal = lab.position;
            }
            else if (stage == 2)
            {
                key = CampaignState.CanStartExam(2) ? "campaign.objectiveReady" : "campaign.objective2";
                if (!CampaignState.CanStartExam(2)) goal = new Vector3(player.transform.position.x < 23.5f ? 22 : 25.5f, 3.04f, 0);
            }
            else if (CampaignState.CanClaimRelationshipHelp)
            { key = "campaign.objectiveFriend"; goal = new Vector3(42, 3.04f, 0); }
            else if (CampaignState.PreparationRemaining > 0)
            {
                key = "campaign.objective3";
                goal = new Vector3(CampaignState.CanFlirt && player.transform.position.x < 44 ? 42 : 46.5f, 3.04f, 0);
            }
            else key = "campaign.objectiveReady";
            Vector3 destination = movingRooms || stage == 1 ? route.Next(player.transform.position, goal) : goal;
            string arrow = Mathf.Abs(destination.x - player.transform.position.x) < 1.3f ? "↓" : destination.x < player.transform.position.x ? "←" : "→";
            routeText.text = GameLocalization.Get(key, arrow, GameInput.Display("Action"), CampaignState.PreparationRemaining, CampaignState.PreparedPoints);
            markedLab = lab != null && !nearFirst && !CampaignState.ExamActive ? lab : null;
            UpdateLabMarker();
        }
        private string InteractionPrompt(string key)
        {
            return GameLocalization.Get(key, GameInput.Display("Action"));
        }
    }
}
