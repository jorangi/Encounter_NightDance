using Encounter.NightDance.Map;

namespace Encounter.NightDance.Core.Strategies
{
    public class WalkingStrategy : IMovementStrategy
    {
        public float Calc(ITile tile)
        {
            return 1f; // TODO: 타일의 종류에 따라 이동 비용 계산, 예: 평지 1, 숲 1.5, 산 2 등
        }
    }
}