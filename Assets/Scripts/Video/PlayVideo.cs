using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace StudentSurvival.Video
{
    [RequireComponent (typeof(AudioSource))]
    [RequireComponent (typeof(VideoPlayer))]

    public class PlayVideo : MonoBehaviour {

        // MovieTexture was removed from Unity, so the clip is now a VideoClip played by a VideoPlayer
        public VideoClip movie;
        // Renamed from "audio": that name hid the obsolete Component.audio property (warning CS0108).
        private AudioSource audioSource;
        private VideoPlayer videoPlayer;
        private RenderTexture renderTexture;

        void Start () {
            audioSource = GetComponent<AudioSource>();
            videoPlayer = GetComponent<VideoPlayer>();

            // The video is rendered into a RenderTexture, which the RawImage then displays
            renderTexture = new RenderTexture((int)movie.width, (int)movie.height, 0);
            GetComponent<RawImage>().texture = renderTexture;

            videoPlayer.playOnAwake = false;
            videoPlayer.clip = movie;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;

            // Send the sound of the video through the AudioSource on this object
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);

            videoPlayer.Play();
        }

        void Update () {
            Keyboard keyboard = Keyboard.current;

            // Space toggles between pause and play
            if (keyboard == null || !keyboard.spaceKey.wasPressedThisFrame) return;

            if (videoPlayer.isPlaying){
                videoPlayer.Pause();
            }
            else {
                videoPlayer.Play();
            }
        }

        void OnDestroy () {
            // Free the RenderTexture that was created in Start
            if (renderTexture != null) {
                renderTexture.Release();
            }
        }
    }
}
