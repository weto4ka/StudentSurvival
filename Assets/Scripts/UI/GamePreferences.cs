using System;
using UnityEngine;

namespace StudentSurvival.UI
{
    public static class GamePreferences
    {
        public static float Volume { get; private set; }
        public static float TextScale { get; private set; }
        public static event Action Changed;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Changed = null;
            Volume = Mathf.Clamp01(PlayerPrefs.GetFloat("StudentSurvival.Volume", 0.8f));
            TextScale = Mathf.Clamp(PlayerPrefs.GetFloat("StudentSurvival.TextScale", 1f), 0.85f, 1.15f);
            AudioListener.volume = Volume;
            if (PlayerPrefs.HasKey("StudentSurvival.Fullscreen")) Screen.fullScreen = PlayerPrefs.GetInt("StudentSurvival.Fullscreen") != 0;
        }
        public static void SetVolume(float value)
        {
            Volume = AudioListener.volume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("StudentSurvival.Volume", Volume);
        }
        public static void SetTextScale(float value)
        {
            TextScale = Mathf.Clamp(value, 0.85f, 1.15f);
            PlayerPrefs.SetFloat("StudentSurvival.TextScale", TextScale);
            Changed?.Invoke();
        }
        public static void Save() { PlayerPrefs.Save(); }
        public static void ToggleFullscreen()
        {
            bool fullscreen = !Screen.fullScreen;
            PlayerPrefs.SetInt("StudentSurvival.Fullscreen", fullscreen ? 1 : 0);
            Screen.fullScreen = fullscreen;
        }
    }
}
