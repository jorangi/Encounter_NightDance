using System.Collections.Generic;
using Encounter.NightDance.Status;
using Encounter.NightDance.Core.Effects;

namespace Encounter.NightDance.Core.Datas
{
    public class ConsumableItemData : ItemData
    {
        public readonly int maxStack = 0; // 0이면 스택 제한 없음
        public int effectTurns = 0; // 0이면 즉시 효과, 1 이상이면 턴마다 효과 지속
        public List<StatModifier> statModifiers = new();
        public List<TriggeredEffect> triggeredEffects = new();
    }
}