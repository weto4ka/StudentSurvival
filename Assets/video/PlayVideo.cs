using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(VideoPlayer))]

public class PlayVideo : MonoBehaviour {

	// MovieTexture was removed from Unity, so the clip is now a VideoClip played by a VideoPlayer
	public VideoClip movie;
	private AudioSource audio;
	private VideoPlayer videoPlayer;
	private RenderTexture renderTexture;


	
	void Start () {
		audio = GetComponent<AudioSource>();
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
		videoPlayer.SetTargetAudioSource(0, audio);

		videoPlayer.Play();
	}
	
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) && videoPlayer.isPlaying){
			videoPlayer.Pause();
		}
		else if (Input.GetKeyDown(KeyCode.Space) && !videoPlayer.isPlaying){
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
