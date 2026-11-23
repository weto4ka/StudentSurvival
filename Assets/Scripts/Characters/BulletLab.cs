using UnityEngine;
namespace StudentSurvival.Characters
{
    public class BulletLab : CombatProjectile
    {
        // The prefab draws at order 1, which is behind the authored room backdrops (order 10)
        // in rooms 2 and 3, so the thrown lab was invisible there. Draw above backdrops, props
        // and teachers (order 50), like the teacher's grades (order 103).
        private const int MinimumSortingOrder = 60;
        protected override void Awake()
        {
            base.Awake();
            if (sprite != null) sprite.sortingOrder = Mathf.Max(sprite.sortingOrder, MinimumSortingOrder);
        }
        protected override Unit FindTarget(Collider2D other) { return other.GetComponentInParent<Prepod>(); }
    }
}
