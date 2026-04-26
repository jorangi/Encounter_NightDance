using Encounter.NightDance.Core.Datas;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Features
{
    /// <summary>
    /// 생명력 특성
    /// </summary>
    public class VitalityFeature : UnitFeatureBase, IGrowableFeature, IDamageableFeature
    {
        public ObjectHealth Vitality {get;}
        public Stat Growth_vitality{get;}
        private int _chanceVitality = 0;

        public VitalityFeature(ObjectHealth vitality = null, Stat growth_vitality = null)
        {
            Vitality = vitality ?? new ObjectHealth(10);
            Growth_vitality = growth_vitality ?? new Stat(0);
        }
        public override void OnRegister(IUnitCore owner)
        {
        }
        public override void OnUnregister(IUnitCore owner)
        {
        }
        /// <summary>
        /// ObjectHealth의 TakeDamage를 호출하는 메서드
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(DamageData damage)
        {
            if(!IsActive || damage.DamageType != DamageType.Vitality) return;
            Vitality.TakeDamage(damage.DamageValue);
        }
        public void ApplyGrowthOnLevelUp(int currentLevel)
        {
            int baseGrowth = Growth_vitality.Value;

            _chanceVitality += baseGrowth;
            while(_chanceVitality >= 100)
            {
                Vitality.MaxValue.IncreaseBaseValue(1);
                _chanceVitality -= 100;
            }

            if(LinearCongruentialGenerator.Instance.NextFloat() * 100f < _chanceVitality)
            {
                Vitality.MaxValue.IncreaseBaseValue(1);
                _chanceVitality = 0;
            }
        }
    }
}