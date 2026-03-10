using Encounter.NightDance.Map;

namespace Encounter.NightDance.Core.Strategies
{
    public interface IMovementStrategy
    {
        /// <summary>
        /// 주어진 타일에 대한 이동 비용을 계산하는 메서드, 타일의 지형에 따라 다른 비용을 반환
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public float Calc(ITile tile);
    }
}