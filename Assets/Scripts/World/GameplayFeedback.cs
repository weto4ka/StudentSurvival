using System;
using UnityEngine;

namespace StudentSurvival.World
{
    public static class GameplayFeedback
    {
        public static string Key { get; private set; }
        public static string DetailKey { get; private set; }
        public static float ExpiresAt { get; private set; }
        public static event Action Changed;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { Key = null; DetailKey = null; ExpiresAt = 0; Changed = null; }
        public static void Clear() { Key = null; DetailKey = null; ExpiresAt = 0; Changed?.Invoke(); }
        public static void Show(string key, string detailKey = null)
        {
            Key = key;
            DetailKey = detailKey;
            ExpiresAt = Time.unscaledTime + 3.5f;
            Changed?.Invoke();
        }
    }
}
