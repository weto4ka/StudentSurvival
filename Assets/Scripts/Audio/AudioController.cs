using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StudentSurvival.Audio
{
    public class AudioController : MonoBehaviour {

        [SerializeField]
        private bool playTriggered = false;
        [SerializeField]
        private bool stopTriggered = false;

        private AudioSource audioSource;

        private void Awake()
        {
            this.audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(this.playTriggered && IsPlayer(other))
            {
                this.Play();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (this.stopTriggered && IsPlayer(other))
            {
                this.Stop();
            }
        }

        public void Play()
        {
            if(this.audioSource != null && !this.audioSource.isPlaying)
            {
                this.audioSource.Play();
            }
        }

        public void Stop()
        {
            if(this.audioSource != null && this.audioSource.isPlaying)
            {
                this.audioSource.Stop();
            }
        }
        private static bool IsPlayer(Collider2D other)
        {
            return other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"));
        }
    }
}
