using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StudentSurvival.Editor
{
    [InitializeOnLoad]
    public static class GameStartup
    {
        private static string Preference { get { return "StudentSurvival.StartAtMenu." + Hash128.Compute(Application.dataPath); } }
        static GameStartup() { EditorApplication.delayCall += Apply; }
        private static void Apply()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            EditorSceneManager.playModeStartScene = EditorPrefs.GetBool(Preference, true)
                ? AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/menu.unity") : null;
        }
        [MenuItem("Tools/Student Survival/Start Play Mode at menu")]
        private static void Menu() { EditorPrefs.SetBool(Preference, true); Apply(); }
        [MenuItem("Tools/Student Survival/Start Play Mode at open scene")]
        private static void OpenScene() { EditorPrefs.SetBool(Preference, false); Apply(); }
    }
}
