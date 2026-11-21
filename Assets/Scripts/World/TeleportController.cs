using UnityEngine;

namespace StudentSurvival.World
{
    public enum ObjectState { Close, Open }
    public sealed class TeleportController
    {
        private static readonly int State = Animator.StringToHash("State");
        private readonly Animator entrance;
        private readonly Animator exit;
        private readonly float duration;
        private float remaining;
        public TeleportController(float waitingTime, GameObject enterObject, GameObject exitObject)
        {
            duration = Mathf.Max(0, waitingTime);
            entrance = enterObject != null ? enterObject.GetComponent<Animator>() : null;
            exit = exitObject != null ? exitObject.GetComponent<Animator>() : null;
        }
        public void Update() { if (remaining > 0 && (remaining -= Time.deltaTime) <= 0) Close(); }
        public void Open() { remaining = duration; Set(ObjectState.Open); }
        public void Close() { remaining = 0; Set(ObjectState.Close); }
        private void Set(ObjectState state)
        {
            if (entrance != null) entrance.SetInteger(State, (int)state);
            if (exit != null) exit.SetInteger(State, (int)state);
        }
    }
}
