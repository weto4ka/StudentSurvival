using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.UI
{
    public static class GameUIBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnLoaded;
            SceneManager.sceneLoaded += OnLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" && scene.path != "Assets/menu.unity") return;
            if (Object.FindFirstObjectByType<GameUI>() != null) return;
            new GameObject("Game interface").AddComponent<GameUI>();
        }
    }
}
