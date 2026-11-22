using System;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Characters
{
    public sealed class ExamProgression : MonoBehaviour, IRunSaveParticipant
    {
        public static ExamProgression Instance { get; private set; }
        public int Stage { get { return CampaignState.Stage; } }
        public Prepod Current { get { return CampaignState.Completed ? null : teachers[Mathf.Clamp(Stage, 1, 3) - 1]; } }
        public bool BetweenExams { get { return !CampaignState.Completed && Stage > 1 && !CampaignState.ExamActive; } }
        public int RestoreOrder { get { return 30; } }
        public string TeacherNameKey { get { return "campaign.teacher" + Stage; } }
        public event Action Changed;
        private readonly Prepod[] teachers = new Prepod[3];
        private Character player;
        private bool changing, restoring;
        public Vector3 NextObjectivePosition
        {
            get
            {
                if (CampaignState.Completed) return player != null ? player.transform.position : Vector3.zero;
                if (player != null && Stage == 2 && player.transform.position.x < 17f) return new Vector3(-0.8f, CampaignRooms.FloorY, 0);
                if (player != null && Stage == 3 && player.transform.position.x < 37f)
                    return player.transform.position.x < 17f ? new Vector3(-0.8f, CampaignRooms.FloorY, 0) : new Vector3(30.3f, CampaignRooms.FloorY, 0);
                return Current != null ? Current.transform.position : Vector3.zero;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic() { Instance = null; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || Instance != null) return;
            Prepod original = FindFirstObjectByType<Prepod>();
            if (original == null) return;
            new GameObject("Semester campaign").AddComponent<ExamProgression>().Begin(original);
        }
        private void Awake() { Instance = this; }
        private void Begin(Prepod original)
        {
            player = FindFirstObjectByType<Character>();
            teachers[0] = original;
            teachers[1] = Instantiate(original.gameObject, new Vector3(28.5f, 3.14f, 0), Quaternion.identity).GetComponent<Prepod>();
            teachers[2] = Instantiate(original.gameObject, new Vector3(48.5f, 3.14f, 0), Quaternion.identity).GetComponent<Prepod>();
            gameObject.AddComponent<CampaignRooms>().Build();
            for (int i = 0; i < teachers.Length; i++)
            {
                Prepod teacher = teachers[i];
                teacher.gameObject.name = i == 0 ? "First examiner" : i == 1 ? "Strict examiner" : "Final examiner";
                teacher.ConfigureCampaign(i + 1, i == 0 ? 5 : i == 1 ? 6 : 8);
                teacher.SetExamActive(false);
                Rigidbody2D body = teacher.GetComponent<Rigidbody2D>();
                if (body != null) body.freezeRotation = true;
                teacher.Defeated += OnDefeated;
                var admission = new GameObject("Start exam"); admission.transform.SetParent(teacher.transform, false);
                admission.AddComponent<CampaignExamAdmission>().Configure(i + 1);
            }
            CampaignState.Changed += OnCampaignChanged;
            OnCampaignChanged();
        }
        private int RequiredFor(int stage)
        {
            if (stage == 1) return 5;
            if (stage == 2) return CampaignState.CheatNotesUsed ? 4 : 6;
            return CampaignState.PreparedPoints >= 2 ? 5 : CampaignState.PreparedPoints == 1 ? 6 : 8;
        }
        public void StartExam(int stage)
        {
            if (GameSession.BlocksGameplay || CampaignState.ExamActive || stage != Stage || Current == null) return;
            if (!CampaignState.CanStartExam(stage)) { GameplayFeedback.Show("campaign.notReady"); return; }
            Current.ConfigureCampaign(stage, RequiredFor(stage));
            Current.SetExamActive(true);
            CampaignState.SetExamActive(true);
            GameplayFeedback.Show("campaign.examStartedNamed", TeacherNameKey);
            RunSaveService.RequestSave();
            Changed?.Invoke();
        }
        private void OnCampaignChanged()
        {
            if (changing || restoring) return;
            if (CampaignState.Completed)
            {
                foreach (Prepod passed in teachers) if (passed != null)
                { passed.SetExamActive(false); passed.gameObject.SetActive(false); }
                Changed?.Invoke();
                return;
            }
            changing = true;
            Prepod teacher = Current;
            if (teacher != null)
            {
                if (!CampaignState.ExamActive)
                {
                    bool wasActive = teacher.ExamActive;
                    if (wasActive) teacher.SetExamActive(false);
                    int required = RequiredFor(Stage);
                    // Preview the preparation benefit before admission as well as resetting a retake.
                    if (teacher.RemainingLabs > 0 && (wasActive || teacher.RequiredLabs != required))
                    { teacher.SetRequiredLabs(required, false); teacher.RestoreHealth(required); }
                }
                else if (CampaignState.ExamActive && Stage == 2 && CampaignState.CheatNotesUsed && teacher.RequiredLabs != 4)
                    teacher.SetRequiredLabs(4);
            }
            changing = false;
            Changed?.Invoke();
        }
        private void OnDefeated(Prepod teacher)
        {
            if (teacher != Current) return;
            int passed = Stage;
            CampaignState.SetExamActive(false);
            if (passed == 3) { CampaignState.CompleteSemester(); GameSession.Win(); return; }
            if (player != null) { player.Sleep += passed == 1 ? 5 : 6; player.Intelligence++; }
            CampaignState.SetStage(passed + 1);
            GameplayFeedback.Show("campaign.stagePassed");
            Changed?.Invoke();
            RunSaveService.CreateCheckpoint();
        }
        public void Capture(RunSnapshot snapshot)
        {
            snapshot.examStage = Stage; snapshot.examActive = CampaignState.ExamActive;
            snapshot.betweenExams = BetweenExams;
            snapshot.teacherRequired = Current != null ? Current.RequiredLabs : RequiredFor(Stage);
            snapshot.teacherRemaining = CampaignState.Completed ? 0 : Current != null ? Mathf.Max(1, Current.RemainingLabs) : snapshot.teacherRequired;
        }
        public void Restore(RunSnapshot snapshot)
        {
            restoring = true;
            if (snapshot.semesterCompleted)
            {
                CampaignState.CompleteSemester();
                foreach (Prepod passed in teachers) if (passed != null)
                { passed.SetExamActive(false); passed.gameObject.SetActive(false); }
                restoring = false;
                Changed?.Invoke();
                return;
            }
            CampaignState.SetStage(Mathf.Clamp(snapshot.examStage, 1, 3));
            for (int i = 0; i < teachers.Length; i++) if (teachers[i] != null)
            {
                teachers[i].gameObject.SetActive(i + 1 >= Stage);
                teachers[i].SetExamActive(false);
            }
            if (Current != null)
            {
                Current.ConfigureCampaign(Stage, RequiredFor(Stage));
                Current.RestoreHealth(snapshot.teacherRemaining);
                Current.SetExamActive(snapshot.campaignVersion > 0 && snapshot.examActive);
            }
            CampaignState.SetExamActive(snapshot.campaignVersion > 0 && snapshot.examActive);
            restoring = false;
            Changed?.Invoke();
        }
        public void RestoreProgress(int stage, int health)
        { Restore(new RunSnapshot { examStage = stage, teacherRemaining = health, examActive = false }); }
        private void OnDestroy()
        {
            CampaignState.Changed -= OnCampaignChanged;
            if (Instance == this) Instance = null;
            foreach (Prepod teacher in teachers) if (teacher != null) teacher.Defeated -= OnDefeated;
        }
    }
}
