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
        private bool changing, changePending, restoring;
        // Walking out (not jumping) cancels after this grace; door travel cancels immediately.
        private const float LeaveGrace = 0.75f;
        private float outsideSince = -1f;
        public Vector3 NextObjectivePosition
        {
            get
            {
                if (CampaignState.Completed) return player != null ? player.transform.position : Vector3.zero;
                if (player != null && Stage > 1)
                {
                    CampaignRooms second=CampaignRooms.Find(2);
                    CampaignRooms occupied=RoomContaining(player.transform.position);
                    if(occupied==null)return new Vector3(-.8f,CampaignRooms.FloorY,0);
                    if(occupied.RoomId==4&&occupied.BackDoor!=null)return occupied.BackDoor.position;
                    if(Stage==3&&occupied==second&&second.ForwardDoor!=null)return second.ForwardDoor.position;
                }
                return Current != null ? Current.transform.position : Vector3.zero;
            }
        }
        public static CampaignRooms RoomContaining(Vector3 point)
        {
            for(int stage=2;stage<=4;stage++)
            {
                CampaignRooms room=CampaignRooms.Find(stage);
                if(room==null)continue;
                StudentSurvival.Cameras.CameraBoundsRegion frame=room.GetComponent<StudentSurvival.Cameras.CameraBoundsRegion>();
                if(frame!=null&&frame.WorldBounds.Contains(new Vector3(point.x,point.y,frame.WorldBounds.center.z)))return room;
            }
            return null;
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
            Prepod original = null;
            foreach (Prepod candidate in FindObjectsByType<Prepod>(FindObjectsSortMode.None))
                if (candidate.GetComponentInParent<CampaignRooms>() == null) { original = candidate; break; }
            if (original == null) return;
            new GameObject("Semester campaign").AddComponent<ExamProgression>().Begin(original);
        }
        private void Awake() { Instance = this; }
        private void Begin(Prepod original)
        {
            player = FindFirstObjectByType<Character>();
            CampaignRooms second = CampaignRooms.Find(2), third = CampaignRooms.Find(3);
            if (second == null || third == null || second.TeacherSpawn == null || third.TeacherSpawn == null)
            { Debug.LogError("Main scene needs its authored campaign room prefabs and teacher markers.", this); return; }
            teachers[0] = original;
            teachers[1] = second.TeacherSpawn.GetComponent<Prepod>();
            teachers[2] = third.TeacherSpawn.GetComponent<Prepod>();
            if (teachers[1] == null || teachers[2] == null)
            { Debug.LogError("Authored room teacher markers need Prepod components.", this); return; }
            CampaignRooms.ConfigureMusic();
            for (int i = 0; i < teachers.Length; i++)
            {
                Prepod teacher = teachers[i];
                teacher.gameObject.name = i == 0 ? "First examiner" : i == 1 ? "Strict examiner" : "Final examiner";
                teacher.ConfigureCampaign(i + 1, i == 0 ? 5 : i == 1 ? 6 : 8);
                teacher.SetExamActive(false);
                Rigidbody2D body = teacher.GetComponent<Rigidbody2D>();
                if (body != null) body.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                teacher.Defeated += OnDefeated;
                var admission = new GameObject("Start exam"); admission.transform.SetParent(teacher.transform, false);
                admission.AddComponent<CampaignExamAdmission>().Configure(i + 1);
            }
            CampaignState.Changed += OnCampaignChanged;
            RunSaveService.PlayerRelocated += OnPlayerRelocated;
            OnCampaignChanged();
        }
        // One rule for all three exams: doors stay usable, but leaving the exam room cancels the
        // exam (teacher health and projectiles reset) and the player can start it again later.
        private void OnPlayerRelocated(Vector3 destination)
        {
            if (!restoring && CampaignState.ExamActive && !InExamRoom(destination)) CancelExam();
        }
        private void Update()
        {
            if (!CampaignState.ExamActive || GameSession.BlocksGameplay || player == null || restoring) { outsideSince = -1f; return; }
            if (InExamRoom(player.transform.position)) { outsideSince = -1f; return; }
            if (outsideSince < 0f) outsideSince = Time.time;
            else if (Time.time - outsideSince >= LeaveGrace) CancelExam();
        }
        private bool InExamRoom(Vector3 point)
        {
            Prepod teacher = Current;
            if (teacher == null) return true;
            CampaignRooms room = teacher.GetComponentInParent<CampaignRooms>();
            StudentSurvival.Cameras.CameraBoundsRegion frame = room != null ? room.GetComponent<StudentSurvival.Cameras.CameraBoundsRegion>() : null;
            if (frame != null)
            {
                Bounds bounds = frame.WorldBounds;
                // Height above the floor is ignored so a jump never counts as leaving.
                return point.x >= bounds.min.x - .5f && point.x <= bounds.max.x + .5f && point.y >= bounds.min.y - 1f;
            }
            // The original classroom has no authored frame: use the teacher's attack range.
            Vector3 offset = point - teacher.transform.position;
            return Mathf.Abs(offset.x) <= 14f && offset.y >= -4f;
        }
        private void CancelExam()
        {
            outsideSince = -1f;
            if (!CampaignState.ExamActive) return;
            CampaignState.SetExamActive(false);
            GameplayFeedback.Show("campaign.examLeft");
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
            if (restoring) return;
            // A change raised while refreshing (e.g. cheat notes finishing exam 2 and advancing
            // the stage) must still configure the next teacher once this pass completes.
            if (changing) { changePending = true; return; }
            if (CampaignState.Completed)
            {
                foreach (Prepod passed in teachers) if (passed != null)
                { passed.SetExamActive(false); passed.gameObject.SetActive(false); }
                Changed?.Invoke();
                return;
            }
            changing = true;
            // Teachers for the current and later stages must be present in an unfinished run.
            for (int i = Mathf.Clamp(Stage, 1, 3) - 1; i < teachers.Length; i++)
                if (teachers[i] != null && !teachers[i].gameObject.activeSelf) teachers[i].gameObject.SetActive(true);
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
            if (changePending) { changePending = false; OnCampaignChanged(); return; }
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
            RunSaveService.PlayerRelocated -= OnPlayerRelocated;
            if (Instance == this) Instance = null;
            foreach (Prepod teacher in teachers) if (teacher != null) teacher.Defeated -= OnDefeated;
        }
    }
}
