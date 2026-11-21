using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentSurvival.World
{
    public static class RunSaveService
    {
        [Serializable]
        private sealed class SaveFile
        {
            public string format;
            public int version = 1;
            public bool completed;
            public bool hasActive, hasCheckpoint;
            public RunSnapshot active;
            public RunSnapshot checkpoint;
        }
        private static SaveFile data;
        private static RunSnapshot pending;
        private static readonly HashSet<string> collected = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, GameObject> pickups = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        private static List<IRunSaveParticipant> participants = new List<IRunSaveParticipant>();
        private static bool ready, dirty, checkpointRequested;
        private static Vector3? checkpointPosition;
        private static float nextSave, nextPeriodic;
        public static bool IsRestoring { get; private set; }
        public static string LastErrorKey { get; private set; }
        public static event Action Changed;
        public static event Action<Vector3> PlayerRelocated;
        public static bool HasContinue { get { EnsureLoaded(); return data.active != null && !data.completed; } }
        public static bool HasCheckpoint { get { EnsureLoaded(); return data.checkpoint != null && !data.completed; } }
        private static string SavePath { get { return Path.Combine(Application.persistentDataPath, "student-survival-run.json"); } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            data = null; pending = null; ready = dirty = checkpointRequested = IsRestoring = false;
            checkpointPosition = null;
            LastErrorKey = null; Changed = null; collected.Clear(); pickups.Clear(); participants.Clear();
            PlayerRelocated = null;
            nextSave = nextPeriodic = 0;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            var driver = new GameObject("Run persistence").AddComponent<RunSaveDriver>();
            UnityEngine.Object.DontDestroyOnLoad(driver.gameObject);
        }
        public static bool PrepareNewRun()
        {
            EnsureLoaded();
            if (!Write(new SaveFile())) return false;
            pending = null;
            IsRestoring = true;
            return true;
        }
        public static bool PrepareContinue()
        {
            EnsureLoaded();
            if (!HasContinue) return false;
            pending = Clone(data.active); IsRestoring = true;
            return true;
        }
        public static bool PrepareRetryCheckpoint()
        {
            EnsureLoaded();
            if (!HasCheckpoint) return false;
            var next = CopyFile(); next.active = Clone(data.checkpoint);
            if (!Write(next)) return false;
            pending = Clone(next.active); IsRestoring = true;
            return true;
        }
        internal static void SceneLoading(Scene scene)
        {
            ready = false; dirty = false; checkpointRequested = false;
            checkpointPosition = null;
            participants.Clear(); collected.Clear();
            IsRestoring = scene.path == "Assets/Main.unity";
            if (!IsRestoring) { pending = null; pickups.Clear(); }
        }
        internal static void InitializeScene(Scene scene)
        {
            if (scene.path != "Assets/Main.unity") { IsRestoring = false; Changed?.Invoke(); return; }
            EnsureLoaded();
            participants = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(component => component.gameObject.scene == scene && component is IRunSaveParticipant)
                .Cast<IRunSaveParticipant>().OrderBy(component => component.RestoreOrder).ToList();
            try
            {
                if (pending != null)
                {
                    foreach (string id in pending.collectedPickupIds) collected.Add(id);
                    foreach (IRunSaveParticipant participant in participants) participant.Restore(pending);
                    GameSession.RestorePickups(pending.pickupsCollected);
                    foreach (var pair in pickups) if (collected.Contains(pair.Key) && pair.Value != null) pair.Value.SetActive(false);
                }
                ready = true;
                // A direct editor start intentionally starts a fresh run rather than overwriting it during Awake.
                if (pending == null) { collected.Clear(); data = new SaveFile(); }
                pending = null;
                IsRestoring = false;
                if (data.active == null) CreateCheckpoint();
                else RequestSave();
            }
            catch (Exception error)
            {
                Report("save.restoreFailed", error);
                // Keep gameplay blocked: a partially restored inventory must never overwrite the valid save.
                ready = false;
            }
            Changed?.Invoke();
        }
        public static void RegisterPickup(string id, GameObject pickup)
        {
            if (string.IsNullOrEmpty(id) || pickup == null) return;
            pickups[id] = pickup;
        }
        public static void CollectPickup(string id)
        {
            if (!string.IsNullOrEmpty(id)) collected.Add(id);
            RequestSave();
        }
        public static void RequestSave() { if (ready && !IsRestoring) dirty = true; }
        public static void CreateCheckpoint()
        {
            if (!ready || IsRestoring || GameSession.IsOver) return;
            checkpointPosition = null;
            checkpointRequested = true; dirty = true;
            SaveNow();
        }
        public static void CreateCheckpoint(Vector3 destination)
        {
            if (!ready || IsRestoring || GameSession.IsOver) return;
            checkpointPosition = destination;
            PlayerRelocated?.Invoke(destination);
            checkpointRequested = true; dirty = true;
            SaveNow();
        }
        internal static void Tick()
        {
            if (!ready || IsRestoring || GameSession.IsOver || GameSession.IsLoading) return;
            if (Time.unscaledTime >= nextSave && (dirty || Time.unscaledTime >= nextPeriodic)) SaveNow();
        }
        public static bool SaveNow()
        {
            EnsureLoaded();
            if (!ready || IsRestoring) return !IsRestoring;
            if (GameSession.Result == SessionResult.Lost) return OnRunLost();
            if (GameSession.Result == SessionResult.Won) return MarkCompleted();
            try
            {
                RunSnapshot snapshot = new RunSnapshot { runId = data.active != null ? data.active.runId : Guid.NewGuid().ToString("N"),
                    pickupsCollected = GameSession.PickupsCollected, collectedPickupIds = collected.OrderBy(id => id, StringComparer.Ordinal).ToArray() };
                foreach (IRunSaveParticipant participant in participants) participant.Capture(snapshot);
                if (checkpointRequested && checkpointPosition.HasValue) snapshot.playerPosition = checkpointPosition.Value;
                if (!Valid(snapshot)) throw new InvalidDataException("A run participant produced an invalid snapshot.");
                var next = CopyFile(); next.active = snapshot; next.completed = false;
                if (checkpointRequested || next.checkpoint == null) next.checkpoint = Clone(snapshot);
                if (!Write(next)) { nextSave = Time.unscaledTime + 5; return false; }
                dirty = checkpointRequested = false;
                checkpointPosition = null;
                nextSave = Time.unscaledTime + 0.5f;
                nextPeriodic = Time.unscaledTime + 10;
                return true;
            }
            catch (Exception error) { Report("save.writeFailed", error); nextSave = Time.unscaledTime + 5; return false; }
        }
        public static bool OnRunLost()
        {
            EnsureLoaded();
            var next = CopyFile(); next.active = next.checkpoint == null ? null : Clone(next.checkpoint);
            return Write(next);
        }
        public static bool MarkCompleted()
        {
            EnsureLoaded();
            return Write(new SaveFile { completed = true });
        }
        private static RunSnapshot Clone(RunSnapshot snapshot) { return snapshot == null ? null : JsonUtility.FromJson<RunSnapshot>(JsonUtility.ToJson(snapshot)); }
        private static SaveFile CopyFile()
        {
            var copy = JsonUtility.FromJson<SaveFile>(JsonUtility.ToJson(data));
            NormalizePresence(copy);
            return copy;
        }
        private static void NormalizePresence(SaveFile file)
        {
            if (!file.hasActive) file.active = null;
            if (!file.hasCheckpoint) file.checkpoint = null;
        }
        private static void EnsureLoaded()
        {
            if (data != null) return;
            // Error notifications can refresh menu subscribers that query HasContinue again.
            data = new SaveFile();
            data = Read(SavePath) ?? Read(SavePath + ".bak") ?? new SaveFile();
        }
        private static SaveFile Read(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                SaveFile loaded = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path));
                if (loaded == null || loaded.format != "StudentSurvival.Run" || loaded.version != 1 ||
                    (loaded.hasActive && (loaded.active == null || !Valid(loaded.active))) ||
                    (loaded.hasCheckpoint && (loaded.checkpoint == null || !Valid(loaded.checkpoint))))
                    throw new InvalidDataException("Unsupported or invalid run save.");
                NormalizePresence(loaded);
                return loaded;
            }
            catch (Exception error) { Report("save.readFailed", error); return null; }
        }
        private static bool Valid(RunSnapshot snapshot)
        {
            if (string.IsNullOrEmpty(snapshot.runId) || snapshot.scenePath != "Assets/Main.unity" || snapshot.sleep < 1 || snapshot.intelligence < 1 ||
                !Finite(snapshot.playerPosition.x) || !Finite(snapshot.playerPosition.y) || !Finite(snapshot.playerPosition.z) ||
                !Finite(snapshot.baseSpeed) || snapshot.baseSpeed < 0 || snapshot.inventory == null || snapshot.inventory.Length != 9 ||
                snapshot.selectedSlot < 0 || snapshot.selectedSlot > 8 || snapshot.collectedPickupIds == null || snapshot.pickupsCollected < 0 ||
                snapshot.examStage < 1 || snapshot.examStage > 2 || snapshot.teacherRequired < 1 || snapshot.teacherRemaining < 1 || snapshot.teacherRemaining > snapshot.teacherRequired) return false;
            foreach (RunInventorySlot slot in snapshot.inventory)
                if (slot != null && (slot.count < 0 || (slot.count > 0 && (slot.itemType < 0 || slot.itemType > 3)))) return false;
            return snapshot.collectedPickupIds.All(id => !string.IsNullOrEmpty(id));
        }
        private static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }
        private static bool Write(SaveFile next)
        {
            try
            {
                next.format = "StudentSurvival.Run";
                next.hasActive = next.active != null;
                next.hasCheckpoint = next.checkpoint != null;
                string path = SavePath, temporary = path + ".tmp";
                Directory.CreateDirectory(Application.persistentDataPath);
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(next, true));
                    stream.Write(bytes, 0, bytes.Length); stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
                else File.Move(temporary, path);
                data = next; LastErrorKey = null; Changed?.Invoke(); return true;
            }
            catch (Exception error) { Report("save.writeFailed", error); return false; }
        }
        private static void Report(string key, Exception error)
        {
            LastErrorKey = key;
            Debug.LogWarning("Run persistence: " + error.Message);
            Changed?.Invoke();
        }
    }
}
