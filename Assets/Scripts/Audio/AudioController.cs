using System.Collections.Generic;
using UnityEngine;

namespace StudentSurvival.Audio
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private bool playTriggered = false;
        [SerializeField] private bool stopTriggered = false;
        [SerializeField, Min(0)] private float musicExitGrace = 1.25f;
        private AudioSource audioSource;
        private bool automaticMusic;
        private readonly Dictionary<Collider2D, int> overlaps = new Dictionary<Collider2D, int>();
        private readonly List<Collider2D> expired = new List<Collider2D>();
        private float exitAt = float.PositiveInfinity, nextCleanup;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioSource.loop)
            {
                automaticMusic = audioSource.playOnAwake;
                audioSource.playOnAwake = false;
            }
        }
        private void OnEnable() { if (automaticMusic) Play(); }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            bool first = overlaps.Count == 0;
            overlaps.TryGetValue(other, out int count);
            overlaps[other] = count + 1;
            exitAt = float.PositiveInfinity;
            if (first && playTriggered) Play();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!overlaps.TryGetValue(other, out int count)) return;
            if (count > 1) overlaps[other] = count - 1; else overlaps.Remove(other);
            if (overlaps.Count == 0) ScheduleExit();
        }
        private void ScheduleExit()
        {
            if (!stopTriggered) return;
            // Keep the same request through a brief airborne exit, rather than restarting on landing.
            if (audioSource != null && audioSource.loop) exitAt = Time.time + musicExitGrace;
            else Stop();
        }
        private void Update()
        {
            if (Time.time >= nextCleanup)
            {
                nextCleanup = Time.time + 0.25f;
                expired.Clear();
                foreach (var pair in overlaps)
                    if (pair.Key == null || !pair.Key.enabled || !pair.Key.gameObject.activeInHierarchy) expired.Add(pair.Key);
                foreach (Collider2D collider in expired) overlaps.Remove(collider);
                if (expired.Count > 0 && overlaps.Count == 0) ScheduleExit();
            }
            if (Time.time < exitAt || overlaps.Count != 0) return;
            exitAt = float.PositiveInfinity;
            Stop();
        }
        public void Play()
        {
            if (audioSource == null) return;
            exitAt = float.PositiveInfinity;
            if (audioSource.loop) MusicRegionMixer.Play(audioSource);
            else if (!audioSource.isPlaying) audioSource.Play();
        }
        public void Stop()
        {
            exitAt = float.PositiveInfinity;
            if (audioSource == null) return;
            if (audioSource.loop) MusicRegionMixer.Stop(audioSource);
            else if (audioSource.isPlaying) audioSource.Stop();
        }
        private void OnDisable()
        {
            overlaps.Clear(); expired.Clear();
            exitAt = float.PositiveInfinity;
            MusicRegionMixer.Stop(audioSource);
        }
        private static bool IsPlayer(Collider2D other)
        {
            return other != null && (other.CompareTag("Player") ||
                (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")));
        }
    }
}
