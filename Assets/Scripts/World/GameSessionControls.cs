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
            if (GameInput.RestartPressed)
            {
                GameSession.Restart();
            }
            else if (GameInput.BackToMenuPressed)
            {
                GameSession.BackToMenu();
            }
        }
    }
}
