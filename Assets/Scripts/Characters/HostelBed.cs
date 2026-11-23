using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Characters
{
    // The dormitory bed is part of the hostel background art (Assets/layout/level.png), so this
    // bootstrap places an invisible interaction trigger over it. An authored HostelBed placed in
    // Main replaces the bootstrap; move that object to adjust the position.
    public sealed class HostelBed : PlayerInteractionTarget
    {
        // Measured from level.png (107 px/unit). The Main "level" renderer is centred at x=-64.15
        // with flipX on, so image positions are mirrored: the bed spans x≈-83.6..-81.3 in the
        // purple upstairs bedroom; the upstairs floor is y≈2.76.
        private static readonly Vector3 DefaultPosition = new Vector3(-82.4f, 2.76f, 0f);
        private const int RestedSleep = 10, RestedIntellect = 5;
        private const float Cooldown = 120f;
        private float readyAt;
        private Character player;
        private bool Ready => Time.time >= readyAt;
        public override string PromptKey => Ready ? "bed.prompt" : "bed.readyIn";
        public override float CooldownSeconds => Mathf.Max(0f, readyAt - Time.time);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || FindFirstObjectByType<HostelBed>(FindObjectsInactive.Include) != null) return;
            var bed = new GameObject("Hostel bed - sleep");
            bed.transform.position = DefaultPosition;
            bed.AddComponent<HostelBed>();
        }
        private void Awake()
        {
            BoxCollider2D trigger = GetComponent<BoxCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<BoxCollider2D>();
                trigger.size = new Vector2(2.8f, 2.4f); trigger.offset = new Vector2(0f, 1.2f);
            }
            trigger.isTrigger = true;
        }
        private void Start() { player = FindFirstObjectByType<Character>(); }

        public override void Interact(PlayerInteractor interactor)
        {
            if (interactor == null || !HasPlayerNearby || GameSession.BlocksGameplay || player == null) return;
            if (CampaignState.ExamActive) { GameplayFeedback.Show("bed.examActive"); return; }
            if (!Ready) { GameplayFeedback.Show("bed.tooSoon"); return; }
            // A nap tops Sleep up to the starting 10 and restores one Intellect below the
            // starting 5. It never exceeds those values, and the 2-minute cooldown plus the walk
            // from the classrooms keep it a recovery option rather than an unlimited refill.
            bool sleepGain = player.Sleep < RestedSleep, intellectGain = player.Intelligence < RestedIntellect;
            if (!sleepGain && !intellectGain) { GameplayFeedback.Show("bed.notTired"); return; }
            if (sleepGain) player.Sleep = RestedSleep;
            if (intellectGain) player.Intelligence = player.Intelligence + 1;
            readyAt = Time.time + Cooldown;
            GameplayFeedback.Show("bed.slept");
            RunSaveService.RequestSave();
        }
    }
}
