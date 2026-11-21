using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Characters
{
    // Shared motion, lifetime and once-only impact guard for both projectile prefabs.
    public abstract class CombatProjectile : MonoBehaviour
    {
        public GameObject Parent { get; set; }
        public Vector3 Direction { set { direction = value.normalized; } }
        public Color Color { set { if (sprite != null) sprite.color = value; } }
        protected SpriteRenderer sprite;
        private Vector3 direction;
        private float speed = 10f, remainingLifetime = 1.4f;
        private bool hit;
        protected virtual void Awake()
        {
            sprite = GetComponentInChildren<SpriteRenderer>(true);
            if (sprite != null) sprite.gameObject.SetActive(true);
        }
        protected void SetFlight(float velocity, float lifetime)
        { speed = Mathf.Max(0.1f, velocity); remainingLifetime = Mathf.Max(0.1f, lifetime); }
        protected virtual void Update()
        {
            if (GameSession.BlocksGameplay) return;
            remainingLifetime -= Time.deltaTime;
            if (remainingLifetime <= 0f) { Destroy(gameObject); return; }
            transform.position += direction * (speed * Time.deltaTime);
        }
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (hit || GameSession.BlocksGameplay || other == null) return;
            Unit target = FindTarget(other);
            if (target == null || target.gameObject == Parent) return;
            hit = true; // Before callbacks, which can end an encounter or the run.
            Destroy(gameObject);
            target.ReceiveDamage();
        }
        protected abstract Unit FindTarget(Collider2D other);
    }
}
