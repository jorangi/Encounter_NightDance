using Encounter.NightDance.Core.Datas;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Features
{
    /// <summary>
    /// 생명력 특성
    /// </summary>
    public class MentalFeature : UnitFeatureBase, IGrowableFeature
    {
        public ObjectMental Mental {get;}
        public Stat Growth_mental{get;}
        private int _chanceMental = 0;
        
        public MentalFeature(ObjectMental mental = null, Stat growth_mental = null)
        {
            Mental = mental ?? new ObjectMental(10);
            Growth_mental = growth_mental ?? new Stat(0);
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
            if(!IsActive || damage.DamageType != DamageType.Mental) return;
            Mental.TakeDamage(damage.DamageValue);
        }
        public void ApplyGrowthOnLevelUp(int currentLevel)
        {
            int baseGrowth = Growth_mental.Value;

            _chanceMental += baseGrowth;
            while(_chanceMental >= 100)
            {
                Mental.MaxValue.IncreaseBaseValue(1);
                _chanceMental -= 100;
            }

            if(LinearCongruentialGenerator.Instance.NextFloat() * 100f < _chanceMental)
            {
                Mental.MaxValue.IncreaseBaseValue(1);
                _chanceMental = 0;
            }
        }
    }
}