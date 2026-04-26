using System.Collections.Generic;
using Encounter.NightDance.Core.Effects;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Datas
{
    public enum EquipmentType {Armor, Weapon, Accessory}
    public enum EquipmentCategory
    {
        Sword,
        Spear,
        Bow,
        Dagger,
        Staff,
        LightArmor,
        HeaveArmor,
        Robe,
        Ring,
        Necklace
    }
    public abstract class EquipmentData : ItemData
    {
        public EquipmentType equipmentType;
        public EquipmentCategory category;
        public List<StatModifier> statModifiers = new();
        public List<TriggeredEffect> triggeredEffects = new();
    }
}