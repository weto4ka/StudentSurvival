using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.World
{
    // Shared preparation state lives below Characters/Inventory so all encounters use one truth.
    public sealed class CampaignState : MonoBehaviour, IRunSaveParticipant
    {
        private static CampaignState instance;
        public static event Action Changed;
        public static int Stage { get; private set; } = 1;
        public static bool ExamActive { get; private set; }
        public static bool Completed { get; private set; }
        public static bool Stage2Studied { get; private set; }
        public static bool HasCheatNotes { get; private set; }
        public static bool CheatNotesUsed { get; private set; }
        public static int GuardWarnings { get; private set; }
        public static int PreparationRemaining { get; private set; } = 2;
        public static int PreparedPoints { get; private set; }
        public static bool HasRelationship { get; private set; }
        public static bool RelationshipHelpClaimed { get; private set; }
        public static bool IsPreparationStage => Stage == 3 && !ExamActive && !Completed;
        public static bool CanSpendPreparation => IsPreparationStage && PreparationRemaining > 0;
        public static bool CanFlirt => CanSpendPreparation && !HasRelationship;
        public static bool CanClaimRelationshipHelp => IsPreparationStage && HasRelationship && PreparedPoints >= 1 && !RelationshipHelpClaimed;
        public int RestoreOrder => 25;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { instance = null; Changed = null; ResetValues(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= Loaded; SceneManager.sceneLoaded += Loaded;
            Attach(SceneManager.GetActiveScene());
        }
        private static void Loaded(Scene scene, LoadSceneMode mode) { if (mode == LoadSceneMode.Single) Attach(scene); }
        private static void Attach(Scene scene)
        {
            if (scene.path != "Assets/Main.unity" || instance != null) return;
            new GameObject("Campaign preparation state").AddComponent<CampaignState>();
        }
        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this; ResetValues(); Changed?.Invoke();
        }
        private static void ResetValues()
        {
            Stage = 1; ExamActive = Completed = false; Stage2Studied = HasCheatNotes = CheatNotesUsed = false;
            GuardWarnings = 0; PreparationRemaining = 2; PreparedPoints = 0;
            HasRelationship = RelationshipHelpClaimed = false;
        }
        private static void Notify() { Changed?.Invoke(); RunSaveService.RequestSave(); }
        public static void SetStage(int stage)
        {
            if (Completed) return;
            int next = Mathf.Clamp(stage, 1, 3);
            if (Stage == next && !ExamActive) return;
            Stage = next; ExamActive = false; Notify();
        }
        public static void SetExamActive(bool active)
        { if (Completed && active || ExamActive == active) return; ExamActive = active; Notify(); }
        public static void CompleteSemester()
        {
            if (Completed) return;
            Stage = 3; ExamActive = false; Completed = true; Notify();
        }
        public static bool CanStartExam(int stage)
        {
            if (Completed || stage != Stage || ExamActive) return false;
            return stage == 1 || (stage == 2 && (Stage2Studied || HasCheatNotes || CheatNotesUsed)) ||
                (stage == 3 && (PreparedPoints > 0 || HasRelationship));
        }
        public static bool TryStudyStage2()
        {
            if (GameSession.BlocksGameplay || Stage != 2 || ExamActive || Stage2Studied) return false;
            Stage2Studied = true; Notify(); return true;
        }
        public static bool TryCollectCheatNotes()
        {
            if (GameSession.BlocksGameplay || Stage != 2 || HasCheatNotes || CheatNotesUsed) return false;
            HasCheatNotes = true; Notify(); return true;
        }
        public static bool TryUseCheatNotes()
        {
            if (GameSession.BlocksGameplay || Stage != 2 || !ExamActive || !HasCheatNotes) return false;
            HasCheatNotes = false; CheatNotesUsed = true; Notify(); return true;
        }
        public static void ConfiscateCheatNotes()
        {
            if (GameSession.BlocksGameplay || Stage != 2 || !HasCheatNotes) return;
            HasCheatNotes = false; ExamActive = false;
            GuardWarnings = Mathf.Min(999, GuardWarnings + 1); Notify();
        }
        public static bool TryPrepare()
        {
            if (GameSession.BlocksGameplay || !CanSpendPreparation) return false;
            PreparationRemaining--; PreparedPoints = Mathf.Min(2, PreparedPoints + 1); Notify(); return true;
        }
        public static bool TryFlirt()
        {
            if (GameSession.BlocksGameplay || !CanFlirt) return false;
            PreparationRemaining--; HasRelationship = true; Notify(); return true;
        }
        public static bool TryClaimRelationshipHelp()
        {
            if (GameSession.BlocksGameplay || !CanClaimRelationshipHelp) return false;
            RelationshipHelpClaimed = true; Notify(); return true;
        }
        public void Capture(RunSnapshot snapshot)
        {
            snapshot.campaignVersion = 1;
            snapshot.semesterCompleted = Completed;
            snapshot.stage2Studied = Stage2Studied; snapshot.hasCheatNotes = HasCheatNotes;
            snapshot.cheatNotesUsed = CheatNotesUsed; snapshot.guardWarnings = GuardWarnings;
            snapshot.preparationRemaining = PreparationRemaining; snapshot.preparedPoints = PreparedPoints;
            snapshot.flirted = HasRelationship; snapshot.relationshipHelpClaimed = RelationshipHelpClaimed;
        }
        public void Restore(RunSnapshot snapshot)
        {
            Completed = snapshot.semesterCompleted;
            Stage = Completed ? 3 : Mathf.Clamp(snapshot.examStage, 1, 3); ExamActive = !Completed && snapshot.examActive;
            bool legacy = snapshot.campaignVersion == 0;
            Stage2Studied = !legacy && snapshot.stage2Studied;
            HasCheatNotes = !legacy && snapshot.hasCheatNotes; CheatNotesUsed = !legacy && snapshot.cheatNotesUsed;
            GuardWarnings = legacy ? 0 : Mathf.Clamp(snapshot.guardWarnings, 0, 999);
            HasRelationship = snapshot.flirted;
            PreparationRemaining = legacy ? (HasRelationship ? 1 : 2) : Mathf.Clamp(snapshot.preparationRemaining, 0, 2);
            PreparedPoints = legacy ? 0 : Mathf.Clamp(snapshot.preparedPoints, 0, 2);
            RelationshipHelpClaimed = !legacy && snapshot.relationshipHelpClaimed;
            Changed?.Invoke();
        }
        private void OnDestroy() { if (instance == this) instance = null; }
    }
}
