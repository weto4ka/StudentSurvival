using System.Collections.Generic;
using StudentSurvival.Controls;
using UnityEngine;

namespace StudentSurvival.World
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        private readonly List<PlayerInteractionTarget> nearby = new List<PlayerInteractionTarget>();
        public PlayerInteractionTarget Target { get; private set; }
        public void Register(PlayerInteractionTarget target) { if (!nearby.Contains(target)) nearby.Add(target); }
        public void Unregister(PlayerInteractionTarget target)
        {
            nearby.Remove(target);
            if (Target == target) Target = null;
        }
        private void Update()
        {
            Target = null;
            if (GameSession.BlocksGameplay) return;
            float bestDistance = float.PositiveInfinity;
            for (int i = nearby.Count - 1; i >= 0; i--)
            {
                PlayerInteractionTarget target = nearby[i];
                if (target == null || !target.isActiveAndEnabled || !target.HasPlayerNearby)
                { nearby.RemoveAt(i); continue; }
                float distance = (target.transform.position - transform.position).sqrMagnitude;
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                Target = target;
            }
            if (Target != null && GameInput.ActionPressed) Target.Interact(this);
        }
    }
}
