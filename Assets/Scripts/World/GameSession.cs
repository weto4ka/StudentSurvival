using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.World
{
    public enum SessionResult { Playing, Won, Lost }
    public static class GameSession
    {
        public static SessionResult Result { get; private set; }
        public static bool IsOver { get { return Result != SessionResult.Playing; } }
        public static bool IsPaused { get; private set; }
        public static bool IsLoading { get; private set; }
        public static bool BlocksGameplay { get { return IsOver || IsPaused || IsLoading; } }
        public static string LossReasonKey { get; private set; }
        public static int PickupsCollected { get; private set; }
        public static void RecordPickup() { PickupsCollected++; Changed?.Invoke(); }
        public static event Action Changed;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Changed = null;
            Reset();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        { if (mode == LoadSceneMode.Single) { Reset(); Changed?.Invoke(); } }
        private static void Reset()
        {
            Result = SessionResult.Playing;
            IsPaused = false;
            IsLoading = false;
            LossReasonKey = null;
            PickupsCollected = 0;
            GameplayFeedback.Clear();
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
        public static void SetPaused(bool paused)
        {
            if (IsOver || IsLoading || IsPaused == paused) return;
            IsPaused = paused;
            Time.timeScale = paused ? 0 : 1;
            AudioListener.pause = paused;
            Changed?.Invoke();
        }
        public static void Win() { End(SessionResult.Won, null); }
        public static void Lose(string reasonKey = "result.noIntelligence") { End(SessionResult.Lost, reasonKey); }
        private static void End(SessionResult result, string reason)
        {
            if (IsOver || IsLoading) return;
            Result = result;
            LossReasonKey = reason;
            IsPaused = false;
            Time.timeScale = 0;
            AudioListener.pause = true;
            Changed?.Invoke();
            new GameObject("GameSessionControls").AddComponent<GameSessionControls>();
        }
        public static void Restart() { Load(SceneManager.GetActiveScene().buildIndex); }
        public static void BackToMenu() { Load(0); }
        public static void Load(int index)
        {
            if (IsLoading || index < 0 || index >= SceneManager.sceneCountInBuildSettings) return;
            IsLoading = true;
            Time.timeScale = 1;
            AudioListener.pause = false;
            SceneManager.LoadSceneAsync(index);
            Changed?.Invoke();
        }
    }
}
