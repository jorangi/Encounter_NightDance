using System;
using Encounter.NightDance.Character;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public class ObjectMental : MonoBehaviour, IDamageable_M
    {
        public Stat MaxMp {get; private set;}
        public int CurMp {get; private set;}
        public event Action OnDamaged;
        public event Action OnAlived;
        public event Action OnContamination;
        public bool IsAlive => CurMp > 0;
        public bool ProceedContamination => CurMp <= 0;
        public void TakeDamage(int damage)
        {
            CurMp -= damage;
            CurMp = Mathf.Max(CurMp, 0);
            OnDamaged?.Invoke();
            if(ProceedContamination) OnContamination?.Invoke();
            else OnAlived?.Invoke();
        }
    }
}