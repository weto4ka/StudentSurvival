using System;
using System.Collections;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Characters
{
    // A second exam reuses the existing classroom and teacher art, rather than
    // duplicating combat code or placing an unreviewed room in the level.
    public sealed class ExamProgression : MonoBehaviour, IRunSaveParticipant
    {
        public static ExamProgression Instance { get; private set; }
        public int Stage { get; private set; } = 1;
        public Prepod Current { get; private set; }
        public bool BetweenExams { get; private set; }
        public int RestoreOrder { get { return 30; } }
        public event Action Changed;
        private Prepod first, second;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic() { Instance = null; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || Instance != null) return;
            Prepod teacher = FindFirstObjectByType<Prepod>();
            if (teacher == null) return;
            ExamProgression progression = new GameObject("ExamProgression").AddComponent<ExamProgression>();
            progression.Begin(teacher);
        }
        private void Awake()
        {
            Instance = this;
        }
        private void Begin(Prepod teacher)
        {
            first = Current = teacher;
            second = Instantiate(teacher.gameObject, teacher.transform.position, teacher.transform.rotation).GetComponent<Prepod>();
            second.gameObject.name = "Final Examiner";
            second.gameObject.SetActive(false);
            second.Configure(7, Mathf.Max(0.5f, teacher.ShotInterval * 0.8f), new Color(1f, 0.7f, 0.3f), true);
            Current.Defeated += OnDefeated;
        }
        private void OnDefeated(Prepod teacher)
        {
            teacher.Defeated -= OnDefeated;
            if (Stage == 2) { GameSession.Win(); return; }
            Stage = 2;
            Current = null;
            BetweenExams = true;
            Character player = FindFirstObjectByType<Character>();
            if (player != null) { player.Sleep += 3; player.Intelligence++; }
            GameplayFeedback.Show("exam.firstPassed");
            Changed?.Invoke();
            RunSaveService.CreateCheckpoint();
            StartCoroutine(NextExam());
        }
        private IEnumerator NextExam()
        {
            yield return new WaitForSeconds(2f);
            if (GameSession.IsOver || second == null) yield break;
            BetweenExams = false;
            Current = second;
            Current.Defeated += OnDefeated;
            Current.gameObject.SetActive(true);
            Changed?.Invoke();
            RunSaveService.RequestSave();
        }
        public void Capture(RunSnapshot snapshot)
        {
            snapshot.examStage = Stage;
            snapshot.betweenExams = BetweenExams;
            Prepod teacher = Current != null ? Current : second;
            snapshot.teacherRequired = teacher != null ? teacher.RequiredLabs : Stage == 2 ? 7 : 5;
            snapshot.teacherRemaining = teacher != null ? Mathf.Max(1, teacher.RemainingLabs) : snapshot.teacherRequired;
        }
        public void Restore(RunSnapshot snapshot) { RestoreProgress(snapshot.examStage, snapshot.teacherRemaining); }
        public void RestoreProgress(int stage, int health)
        {
            StopAllCoroutines();
            if (Current != null) Current.Defeated -= OnDefeated;
            Stage = stage == 2 ? 2 : 1;
            BetweenExams = false;
            // Restoring the transition enters exam two immediately; rewards were already saved.
            if (first != null) first.gameObject.SetActive(Stage == 1);
            if (second != null) second.gameObject.SetActive(Stage == 2);
            Current = Stage == 2 ? second : first;
            if (Current != null)
            {
                Current.RestoreHealth(health);
                Current.Defeated += OnDefeated;
            }
            Changed?.Invoke();
        }
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (Current != null) Current.Defeated -= OnDefeated;
        }
    }
}
