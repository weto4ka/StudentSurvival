using UnityEngine;
using StudentSurvival.Controls;

namespace StudentSurvival.World
{
    /// <summary>
    /// Created by GameSession when a level is decided. Listens for the restart and back-to-menu buttons.
    /// It lives in the scene, so it disappears together with it when the level is reloaded.
    /// </summary>
    public sealed class GameSessionControls : MonoBehaviour
    {
        private void Update()
        {
            // Result shortcuts must not remain active after resuming the completed campus.
            if (!GameSession.IsOver) { Destroy(gameObject); return; }
            if (GameInput.RestartPressed)
            {
                GameSession.Restart();
            }
            else if (GameInput.BackToMenuPressed)
            {
                if (GameSession.Result == SessionResult.Won) GameSession.KeepExploring();
                else GameSession.BackToMenu();
            }
        }
    }
}
