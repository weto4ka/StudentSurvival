using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Characters
{
    public partial class Character
    {
        private const float BoostDuration = 20, CrashDuration = 10, RescueDuration = 8;
        private float stimulationRemaining, crashRemaining, exhaustionRemaining = RescueDuration;
        private int borrowedSleep;
        private float horizontalSmoothing;
        public string EnergyStatusKey => stimulationRemaining > 0 ? "energy.stimulated" : sleep == 0 ? "energy.exhausted" : crashRemaining > 0 ? "energy.crash" : "";
        public float EnergyStatusSeconds => stimulationRemaining > 0 ? stimulationRemaining : sleep == 0 ? exhaustionRemaining : crashRemaining;
        private float MovementMultiplier => stimulationRemaining > 0 ? 1.7f : crashRemaining > 0 ? .55f : 1;
        private float MovementResponse => stimulationRemaining > 0 ? .015f : crashRemaining > 0 ? .22f : .035f;
        public bool TryDrinkEnergy()
        {
            if (GameSession.BlocksGameplay) return false;
            if (stimulationRemaining > 0) { GameplayFeedback.Show("energy.already"); return false; }
            sleep += 3; borrowedSleep = 3;
            stimulationRemaining = BoostDuration; crashRemaining = CrashDuration; exhaustionRemaining = RescueDuration;
            EnergyChanged();
            return true;
        }
        private void SetSleep(int value)
        {
            int previous = sleep;
            sleep = Mathf.Max(0, value);
            borrowedSleep = Mathf.Max(0, borrowedSleep - Mathf.Max(0, previous - sleep));
            if (sleep > 0 || previous > 0) exhaustionRemaining = RescueDuration;
            EnergyChanged();
        }
        private void TickEnergy()
        {
            float delta = Time.deltaTime;
            if (stimulationRemaining > 0)
            {
                stimulationRemaining = Mathf.Max(0, stimulationRemaining - delta);
                if (stimulationRemaining == 0)
                {
                    sleep = Mathf.Max(0, sleep - borrowedSleep); borrowedSleep = 0;
                    crashRemaining = CrashDuration; exhaustionRemaining = RescueDuration;
                    GameplayFeedback.Show("energy.crashStarted"); EnergyChanged();
                }
            }
            else
            {
                if (crashRemaining > 0)
                {
                    crashRemaining = Mathf.Max(0, crashRemaining - delta);
                    if (crashRemaining == 0) EnergyChanged();
                }
                if (sleep == 0)
                {
                    exhaustionRemaining = Mathf.Max(0, exhaustionRemaining - delta);
                    if (exhaustionRemaining == 0) { GameSession.Lose("result.noSleepTimed"); return; }
                }
            }
        }
        private void EnergyChanged() { StatsChanged?.Invoke(); RunSaveService.RequestSave(); }
        private void CaptureEnergy(RunSnapshot snapshot)
        {
            snapshot.stimulationRemaining = Mathf.Clamp(stimulationRemaining, 0, BoostDuration);
            snapshot.crashRemaining = Mathf.Clamp(crashRemaining, 0, CrashDuration);
            snapshot.exhaustionRemaining = Mathf.Clamp(exhaustionRemaining, 0, RescueDuration);
            snapshot.borrowedSleep = Mathf.Clamp(borrowedSleep, 0, sleep);
        }
        private void RestoreEnergy(RunSnapshot snapshot)
        {
            stimulationRemaining = Mathf.Clamp(snapshot.stimulationRemaining, 0, BoostDuration);
            crashRemaining = Mathf.Clamp(snapshot.crashRemaining, 0, CrashDuration);
            exhaustionRemaining = snapshot.exhaustionRemaining > 0 ? Mathf.Clamp(snapshot.exhaustionRemaining, 0, RescueDuration) : RescueDuration;
            borrowedSleep = Mathf.Clamp(snapshot.borrowedSleep, 0, sleep); horizontalSmoothing = 0;
        }
    }
}
