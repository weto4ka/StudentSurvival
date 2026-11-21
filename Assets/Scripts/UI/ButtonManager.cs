using StudentSurvival.World;
using UnityEngine;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    // Existing scene callbacks delegate to the same loading path as the localized menu.
    public class ButtonManager : MonoBehaviour
    {
        public GameObject loadingScreen;
        public Slider slider;
        public void LoadLevel(int sceneIndex) { GameSession.Load(sceneIndex); }
        public void ExitGameBtn() { Application.Quit(); }
    }
}
