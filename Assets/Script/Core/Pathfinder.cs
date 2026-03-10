using System.Collections.Generic;
using Encounter.NightDance.Core.Strategies;
using UnityEngine;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// A* 알고리즘을 사용하여 최적의 경로를 찾는 클래스
    /// </summary>
    public static class PathFinder
    {
        public static HashSet<Vector2Int> CalculateRange(Vector2Int start, int movement, IMovementStrategy strategy)
        {
            return null; // TODO: A* 알고리즘 구현, strategy를 활용하여 이동 가능 범위 계산
        }
    }
}