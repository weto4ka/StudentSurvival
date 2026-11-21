using System;
using StudentSurvival.World;
using UnityEngine;

namespace StudentSurvival.Characters
{
    public class Prepod : Monster
    {
        [SerializeField, Min(0.2f)] private float rate = 2f;
        [SerializeField, Min(1)] private int needLabs = 5;
        [SerializeField] private Color bulletColor = Color.white;
        private BulletOcenka bullet;
        private int remainingLabs;
        private bool defeated;
        public int RemainingLabs { get { return remainingLabs; } }
        public int RequiredLabs { get { return needLabs; } }
        public float ShotInterval { get { return rate; } }
        public event Action<Prepod> Defeated;
        public event Action HealthChanged;
        protected override void Awake()
        {
            remainingLabs = Mathf.Max(1, needLabs);
            bullet = Resources.Load<BulletOcenka>("BulletOcenka");
        }
        protected override void Start() { InvokeRepeating(nameof(Shoot), Mathf.Max(0.2f, rate), Mathf.Max(0.2f, rate)); }
        public void Configure(int labs, float shotInterval, Color color)
        {
            needLabs = remainingLabs = Mathf.Max(1, labs);
            rate = Mathf.Max(0.2f, shotInterval);
            bulletColor = color;
        }
        private void Shoot()
        {
            if (GameSession.BlocksGameplay || defeated || bullet == null) return;
            BulletOcenka instance = Instantiate(bullet, transform.position + Vector3.up * 0.5f, bullet.transform.rotation);
            instance.Parent = gameObject;
            instance.Direction = Vector3.left;
            instance.Color = bulletColor;
        }
        public override void ReceiveDamage()
        {
            if (GameSession.BlocksGameplay || defeated) return;
            remainingLabs = Mathf.Max(0, remainingLabs - 1);
            HealthChanged?.Invoke();
            if (remainingLabs > 0) return;
            defeated = true;
            CancelInvoke();
            // ExamProgression owns the two-encounter result. Standalone teachers still work.
            if (Defeated != null) Defeated(this); else GameSession.Win();
            Destroy(gameObject);
        }
        protected override void OnTriggerEnter2D(Collider2D collider) { }
    }
}
