using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.UI
{
    // Preserve the jam's system-error joke as localized feedback, without hiding
    // the HUD, using out-of-range colors, or spending a lab to dismiss an image.
    public class WindowsError : MonoBehaviour
    {
        [SerializeField] private bool showed;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (showed || GameSession.BlocksGameplay || other.GetComponentInParent<Character>() == null) return;
            showed = true;
            GameplayFeedback.Show("feedback.glitch");
        }
    }
}
