using UnityEngine;
namespace StudentSurvival.Characters
{
    public sealed class CampaignRoomArt : ScriptableObject
    {
        [SerializeField] private Sprite classroom;
        [SerializeField] private Sprite corridor;
        public Sprite ForStage(int stage) { return stage == 3 ? corridor : classroom; }
    }
}
