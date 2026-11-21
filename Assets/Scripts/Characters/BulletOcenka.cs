using UnityEngine;
namespace StudentSurvival.Characters
{
    public class BulletOcenka : CombatProjectile
    {
        public void ConfigureLane()
        {
            SetFlight(8f, 2.1f);
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            if (circle != null) { circle.radius = 0.22f; circle.offset = Vector2.zero; }
            if (sprite != null) sprite.transform.localScale = Vector3.one * 0.44f;
        }
        protected override Unit FindTarget(Collider2D other) { return other.GetComponentInParent<Character>(); }
    }
}
