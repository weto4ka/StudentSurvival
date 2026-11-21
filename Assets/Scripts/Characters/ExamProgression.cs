using System;
using System.Collections;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.Characters
{
    // A second exam reuses the existing classroom and teacher art, rather than
    // duplicating combat code or placing an unreviewed room in the level.
    public sealed class ExamProgression : MonoBehaviour
    {
        public static ExamProgression Instance { get; private set; }
        public int Stage { get; private set; } = 1;
        public Prepod Current { get; private set; }
        public event Action Changed;
        private Prepod second;
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
            Current = teacher;
            second = Instantiate(teacher.gameObject, teacher.transform.position, teacher.transform.rotation).GetComponent<Prepod>();
            second.gameObject.name = "Final Examiner";
            second.gameObject.SetActive(false);
            second.Configure(7, Mathf.Max(0.5f, teacher.ShotInterval * 0.8f), new Color(1f, 0.7f, 0.3f));
            Current.Defeated += OnDefeated;
        }
        private void OnDefeated(Prepod teacher)
        {
            teacher.Defeated -= OnDefeated;
            if (Stage == 2) { GameSession.Win(); return; }
            Stage = 2;
            Current = null;
            Character player = FindFirstObjectByType<Character>();
            if (player != null) { player.Sleep += 3; player.Intelligence++; }
            GameplayFeedback.Show("exam.firstPassed");
            Changed?.Invoke();
            StartCoroutine(NextExam());
        }
        private IEnumerator NextExam()
        {
            yield return new WaitForSeconds(2f);
            if (GameSession.IsOver || second == null) yield break;
            Current = second;
            Current.Defeated += OnDefeated;
            Current.gameObject.SetActive(true);
            Changed?.Invoke();
        }
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (Current != null) Current.Defeated -= OnDefeated;
        }
    }
}
