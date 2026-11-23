using System.Collections.Generic;
using UnityEngine;

namespace StudentSurvival.World
{
    // Count collider pairs: one collider leaving must not cancel another overlap.
    public abstract class PlayerInteractionTarget : MonoBehaviour
    {
        private readonly Dictionary<Collider2D, int> overlaps = new Dictionary<Collider2D, int>();
        private PlayerInteractor interactor;
        public bool HasPlayerNearby { get { return interactor != null && overlaps.Count > 0; } }
        public abstract string PromptKey { get; }
        public virtual string ItemKey { get { return null; } }
        // Seconds until a cooled-down spot can be used again; the prompt shows it as m:ss.
        public virtual float CooldownSeconds { get { return 0f; } }
        public abstract void Interact(PlayerInteractor player);

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            PlayerInteractor candidate = other.GetComponentInParent<PlayerInteractor>();
            if (candidate == null || !candidate.CompareTag("Player")) return;
            if (interactor != null && candidate != interactor) return;
            interactor = candidate;
            overlaps.TryGetValue(other, out int count);
            overlaps[other] = count + 1;
            interactor.Register(this);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!overlaps.TryGetValue(other, out int count)) return;
            if (count > 1) overlaps[other] = count - 1;
            else overlaps.Remove(other);
            if (overlaps.Count == 0) Leave();
        }

        protected virtual void OnDisable() { Leave(); }
        private void Leave()
        {
            if (interactor != null) interactor.Unregister(this);
            interactor = null;
            overlaps.Clear();
        }
    }
}
