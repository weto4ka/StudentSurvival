using StudentSurvival.Audio;
using UnityEngine;

namespace StudentSurvival.World
{
    public class Teleport : PlayerInteractionTarget
    {
        [SerializeField, Min(0)] private float waitingTime = 2f;
        [SerializeField] public bool avaliable = true;
        [SerializeField] private GameObject enterObject;
        [SerializeField] private GameObject exitObject;
        public Transform target;
        private TeleportController controller;
        private AudioSource audioSource;
        private AudioController lockedAudio;
        private Rigidbody2D passenger;
        private float remaining;
        private bool travelling;
        public bool NearDoors { get { return HasPlayerNearby; } }
        public bool IsAvaliable { get { return avaliable; } }
        public override string PromptKey { get { return travelling ? "door.travelling" : avaliable ? "door.open" : "door.locked"; } }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (enterObject != null && exitObject != null)
                controller = new TeleportController(waitingTime, enterObject, exitObject);
            GameObject locked = GameObject.FindGameObjectWithTag("DoorLockedAudio");
            if (locked != null) lockedAudio = locked.GetComponent<AudioController>();
            // Locked/decorative doors deliberately have no destination.
            if (avaliable && target == null) Debug.LogError("An unlocked teleport needs a destination: " + name, this);
        }
        public override void Interact(PlayerInteractor player)
        {
            if (GameSession.BlocksGameplay || travelling || !HasPlayerNearby || player == null) return;
            if (!avaliable)
            {
                if (lockedAudio != null) lockedAudio.Play();
                GameplayFeedback.Show("door.locked");
                return;
            }
            if (target == null) { GameplayFeedback.Show("door.unavailable"); return; }
            passenger = player.GetComponent<Rigidbody2D>();
            if (passenger == null) return;
            remaining = waitingTime;
            travelling = true;
            controller?.Open();
            if (audioSource != null) audioSource.Play();
        }
        // Retained for existing Inspector callbacks.
        public void Activate(Transform player)
        {
            if (player != null) Interact(player.GetComponent<PlayerInteractor>());
        }
        private void Update()
        {
            if (GameSession.BlocksGameplay) return;
            controller?.Update();
            if (!travelling) return;
            if (!HasPlayerNearby || passenger == null || target == null) { Cancel(); return; }
            remaining -= Time.deltaTime;
            if (remaining > 0) return;
            passenger.linearVelocity = Vector2.zero;
            passenger.position = target.position;
            Cancel();
        }
        private void Cancel() { travelling = false; passenger = null; controller?.Close(); }
        protected override void OnDisable() { Cancel(); base.OnDisable(); }
    }
}
