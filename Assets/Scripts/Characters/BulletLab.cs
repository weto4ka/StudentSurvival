using UnityEngine;
namespace StudentSurvival.Characters
{
    public class BulletLab : CombatProjectile
    {
        protected override Unit FindTarget(Collider2D other) { return other.GetComponentInParent<Prepod>(); }
    }
}
