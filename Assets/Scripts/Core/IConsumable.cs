using Encounter.NightDance.Status;
namespace Encounter.NightDance.Core
{
    public interface IConsumable
    {
        /// <summary>
        /// 소비 아이템을 사용하는 함수
        /// </summary>
        /// <param name="unit"></param>
        public void Consume(IUnitCore source, IUnitCore target);
    }
}