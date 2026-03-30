namespace Encounter.NightDance.Core.Datas
{
    public class ItemData : EntityData
    {
        public enum ItemGrade { Common, Uncommon, Rare, Epic, Legendary }
        public ItemGrade Grade;
    }
}