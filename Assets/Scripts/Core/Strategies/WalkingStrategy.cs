using Encounter.NightDance.Map;
using Encounter.NightDance.ScriptableObjects;
using UnityEngine;

namespace Encounter.NightDance.Core.Strategies
{
    public class WalkingStrategy : IMovementStrategy
    {
        private MovementStrategySO costData;
        /// <summary>
        /// 보병 유닛의 이동 전략, 지형 코스트 SO를 받아 초기화
        /// </summary>
        /// <param name="costSO"></param>
        public WalkingStrategy(MovementStrategySO costSO)
        {
            this.costData = costSO;
            costData.Initialize();
        }
        public float Calc(ITile tile)
        {
            return costData.GetCost(tile.Terrain);
        }
    }
}