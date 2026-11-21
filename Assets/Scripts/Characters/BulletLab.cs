using UnityEngine;
using System.Collections;
using StudentSurvival.World;

namespace StudentSurvival.Characters
{
    public class BulletLab : MonoBehaviour
    {
        private GameObject parent;
        public GameObject Parent { set { parent = value; } get { return parent; } }

        private float speed = 10.0F;
        private Vector3 direction;
        public Vector3 Direction { set { direction = value; } }

        public Color Color
        {
            set { sprite.color = value; }
        }

        private SpriteRenderer sprite;

        // A bullet can overlap several colliders of its target in one frame, but must deal damage only once.
        private bool hit = false;

        private void Awake()
        {
            GameObject gObject = this.transform.GetChild(0).gameObject;
            gObject.SetActive(true);
            sprite = gObject.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Destroy(gameObject, 1.4F);
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (this.hit || GameSession.BlocksGameplay) return;

            // Looked up from the collider that was hit, so nothing breaks when the Prepod is already gone.
            Prepod prepod = collider.GetComponentInParent<Prepod>();

            if (prepod == null) return;

            this.hit = true;
            Destroy(gameObject);
            prepod.ReceiveDamage();
        }
    }
}
