using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    public sealed class CheatingEncounter : MonoBehaviour
    {
        private const float FloorY = 3.04f, DetectionTime = 2.5f;
        private static readonly Vector3 Preparation = new Vector3(25f, FloorY, 0);
        public static CheatingEncounter Instance { get; private set; }
        public bool IsSuspicious { get { return CampaignState.Stage == 2 && CampaignState.HasCheatNotes && suspicion > .01f; } }
        public float WarningSeconds { get { return IsSuspicious ? Mathf.Max(0, DetectionTime - suspicion) : 0; } }
        public Transform GuardTransform { get { return guard; } }
        private Character player;
        private Transform guard, noteArt, warningFill, attention;
        private SpriteRenderer guardArt, warningArt;
        private CheatingInteraction notes, study, use;
        private GameObject props;
        private float suspicion, turnPause;
        private int direction = -1, lastStage = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { Instance = null; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded -= Loaded;
            SceneManager.sceneLoaded += Loaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void Loaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path == "Assets/Main.unity" && Instance == null) new GameObject("Cheat notes and campus guard").AddComponent<CheatingEncounter>();
        }
        private void Awake()
        {
            Instance = this;
            player = FindFirstObjectByType<Character>();
            props = new GameObject("Stage two preparation props"); props.transform.SetParent(transform, false);
            notes = Target("Hidden cheat notes", CheatingInteractionKind.Notes, new Vector3(22, FloorY, 0));
            study = Target("Free revision desk", CheatingInteractionKind.Study, new Vector3(25.5f, FloorY, 0));
            use = Target("Use notes beside examiner", CheatingInteractionKind.UseNotes, new Vector3(28.5f, FloorY, 0));
            CheatingPixelArt toilet = CheatingPixelArt.Create(props.transform, "Toilet alcove", new Vector3(22.3f, FloorY, 0), CheatingPixelArt.Kind.Toilet);
            toilet.transform.localScale = Vector3.one * .8f;
            noteArt = CheatingPixelArt.Create(props.transform, "Folded notes behind toilet", new Vector3(21.85f, FloorY + .65f, .05f), CheatingPixelArt.Kind.Notes).transform;
            noteArt.localScale = Vector3.one * .4f;
            CheatingPixelArt.Create(props.transform, "Revision desk", new Vector3(25.5f, FloorY, 0), CheatingPixelArt.Kind.Desk);
            CheatingPixelArt guardVisual = CheatingPixelArt.Create(props.transform, "Campus guard", new Vector3(26.5f, FloorY, 0), CheatingPixelArt.Kind.Guard);
            guard = guardVisual.transform; guardArt = guardVisual.Renderer;
            var indicator = new GameObject("Suspicion meter"); indicator.transform.SetParent(guard, false); indicator.transform.localPosition = Vector3.up * 2.95f;
            CheatingPixelArt.Create(indicator.transform, "Meter border", Vector3.zero, CheatingPixelArt.Kind.Pixel).transform.localScale = new Vector3(1.1f, .16f, 1);
            CheatingPixelArt fill = CheatingPixelArt.Create(indicator.transform, "Suspicion", new Vector3(-.51f, 0, -.01f), CheatingPixelArt.Kind.Pixel);
            warningFill = fill.transform; warningArt = fill.Renderer;
            warningFill.localScale = new Vector3(0, .1f, 1);
            attention = CheatingPixelArt.Create(guard, "Guard attention", new Vector3(0, 3.15f, 0), CheatingPixelArt.Kind.Attention).transform;
            CampaignState.Changed += Refresh;
            Refresh();
        }
        private CheatingInteraction Target(string label, CheatingInteractionKind kind, Vector3 position)
        {
            var target = new GameObject(label); target.transform.SetParent(props.transform, false); target.transform.position = position;
            CheatingInteraction interaction = target.AddComponent<CheatingInteraction>(); interaction.Configure(kind);
            return interaction;
        }
        private void Refresh()
        {
            if (CampaignState.Stage != lastStage)
            {
                lastStage = CampaignState.Stage; suspicion = turnPause = 0; direction = -1;
                guard.position = new Vector3(26.5f, FloorY, 0);
            }
            bool stageTwo = CampaignState.Stage == 2;
            props.SetActive(stageTwo);
            notes.SetAvailable(stageTwo && !CampaignState.HasCheatNotes && !CampaignState.CheatNotesUsed);
            noteArt.gameObject.SetActive(stageTwo && !CampaignState.HasCheatNotes && !CampaignState.CheatNotesUsed);
            study.SetAvailable(stageTwo && !CampaignState.ExamActive && !CampaignState.Stage2Studied);
            use.SetAvailable(stageTwo && CampaignState.ExamActive && CampaignState.HasCheatNotes);
            if (!CampaignState.HasCheatNotes) suspicion = 0;
            UpdateWarning();
        }
        private void Update()
        {
            if (GameSession.BlocksGameplay || CampaignState.Stage != 2 || player == null) return;
            Prepod teacher = ExamProgression.Instance != null ? ExamProgression.Instance.Current : null;
            bool mayUse = CampaignState.ExamActive && CampaignState.HasCheatNotes && teacher != null && teacher.isActiveAndEnabled;
            use.SetAvailable(mayUse);
            if (mayUse) use.transform.position = teacher.transform.position - Vector3.right * 1.15f;
            float delta = Time.deltaTime;
            if (turnPause > 0) turnPause = Mathf.Max(0, turnPause - delta);
            else
            {
                guard.position += Vector3.right * (direction * 1.25f * delta);
                if (guard.position.x <= 23.4f || guard.position.x >= 27f)
                {
                    guard.position = new Vector3(Mathf.Clamp(guard.position.x, 23.4f, 27), FloorY, 0);
                    direction = -direction; turnPause = .65f;
                }
            }
            guardArt.flipX = direction < 0;
            bool seen = CanSeeNotes();
            float before = suspicion;
            suspicion = seen ? Mathf.Min(DetectionTime, suspicion + delta) : Mathf.Max(0, suspicion - delta * 1.6f);
            if (seen && before == 0) GameplayFeedback.Show("cheat.warning");
            UpdateWarning();
            if (suspicion >= DetectionTime) Confiscate();
        }
        private bool CanSeeNotes()
        {
            if (!CampaignState.HasCheatNotes) return false;
            Vector3 position = player.transform.position;
            if (Mathf.Abs(position.y - FloorY) > 3f || Mathf.Abs(position.x - 22f) <= .85f) return false;
            float horizontal = position.x - guard.position.x;
            // Crouching hides the notes at a distance, but walking into the guard is still risky.
            float range = player.IsCrouching ? 1.05f : 4f;
            return Mathf.Abs(horizontal) <= range && (Mathf.Abs(horizontal) <= .8f || horizontal * direction >= 0);
        }
        private void UpdateWarning()
        {
            float fraction = Mathf.Clamp01(suspicion / DetectionTime);
            warningFill.localScale = new Vector3(fraction * 1.02f, .1f, 1);
            warningFill.localPosition = new Vector3(-.51f + fraction * .51f, 0, -.01f);
            warningArt.color = Color.Lerp(new Color(1, .8f, .2f), new Color(1, .22f, .1f), fraction);
            attention.gameObject.SetActive(fraction > .01f);
        }
        private void Confiscate()
        {
            suspicion = 0;
            CampaignState.ConfiscateCheatNotes();
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            if (body != null) { body.linearVelocity = Vector2.zero; body.position = Preparation; }
            player.transform.position = Preparation;
            if (player.Sleep > 0) RunSaveService.CreateCheckpoint(Preparation); else RunSaveService.RequestSave();
            GameplayFeedback.Show("cheat.caught");
            UpdateWarning();
        }
        private void OnDestroy()
        {
            CampaignState.Changed -= Refresh;
            if (Instance == this) Instance = null;
        }
    }
}
