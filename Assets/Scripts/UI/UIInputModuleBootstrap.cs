using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace StudentSurvival.UI
{
    /// <summary>
    /// The scenes still carry the old StandaloneInputModule, which reads the legacy Input Manager and stops
    /// working as soon as the Input System package is the active input handler. This swaps it for the
    /// InputSystemUIInputModule whenever a scene is loaded, so the menu buttons and sliders keep working.
    /// Replacing the component once in the Inspector (the "Replace with InputSystemUIInputModule" button
    /// on the EventSystem) makes this a no-op for that scene.
    /// </summary>
    public static class UIInputModuleBootstrap
    {
#if ENABLE_INPUT_SYSTEM
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            ReplaceLegacyModules();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ReplaceLegacyModules();
        }

        private static void ReplaceLegacyModules()
        {
            EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

            foreach (EventSystem eventSystem in eventSystems)
            {
                EnsureInputModule(eventSystem);
            }
        }
#endif
        public static void EnsureInputModule(EventSystem eventSystem)
        {
#if ENABLE_INPUT_SYSTEM
            StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule != null)
            {
                legacyModule.enabled = false;
                Object.Destroy(legacyModule);
            }
            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                InputSystemUIInputModule module = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                module.AssignDefaultActions();
            }
#endif
        }
    }
}
