using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core
{
    public interface IEquipable
    {
        /// <summary>
        /// 장비를 착용하는 함수
        /// </summary>
        /// <param name="unit"></param>
        public void Equip(IUnitCore unit);
        /// <summary>
        /// 장비를 해제하는 함수
        /// </summary>
        /// <param name="unit"></param>
        public void Unequip(IUnitCore unit);
    }
}