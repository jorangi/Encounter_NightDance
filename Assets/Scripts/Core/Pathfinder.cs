using System.Collections.Generic;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.Map;
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
                Vector2Int currentPos = current.Item1;
                int currentMovement = current.Item2;
                if(closeSet[currentPos] > currentMovement) continue;//이미 이득을 본 경우 무시
                foreach(Vector2Int dir in direction)
                {
                    Vector2Int neighbor = currentPos + dir;
                    if(!FieldManager.IsWithinField(neighbor)) continue; //필드 범위를 벗어난 경우 무시
                    ITile neighborTile = FieldManager.GetTile(neighbor);
                    int cost = strategy.Calc(neighborTile);//이동 전략에 따른 이동 비용 계산
                    int remainingMovement = currentMovement - cost;
                    if(remainingMovement < 0) continue; //이동력이 부족하면 무시
                    if(!closeSet.ContainsKey(neighbor) || closeSet[neighbor] < remainingMovement)
                    {
                        closeSet[neighbor] = remainingMovement; //더 적은 이동력으로 갱신
                        openSet.Enqueue((neighbor, remainingMovement)); //탐색할 위치와 남은 이동력을 큐에 추가
                    }
                }
            }
            return closeSet;
        }
    }
}