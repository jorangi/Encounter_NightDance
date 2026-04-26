using Encounter.NightDance.Core.Snapshot;
using Encounter.NightDance.Status;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace Encounter.NightDance.Core.Snapshot
{
    /// <summary>
    /// 스탯 모디파이어 스냅샷 구조체
    /// </summary>
    public struct ModifierSnapshotData
    {
        public float Value;
        public StatModifierType type;
        public int sourceId;
        public ModifierSnapshotData(float value, StatModifierType type, int sourceId)
        {
            Value = value;
            this.type = type;
            this.sourceId = sourceId;
        }
    }
    /// <summary>
    /// 유닛의 스탯과 모디파이어 스냅샷
    /// </summary>
    public abstract class UnitStatSnapshot : ISnapshot
    {
        public int TargetId {get; set;}
        public abstract int Version{get;}
        public int BaseValue {get; private set;}
        public ModifierSnapshotData[] modifiers;
        public int ModifierCount {get; private set;}
        public ReadOnlySpan<ModifierSnapshotData> Modifiers => modifiers.AsSpan(0, ModifierCount);
        public void Initialize(int targetId, Stat stat)
        {
            TargetId = targetId;
            BaseValue = stat.BaseValue;
            IReadOnlyList<StatModifier> mods = stat.Modifiers;
            int count = mods.Count;
            ModifierCount = count;

            if(count == 0) return;

            modifiers = ArrayPool<ModifierSnapshotData>.Shared.Rent(count);
            for(int i = 0; i < count; i++)
            {
                StatModifier mod = mods[i];
                this.modifiers[i] = new ModifierSnapshotData(mod.Value, mod.Type, mod.Source?.GetHashCode() ?? 0);
            }
        }
        public void Dispose()
        {
            if(modifiers != null)
            {
                ArrayPool<ModifierSnapshotData>.Shared.Return(modifiers);
                modifiers = null;
                ModifierCount = 0;
            }
        }
    }
}