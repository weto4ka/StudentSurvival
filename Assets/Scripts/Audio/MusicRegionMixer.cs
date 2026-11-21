using System.Collections.Generic;
using UnityEngine;

namespace StudentSurvival.Audio
{
    // Shared playback cursors let adjacent regions and scene changes crossfade without restarting clips.
    public sealed class MusicRegionMixer : MonoBehaviour
    {
        private sealed class Track
        {
            public AudioSource Source;
            public float Gain, Volume;
            public bool Running;
        }
        private static MusicRegionMixer instance;
        private readonly List<AudioSource> owners = new List<AudioSource>();
        private readonly Dictionary<AudioClip, Track> tracks = new Dictionary<AudioClip, Track>();
        private Track selected;
        private const float FadeSeconds = 0.6f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { instance = null; }

        public static void Play(AudioSource owner)
        {
            if (owner == null || owner.clip == null) return;
            owner.playOnAwake = false;
            if (owner.isPlaying) owner.Pause(); // The authored source describes the voice, never a second playback.
            if (instance == null)
            {
                instance = new GameObject("Music crossfades").AddComponent<MusicRegionMixer>();
                DontDestroyOnLoad(instance.gameObject);
            }
            // The most recently entered region wins overlapping volumes. Re-entry updates priority
            // even if its exit grace is still active. Identical clips always share one voice.
            instance.owners.Remove(owner);
            instance.owners.Add(owner);
            instance.SelectOwner();
        }
        public static void Stop(AudioSource owner)
        {
            if (instance == null || !instance.owners.Remove(owner)) return;
            instance.SelectOwner();
        }
        private void SelectOwner()
        {
            selected = null;
            for (int i = owners.Count - 1; i >= 0; i--)
            {
                AudioSource owner = owners[i];
                if (owner == null || !owner.isActiveAndEnabled || owner.clip == null) { owners.RemoveAt(i); continue; }
                if (!tracks.TryGetValue(owner.clip, out Track track))
                {
                    AudioSource playback = gameObject.AddComponent<AudioSource>();
                    playback.playOnAwake = false; playback.loop = true; playback.clip = owner.clip;
                    playback.volume = 0; playback.pitch = owner.pitch; playback.priority = owner.priority;
                    playback.outputAudioMixerGroup = owner.outputAudioMixerGroup;
                    playback.ignoreListenerPause = owner.ignoreListenerPause;
                    playback.mute = owner.mute; playback.spatialBlend = 0;
                    if (owner.timeSamples > 0 && owner.timeSamples < owner.clip.samples) playback.timeSamples = owner.timeSamples;
                    track = new Track { Source = playback };
                    tracks.Add(owner.clip, track);
                }
                track.Volume = owner.volume;
                selected = track;
                break;
            }
        }
        private void Update()
        {
            // Disabled/destroyed scene owners must not remain as requests in this persistent mixer.
            bool removed = false;
            for (int i = owners.Count - 1; i >= 0; i--)
                if (owners[i] == null || !owners[i].isActiveAndEnabled || owners[i].clip == null)
                { owners.RemoveAt(i); removed = true; }
            if (removed) SelectOwner();
            // Music follows AudioListener pause; UI cues have their separate ignoreListenerPause source.
            // Do not ramp gains or call Play while the listener is paused (isPlaying is false then).
            if (AudioListener.pause) return;
            foreach (Track track in tracks.Values)
            {
                bool wanted = track == selected;
                if (wanted && !track.Running)
                {
                    track.Source.UnPause();
                    if (!track.Source.isPlaying) track.Source.Play();
                    track.Running = true;
                }
                track.Gain = Mathf.MoveTowards(track.Gain, wanted ? 1f : 0f, Time.deltaTime / FadeSeconds);
                track.Source.volume = track.Volume * track.Gain;
                if (!wanted && track.Running && track.Gain <= 0f)
                {
                    track.Source.Pause(); // Preserve the cursor for the next region/scene requesting this clip.
                    track.Running = false;
                }
            }
        }
        private void OnDestroy() { if (instance == this) instance = null; }
    }
}
