using UnityEngine;

namespace Encounter.NightDance.Status
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDead{get;}
    }
    public interface IDamageable_M
    {
        void TakeDamage(int damage);
    }
}
