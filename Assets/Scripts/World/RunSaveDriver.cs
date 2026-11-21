using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.World
{
    public sealed class RunSaveDriver : MonoBehaviour
    {
        private void Awake() { SceneManager.sceneLoaded += Loaded; }
        private void OnDestroy() { SceneManager.sceneLoaded -= Loaded; }
        private void Loaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            StopAllCoroutines();
            RunSaveService.SceneLoading(scene);
            StartCoroutine(Initialize(scene));
        }
        private IEnumerator Initialize(Scene scene)
        {
            yield return null; // Let all scene Start/bootstrap callbacks construct their participants.
            if (!scene.isLoaded || SceneManager.GetActiveScene() != scene) yield break;
            RunSaveService.InitializeScene(scene);
        }
        private void LateUpdate() { RunSaveService.Tick(); }
        private void OnApplicationPause(bool paused) { if (paused) RunSaveService.SaveNow(); }
        private void OnApplicationFocus(bool focused) { if (!focused) RunSaveService.SaveNow(); }
        private void OnApplicationQuit() { RunSaveService.SaveNow(); }
    }
}
