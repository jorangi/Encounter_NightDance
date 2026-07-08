using System.Collections.Generic;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.Map;
using UnityEngine;
using Encounter.NightDance.ScriptableObjects;
using Encounter.NightDance.Core;
using System;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// A* 알고리즘을 사용하여 최적의 경로를 찾는 클래스
    /// </summary>
    public static class PathFinder
    {
        private class PathFinderNode : IComparable<PathFinderNode>
        {
            public Vector2Int Position { get; set; }
            public PathFinderNode Parent { get; set; }
            public int G { get; set; }
            public int H { get; set; }
            public int F => G + H;
            public Vector2Int IncomingDir { get; set; }
            public PathFinderNode(Vector2Int pos, PathFinderNode pa)
            {
                this.Position = pos;
                this.Parent = pa;
                this.IncomingDir = pa != null ? (pos - pa.Position) : Vector2Int.zero;
            }
            public int CompareTo(PathFinderNode other)
            {
                int compare = this.F.CompareTo(other.F);
                if (compare == 0)
                {
                    compare = this.H.CompareTo(other.H);
                }
                return compare;
            }
        }
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
            while (openSet.Count > 0)
            {
                (Vector2Int, int) current = openSet.Dequeue();
                Vector2Int currentPos = current.Item1;
                int currentMovement = current.Item2;
                if (closeSet[currentPos] > currentMovement) continue;//이미 이득을 본 경우 무시
                foreach (Vector2Int dir in direction)
                {
                    Vector2Int neighbor = currentPos + dir;
                    if (!FieldManager.IsWithinField(neighbor)) continue; //필드 범위를 벗어난 경우 무시
                    ITile neighborTile = FieldManager.GetTile(neighbor);
                    if (neighborTile == null) continue;
                    int cost = strategy.Calc(neighborTile);//이동 전략에 따른 이동 비용 계산
                    int remainingMovement = currentMovement - cost;
                    if (remainingMovement < 0) continue; //이동력이 부족하면 무시
                    if (!closeSet.ContainsKey(neighbor) || closeSet[neighbor] < remainingMovement)
                    {
                        closeSet[neighbor] = remainingMovement; //더 적은 이동력으로 갱신
                        openSet.Enqueue((neighbor, remainingMovement)); //탐색할 위치와 남은 이동력을 큐에 추가
                    }
                }
            }
            return closeSet;
        }
        /// <summary>
        /// A* 탐색 알고리즘으로 길 찾는 알고리즘
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<Vector2Int> GetPath(Vector2Int start, Vector2Int end, IMovementStrategy strategy, List<Vector2Int> previousPath = null)
        {
            //필요한 정보: 셀 좌표, 셀의 이동 비용, 이동 불가능여부?
            MinHeap<PathFinderNode> openList = new();
            HashSet<Vector2Int> closeList = new();
            Dictionary<Vector2Int, PathFinderNode> allNodes = new();
            Vector2Int[] direction = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            PathFinderNode startNode = new(start, null)
            {
                G = 0,
                H = GetHeuristic(start, end)
            };
            openList.Push(startNode);
            allNodes.Add(start, startNode);
            while (openList.Count > 0)
            {
                PathFinderNode current = openList.Pop();
                if (current.Position == end)
                {
                    return RetracePath(current);
                }
                if (!closeList.Add(current.Position))
                {
                    continue;
                }
                foreach (var dir in direction)
                {
                    Vector2Int neighborPos = current.Position + dir;
                    var tile = FieldManager.GetTile(neighborPos);
                    if (tile == null) continue;
                    int cost = strategy.Calc(tile);
                    if (cost >= 999) continue;

                    int moveCost = cost * 1000;
                    if (current.Parent != null)
                    {
                        Vector2Int moveDir = neighborPos - current.Position;
                        if (moveDir != current.IncomingDir)
                        {
                            moveCost += 50; // 방향 전환 페널티
                        }
                    }
                    if (previousPath != null && previousPath.Contains(neighborPos))
                    {
                        moveCost -= 60; // 이전 경로 재사용 인센티브
                    }

                    int newCost = current.G + moveCost;
                    if (closeList.Contains(neighborPos))
                    {
                        if (allNodes.TryGetValue(neighborPos, out PathFinderNode closedNode))
                        {
                            if (newCost < closedNode.G)
                            {
                                closeList.Remove(neighborPos);
                                closedNode.Parent = current;
                                closedNode.G = newCost;
                                closedNode.IncomingDir = neighborPos - current.Position;
                                openList.Push(closedNode);
                            }
                        }
                        continue;
                    }
                    if (!allNodes.TryGetValue(neighborPos, out PathFinderNode neighborNode))
                    {
                        neighborNode = new(neighborPos, current)
                        {
                            G = newCost,
                            H = GetHeuristic(neighborPos, end)
                        };
                        allNodes.Add(neighborPos, neighborNode);
                        openList.Push(neighborNode);
                    }
                    else if (newCost < neighborNode.G)
                    {
                        neighborNode.Parent = current;
                        neighborNode.G = newCost;
                        neighborNode.IncomingDir = neighborPos - current.Position;
                        openList.Push(neighborNode);
                    }
                }
            }
            return null;
        }
        private static int GetHeuristic(Vector2Int stdPos, Vector2Int endPos)
        {
            int dx = Mathf.Abs(stdPos.x - endPos.x);
            int dy = Mathf.Abs(stdPos.y - endPos.y);
            return (dx + dy) * 1000;
        }
        private static List<Vector2Int> RetracePath(PathFinderNode node)
        {
            List<Vector2Int> path = new();
            while (node != null)
            {
                path.Add(node.Position);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}