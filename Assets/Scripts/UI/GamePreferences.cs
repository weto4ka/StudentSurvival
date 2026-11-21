using System;
using UnityEngine;

namespace StudentSurvival.UI
{
    public static class GamePreferences
    {
        public static float Volume { get; private set; }
        public static float TextScale { get; private set; }
        public static float UiVolume { get; private set; }
        public static bool MinimalTitleArt { get; private set; } = true;
        public static bool FullscreenRequested { get; private set; }
        public static bool CanChangeWindowMode { get { return !Application.isEditor; } }
        private static int windowWidth = 1280, windowHeight = 720;
        public static event Action Changed;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Changed = null;
            MinimalTitleArt = PlayerPrefs.GetInt("StudentSurvival.MinimalTitleArt", 1) != 0;
            Volume = Mathf.Clamp01(PlayerPrefs.GetFloat("StudentSurvival.Volume", 0.8f));
            TextScale = Mathf.Clamp(PlayerPrefs.GetFloat("StudentSurvival.TextScale", 1f), 0.85f, 1.15f);
            AudioListener.volume = Volume;
            UiVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("StudentSurvival.UiVolume", 0.6f));
            FullscreenRequested = PlayerPrefs.GetInt("StudentSurvival.Fullscreen", Screen.fullScreen ? 1 : 0) != 0;
            if (CanChangeWindowMode && PlayerPrefs.HasKey("StudentSurvival.Fullscreen")) ApplyWindowMode();
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
        public static void ToggleTitleArt()
        { MinimalTitleArt = !MinimalTitleArt; PlayerPrefs.SetInt("StudentSurvival.MinimalTitleArt", MinimalTitleArt ? 1 : 0); Changed?.Invoke(); }
        public static void Save() { PlayerPrefs.Save(); }
        public static void SetUiVolume(float value)
        {
            UiVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("StudentSurvival.UiVolume", UiVolume);
        }
        public static void ToggleFullscreen()
        {
            if (!CanChangeWindowMode) return;
            if (!FullscreenRequested) { windowWidth = Screen.width; windowHeight = Screen.height; }
            FullscreenRequested = !FullscreenRequested;
            PlayerPrefs.SetInt("StudentSurvival.Fullscreen", FullscreenRequested ? 1 : 0);
            ApplyWindowMode();
            Changed?.Invoke();
        }
        private static void ApplyWindowMode()
        {
            if (FullscreenRequested)
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
            else Screen.SetResolution(windowWidth, windowHeight, FullScreenMode.Windowed);
        }
    }
}
