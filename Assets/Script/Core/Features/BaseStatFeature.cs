using System.Collections.Generic;
using Encounter.NightDance.Status;

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
        private Dictionary<StatType, Stat> _stats = new();
        private Dictionary<StatType, Stat> _growthStats = new();
        private Dictionary<StatType, int> _chanceStats = new();

        public IModifiableStat Intensity => _stats[StatType.Intensity];
        public IModifiableStat Control => _stats[StatType.Control];
        public IModifiableStat Speed => _stats[StatType.Speed];
        public IModifiableStat Mobility => _stats[StatType.Mobility];

        public BaseStatFeature(UnitData unitData)
        {
            _stats[StatType.Intensity] = new Stat(unitData.Intensity);
            _stats[StatType.Control] = new Stat(unitData.Control);
            _stats[StatType.Speed] = new Stat(unitData.Speed);
            _stats[StatType.Mobility] = new Stat(unitData.Mobility);

            _growthStats[StatType.Intensity] = new Stat(unitData.GrowthIntensity);
            _growthStats[StatType.Control] = new Stat(unitData.GrowthControl);
            _growthStats[StatType.Speed] = new Stat(unitData.GrowthSpeed);

            foreach(StatType statType in _growthStats.Keys)
            {
                _chanceStats[statType] = 0;
            }
        }
        /// <summary>
        /// 레벨업 시 스탯 상승 로직
        /// </summary>
        /// <param name="currentLevel"></param>
        public void ApplyGrowthOnLevelUp(int currentLevel)
        {
            foreach(StatType statType in _growthStats.Keys)
            {
                GrowStat(statType);
            }
        }
        /// <summary>
        /// 레벨업 스탯 상승 계산 및 적용 메서드
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="growth"></param>
        /// <param name="chanceAccumulator"></param>
        /// <param name="stringBuilder"></param>
        /// <param name="statName"></param>
        public void GrowStat(StatType statType)
        {
            Stat stat = _stats[statType];
            Stat growth = _growthStats[statType];
            int chanceAccumulator = _chanceStats[statType];

            chanceAccumulator += growth.Value; // 누적 확률 계산


            while (chanceAccumulator >= 100)
            {
                stat.IncreaseBaseValue(1);
                chanceAccumulator -= 100;
            }
            var c = LinearCongruentialGenerator.Instance.NextFloat() * 100f;
            if(c < chanceAccumulator)
            {
                stat.IncreaseBaseValue(1);
                chanceAccumulator = 0;
            }
            _chanceStats[statType] = chanceAccumulator;
        }
    }
}