using System;
using UnityEngine;

namespace StudentSurvival.World
{
    // DTOs deliberately stay in World so persistence never depends on Characters or Inventory.
    [Serializable]
    public sealed class RunSnapshot
    {
        public string runId;
        public string scenePath = "Assets/Main.unity";
        public Vector3 playerPosition;
        public int sleep = 10, intelligence = 5;
        public float baseSpeed = 3;
        public bool facingLeft;
        public bool crouching;
        public float stimulationRemaining, crashRemaining, exhaustionRemaining;
        public int borrowedSleep;
        public bool flirted;
        public int pickupsCollected;
        public RunInventorySlot[] inventory = new RunInventorySlot[9];
        public int selectedSlot;
        public string[] collectedPickupIds = Array.Empty<string>();
        public int examStage = 1, teacherRemaining = 5, teacherRequired = 5;
        public bool betweenExams;
    }

    [Serializable]
    public sealed class RunInventorySlot
    {
        public int itemType;
        public int count;
        public string sourcePickupId;
    }

    public interface IRunSaveParticipant
    {
        int RestoreOrder { get; }
        void Capture(RunSnapshot snapshot);
        void Restore(RunSnapshot snapshot);
    }
}
