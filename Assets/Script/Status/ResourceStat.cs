using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    [Serializable]
    public abstract class ResourceStat
    {
        [field: SerializeField]public Stat MaxValue {get; private set;}
        [field: SerializeField]public int CurValue {get; private set;}
        public ResourceStat(int baseValue)
        {
            MaxValue = new(baseValue);
            CurValue = MaxValue.Value;
        }
        public virtual void TakeDamage(int value)
        {
            CurValue = Mathf.Clamp(CurValue - value, 0, MaxValue.Value);
            OnValueCheck(value);
        }
        public void ApplySnapshot(int curValue)
        {
            CurValue = Mathf.Clamp(curValue, 0, MaxValue.Value);
        }
        public abstract void OnValueCheck(int value);
    }
}
