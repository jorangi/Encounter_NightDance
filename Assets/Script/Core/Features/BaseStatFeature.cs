using System.Text;
using Encounter.NightDance.Core.Status;
using Encounter.NightDance.Status;
using UnityEngine;

namespace Encounter.NightDance.Core.Features
{
    public interface IBaseStats: IUnitFeature
    {
        IModifiableStat Intensity { get; }
        IModifiableStat Control { get; }
        IModifiableStat Speed { get; }
        IModifiableStat Mobility { get; }
    }
    public class BaseStatFeature : UnitFeatureBase, IBaseStats, IGrowableFeature
    {
        private Stat _intensity;
        public IModifiableStat Intensity => _intensity;
        private Stat _control;
        public IModifiableStat Control => _control;
        private Stat _speed;
        public IModifiableStat Speed => _speed;
        private Stat _mobility;
        public IModifiableStat Mobility => _mobility;
        private Stat _growth_intensity;
        private Stat _growth_control;
        private Stat _growth_speed;
        private int _chance_intensity;
        private int _chance_control;
        private int _chance_speed;

        /// <summary>
        /// 레벨업 시 스탯 상승 로직
        /// </summary>
        /// <param name="currentLevel"></param>
        public void ApplyGrowthOnLevelUp(int currentLevel)
        {
            GrowStat(_intensity, _growth_intensity, ref _chance_intensity);
            GrowStat(_control, _growth_control, ref _chance_control);
            GrowStat(_speed, _growth_speed, ref _chance_speed);
        }
        /// <summary>
        /// 레벨업 스탯 상승 계산 및 적용 메서드
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="growth"></param>
        /// <param name="chanceAccumulator"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="statName"></param>
        public void GrowStat(Stat stat, Stat growth, ref int chanceAccumulator)
        {
            StringBuilder stringBuilder = LevelingFeature.sb; // 로그용
            string statName = stat switch // 로그용
            {
                Stat s when s == _intensity => "강도",
                Stat s when s == _control => "제어",
                Stat s when s == _speed => "속도",
                _ => "알 수 없는 스탯"
            };

            int statUpCount = 0;

            int beforeValue = stat.Value;
            chanceAccumulator += growth.Value; // 누적 확률 계산

            stringBuilder.Append($"{statName} 상승: {stat.Value} -> "); // 로그용

            while (chanceAccumulator >= 100)
            {
                stat.IncreaseBaseValue(1);
                statUpCount++;
                chanceAccumulator -= 100;
            }
            if(LinearCongruentialGenerator.Instance.Next() < chanceAccumulator)
            {
                stat.IncreaseBaseValue(1);
                statUpCount++;
                chanceAccumulator = 0;
            }

            if (statUpCount > 0) // 로그용
            {
                stringBuilder.AppendLine($"{statName} 상승: {beforeValue} -> {stat.Value} (+{statUpCount} 상승)");
            }
            else
            {
                stringBuilder.AppendLine($"{statName} 유지: {beforeValue}(현재 누적 확률: {chanceAccumulator}%)"); 
            }
        }
    }
}