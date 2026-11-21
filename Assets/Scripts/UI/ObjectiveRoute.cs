using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.UI
{
    // Small directed graph: walk along a floor, then use a door to its actual destination.
    public sealed class ObjectiveRoute : MonoBehaviour
    {
        private Teleport[] doors;
        private Vector3[] points;
        private float[] distances;
        private int[] previous;
        private bool[] visited;
        private void Awake()
        {
            doors = FindObjectsByType<Teleport>(FindObjectsSortMode.None);
            int count = doors.Length * 2 + 2;
            points = new Vector3[count]; distances = new float[count]; previous = new int[count]; visited = new bool[count];
        }
        public Vector3 Next(Vector3 from, Vector3 goal)
        {
            int end = points.Length - 1; points[0] = from; points[end] = goal;
            for (int i = 0; i < doors.Length; i++)
            {
                bool enabled = doors[i] != null && doors[i].isActiveAndEnabled && doors[i].IsAvaliable && doors[i].target != null;
                points[i * 2 + 1] = enabled ? doors[i].transform.position : Vector3.one * 100000;
                points[i * 2 + 2] = enabled ? doors[i].target.position : Vector3.one * 100000;
            }
            for (int i = 0; i <= end; i++) { distances[i] = float.PositiveInfinity; previous[i] = -1; visited[i] = false; }
            distances[0] = 0;
            for (int step = 0; step <= end; step++)
            {
                int node = -1; float best = float.PositiveInfinity;
                for (int i = 0; i <= end; i++) if (!visited[i] && distances[i] < best) { node = i; best = distances[i]; }
                if (node < 0 || node == end) break;
                visited[node] = true;
                for (int next = 1; next <= end; next++)
                {
                    bool portal = node % 2 == 1 && node < end && next == node + 1;
                    if (!portal && Mathf.Abs(points[node].y - points[next].y) > 3) continue;
                    float cost = portal ? 3 : Mathf.Abs(points[node].x - points[next].x);
                    float distance = best + cost;
                    if (distance >= distances[next]) continue;
                    distances[next] = distance; previous[next] = node;
                }
            }
            if (previous[end] < 0) return goal;
            int target = end;
            for (int safety = 0; safety <= end && previous[target] > 0; safety++) target = previous[target];
            return points[target];
        }
    }
}
