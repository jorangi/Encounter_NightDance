using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Datas
{
    public enum DamageType
    {
        Vitality,
        Mental
    }
    public sealed class DamageData
    {
        public int DamageValue{get; private set;}
        public DamageType DamageType{get; private set;}
        public IUnitCore Attacker{get; private set;}
        public bool IsCritical{get; private set;}
        public DamageData(IUnitCore attacker, int damageValue, DamageType damageType, bool isCritical = false)
        {
            this.Attacker = attacker;
            this.IsCritical = isCritical;
            this.DamageValue = damageValue;
            this.DamageType = damageType;
        }
    }
}