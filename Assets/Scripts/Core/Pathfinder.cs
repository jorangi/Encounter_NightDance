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
        /// <summary>
        /// 시작 지점에서 이동 가능한 범위를 계산하는 메서드, 반환의 key는 좌표, value는 남는 이동력
        /// </summary>
        /// <param name="start"></param>
        /// <param name="movement"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static Dictionary<Vector2Int, int> GetMoveRange(Vector2Int start, int movement, IMovementStrategy strategy)
        {
            Queue<(Vector2Int, int)> openSet = new(); //튜플로 좌표와 남은 이동력을 함께 저장
            Dictionary<Vector2Int, int> closeSet = new();
            Vector2Int[] direction = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            openSet.Enqueue((start, movement));
            closeSet.Add(start, movement);
            while(openSet.Count > 0)
            {
                (Vector2Int, int) current = openSet.Dequeue();
                foreach(Vector2Int dir in direction)
                {
                    Vector2Int neighbor = current.Item1 + dir;
                    if(!FieldManager.IsWithinField(neighbor)) continue; //필드 범위를 벗어난 경우 무시

                    //int cost = strategy.Calc(neighborTile)
                    
                }
            }
            return null;
        }
    }
}