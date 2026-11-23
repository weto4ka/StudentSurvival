using StudentSurvival.Characters;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Inventory
{
    public sealed class CheatingEncounter : MonoBehaviour
    {
        private const float DetectionTime = 2.5f;
        // Like a real exam: the guard checks the room when exam 2 starts and watches anyone
        // using notes. Merely carrying notes outside that opening check is never punished.
        private const float InspectionDuration = 4f;
        private float inspectionUntil;
        private bool lastExamActive;
        public bool IsInspecting { get { return CampaignState.Stage == 2 && CampaignState.ExamActive && CampaignState.HasCheatNotes && Time.time < inspectionUntil; } }
        public float InspectionSeconds { get { return IsInspecting ? inspectionUntil - Time.time : 0; } }
        private CampaignRooms room, toilet;
        private float FloorY => room.GuardStart.position.y;
        private Vector3 Preparation => room.StudyPoint.position;
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
            room = CampaignRooms.Find(2); toilet = CampaignRooms.Find(4);
            if (room == null || toilet == null) { Debug.LogError("Missing authored practical classroom.", this); enabled = false; return; }
            player = FindFirstObjectByType<Character>();
            props = new GameObject("Stage two preparation props"); props.transform.SetParent(transform, false);
            notes = Target("Hidden cheat notes", CheatingInteractionKind.Notes, toilet.NotesPoint.position);
            study = Target("Free revision desk", CheatingInteractionKind.Study, room.StudyPoint.position);
            use = Target("Use notes beside examiner", CheatingInteractionKind.UseNotes, room.TeacherSpawn.position);
            noteArt = CheatingPixelArt.Create(props.transform, "Folded notes behind toilet", toilet.NotesPoint.position + new Vector3(-.15f, .65f, .05f), CheatingPixelArt.Kind.Notes).transform;
            noteArt.localScale = Vector3.one * .4f;
            CheatingPixelArt guardVisual = CheatingPixelArt.Create(props.transform, "Campus guard", room.GuardStart.position, CheatingPixelArt.Kind.Guard);
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
                guard.position = room.GuardStart.position;
            }
            bool stageTwo = CampaignState.Stage == 2;
            bool examTwo = stageTwo && CampaignState.ExamActive;
            if (examTwo && !lastExamActive) { inspectionUntil = Time.time + InspectionDuration; suspicion = 0; }
            if (!examTwo) inspectionUntil = 0;
            lastExamActive = examTwo;
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
                if (guard.position.x <= room.PatrolLeft.position.x || guard.position.x >= room.PatrolRight.position.x)
                {
                    guard.position = new Vector3(Mathf.Clamp(guard.position.x, room.PatrolLeft.position.x, room.PatrolRight.position.x), FloorY, 0);
                    direction = -direction; turnPause = .65f;
                }
            }
            guardArt.flipX = direction < 0;
            bool inspecting = IsInspecting;
            bool seen = inspecting && CanSeeNotes();
            float before = suspicion;
            suspicion = seen ? Mathf.Min(DetectionTime, suspicion + delta) : inspecting ? Mathf.Max(0, suspicion - delta * 1.6f) : 0;
            if (seen && before == 0) GameplayFeedback.Show("cheat.warning");
            UpdateWarning();
            if (suspicion >= DetectionTime) Confiscate();
        }
        private bool CanSeeNotes()
        {
            if (!CampaignState.HasCheatNotes) return false;
            Vector3 position = player.transform.position;
            if (Mathf.Abs(position.y - FloorY) > 3f || Mathf.Abs(position.x - toilet.HidePoint.position.x) <= .85f) return false;
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
            // The attention mark also shows while the guard inspects at the start of exam 2.
            attention.gameObject.SetActive(fraction > .01f || IsInspecting);
        }
        // Using notes in the guard's view is caught immediately; out of view (or crouched at a
        // distance) the notes apply as before.
        public bool TryUseNotes()
        {
            if (GameSession.BlocksGameplay || player == null || CampaignState.Stage != 2 || !CampaignState.ExamActive || !CampaignState.HasCheatNotes) return false;
            if (CanSeeNotes()) { Confiscate("cheat.caughtUsing"); return false; }
            return CampaignState.TryUseCheatNotes();
        }
        private void Confiscate(string messageKey = "cheat.caught")
        {
            suspicion = 0;
            CampaignState.ConfiscateCheatNotes();
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            if (body != null) { body.linearVelocity = Vector2.zero; body.position = Preparation; }
            player.transform.position = Preparation;
            if (player.Sleep > 0) RunSaveService.CreateCheckpoint(Preparation); else RunSaveService.RequestSave();
            GameplayFeedback.Show(messageKey);
            UpdateWarning();
        }
        private void OnDestroy()
        {
            CampaignState.Changed -= Refresh;
            if (Instance == this) Instance = null;
        }
    }
}
