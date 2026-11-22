using System.Collections.Generic;
using UnityEngine;

namespace StudentSurvival.Cameras
{
    // Explicit world-space camera frames. Runtime rooms register once; following does not search scenes.
    public sealed class CameraBoundsRegion : MonoBehaviour
    {
        private static readonly List<CameraBoundsRegion> regions = new List<CameraBoundsRegion>();
        private Bounds worldBounds;
        private bool configured;
        public Bounds WorldBounds { get { return worldBounds; } }
        public int Revision { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { regions.Clear(); }
        public void Configure(Bounds bounds) { worldBounds = bounds; configured = true; Revision++; }
        private void OnEnable() { if (!regions.Contains(this)) regions.Add(this); }
        private void OnDisable() { regions.Remove(this); }
        public static CameraBoundsRegion At(Vector3 point)
        {
            for (int i = regions.Count - 1; i >= 0; i--)
            {
                CameraBoundsRegion region = regions[i];
                if (region == null) { regions.RemoveAt(i); continue; }
                if (!region.configured) continue;
                Bounds bounds = region.worldBounds;
                if (point.x >= bounds.min.x && point.x <= bounds.max.x && point.y >= bounds.min.y && point.y <= bounds.max.y)
                    return region;
            }
            return null;
        }
    }
}
