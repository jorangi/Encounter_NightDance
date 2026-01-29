using UnityEngine;

namespace Encounter.NightDance.Character
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDead{get;}
    }
}
