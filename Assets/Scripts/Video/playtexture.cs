using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace StudentSurvival.Video
{
    public class playtexture : MonoBehaviour {
        // MovieTexture was removed from Unity, VideoPlayer is its replacement.
        // Assign a VideoPlayer component (with a VideoClip set on it) in the Inspector.
        public VideoPlayer movie;

        // Use this for initialization
        void Start () {
            movie.Play();
        }

        // Update is called once per frame
        void Update () {

        }
    }
}
