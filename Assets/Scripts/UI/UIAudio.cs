using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.UI
{
    public sealed class UIAudio : MonoBehaviour
    {
        private static UIAudio instance;
        private AudioSource cues, music;
        private AudioClip hover, click, pause, resume;
        private float lastHover = -1;
        private float lastClick = -1;
        private bool paused;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { instance = null; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            instance = new GameObject("Interface audio").AddComponent<UIAudio>();
            DontDestroyOnLoad(instance.gameObject);
        }
        private void Awake()
        {
            cues = gameObject.AddComponent<AudioSource>();
            cues.playOnAwake = false; cues.spatialBlend = 0; cues.ignoreListenerPause = true;
            music = gameObject.AddComponent<AudioSource>();
            music.playOnAwake = false; music.spatialBlend = 0; music.loop = true; music.volume = 0.3f;
            hover = Resources.Load<AudioClip>("Audio/UI/hover");
            click = Resources.Load<AudioClip>("Audio/UI/click");
            pause = Resources.Load<AudioClip>("Audio/UI/pause");
            resume = Resources.Load<AudioClip>("Audio/UI/resume");
            GameAudioSettings settings = Resources.Load<GameAudioSettings>("Audio/GameAudioSettings");
            if (settings != null) music.clip = settings.menuMusic;
            SceneManager.sceneLoaded += OnLoaded;
            GameSession.Changed += OnSession;
        }
        private void OnLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            if (scene.path == "Assets/menu.unity" && music.clip != null) music.Play();
            else music.Stop(); // Main keeps the original scene-based music and area effects.
        }
        private void OnSession()
        {
            if (GameSession.IsPaused != paused)
            {
                if (!GameSession.IsOver && !GameSession.IsLoading) Play(GameSession.IsPaused ? pause : resume);
                paused = GameSession.IsPaused;
            }
        }
        private void Play(AudioClip clip) { if (clip != null) cues.PlayOneShot(clip, GamePreferences.UiVolume); }
        public static void Hover()
        {
            if (instance == null || Time.unscaledTime - instance.lastHover < 0.075f) return;
            instance.lastHover = Time.unscaledTime; instance.Play(instance.hover);
        }
        public static void Click()
        {
            if (instance == null || Time.unscaledTime - instance.lastClick < 0.04f) return;
            instance.lastClick = Time.unscaledTime; instance.Play(instance.click);
        }
        private void OnDestroy() { SceneManager.sceneLoaded -= OnLoaded; GameSession.Changed -= OnSession; if (instance == this) instance = null; }
    }
}
