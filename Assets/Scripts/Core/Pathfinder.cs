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
        /// <summary>
        /// 경로 탐색에 필요한 정보를 담고 있는 노드 구조체
        /// </summary>
        private struct PathFinderNode : IComparable<PathFinderNode>
        {
            /// <summary>
            /// 노드의 위치
            /// </summary>
            public Vector2Int Position { get; set; }
            /// <summary>
            /// 노드의 부모 노드 위치(구조체로 변경하면서 reference 대신 value type을 사용하기 위해 변경)
            /// </summary>
            public Vector2Int Parent {get; set;}
            /// <summary>
            /// 부모 노드로부터 해당 노드까지 오는데 사용된 이동력
            /// </summary>
            public int G { get; set; }
            /// <summary>
            /// 해당 노드에서 목표 지점까지 가는데 사용될 휴리스틱 값
            /// </summary>
            public int H { get; set; }
            /// <summary>
            /// F = G + H
            /// </summary>
            public readonly int F => G + H;
            /// <summary>
            /// 해당 노드로 들어온 방향
            /// </summary>
            public Vector2Int IncomingDir { get; set; }
            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="pos">노드의 위치</param>
            /// <param name="pa">부모 노드</param>
            public PathFinderNode(Vector2Int pos, PathFinderNode? pa)
            {
                this.Position = pos;
                this.G = 0;
                this.H = 0;
                if(pa.HasValue)
                {
                    this.Parent = pa.Value.Position;
                    this.IncomingDir = Parent != -Vector2Int.one ? (pos - this.Parent) : Vector2Int.zero;
                }
                else
                {
                    this.Parent = -Vector2Int.one;
                    this.IncomingDir = Vector2Int.zero;
                }
            }
            /// <summary>
            /// 최소 힙 정렬을 위한 비교 메서드, F값을 기준으로 비교하며, F값이 같으면 H값을 기준으로 비교함
            /// </summary>
            /// <param name="other">비교 대상 노드</param>
            /// <returns></returns>
            public readonly int CompareTo(PathFinderNode other)
            {
                int compare = this.F.CompareTo(other.F);
                if (compare == 0)
                {
                    compare = this.H.CompareTo(other.H);
                }
                return compare;
            }
            /// <summary>
            /// 부모 노드를 설정하는 메서드
            /// </summary>
            /// <param name="parent">부모 노드</param>
            public void SetParent(PathFinderNode parent)
            {
                Parent = parent.Position;
            }
        }
        
        private static readonly List<Vector2Int> path = new();
        private static readonly MinHeap<PathFinderNode> openList = new();
        private static readonly HashSet<(Vector2Int, Vector2Int)> closeList = new();
        private static readonly Dictionary<(Vector2Int, Vector2Int), PathFinderNode> allNodes = new();
        private static readonly Vector2Int[] direction = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        private static readonly MinHeap<(int, Vector2Int)> openSet = new();
        /// <summary>
        /// 시작 지점에서 이동 가능한 범위를 계산하는 메서드, 반환의 key는 좌표, value는 남는 이동력 + 1, 최소힙은 낭비가 심한 것부터 탐색하기에 이동력을 -로 취해 가장 이동력이 적은 것부터 탐색하는 원래 의도에 맞게끔 수정
        /// </summary>
        /// <param name="start">시작 지점</param>
        /// <param name="movement">이동력</param>
        /// <param name="strategy">이동 전략</param>
        /// <param name="resultDic">결과 저장 딕셔너리, 기존 함수 내 변수들을 외부 static으로 변경하였기 때문에 매개변수로 원본을 전달</param>
        public static void GetMoveRange(Vector2Int start, int movement, IMovementStrategy strategy, Dictionary<Vector2Int, int> resultDic)
        {
            openSet.Clear();
            resultDic.Clear();
            openSet.Push((-movement, start));
            resultDic.Add(start, movement);
            while (openSet.Count > 0)
            {
                (int, Vector2Int) current = openSet.Pop();
                int currentMovement = -current.Item1;
                Vector2Int currentPos = current.Item2;
                if (resultDic[currentPos] > currentMovement) continue;//이미 이득을 본 경우 무시
                foreach (Vector2Int dir in direction)
                {
                    Vector2Int neighbor = currentPos + dir;
                    if (!FieldManager.IsWithinField(neighbor)) continue; //필드 범위를 벗어난 경우 무시
                    ITile neighborTile = FieldManager.GetTile(neighbor);
                    if (neighborTile == null) continue;
                    int cost = strategy.Calc(neighborTile);//이동 전략에 따른 이동 비용 계산
                    int remainingMovement = currentMovement - cost;
                    if (remainingMovement < 0) continue; //이동력이 부족하면 무시
                    if (!resultDic.ContainsKey(neighbor) || resultDic[neighbor] < remainingMovement)
                    {
                        resultDic[neighbor] = remainingMovement; //더 적은 이동력으로 갱신
                        openSet.Push((-remainingMovement, neighbor)); //탐색할 위치와 남은 이동력을 큐에 추가
                    }
                }
            }
        }
        /// <summary>
        /// A* 탐색 알고리즘으로 길 찾는 알고리즘
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static void GetPath(Vector2Int start, Vector2Int end, IMovementStrategy strategy, List<Vector2Int> resultPath, List<Vector2Int> previousPath = null)
        {
            //필요한 정보: 셀 좌표, 셀의 이동 비용, 이동 불가능여부?
            openList.Clear();
            closeList.Clear();
            allNodes.Clear();
            PathFinderNode startNode = new(start, null)
            {
                G = 0,
                H = GetHeuristic(start, end)
            };
            openList.Push(startNode);
            allNodes.Add((start, Vector2Int.zero), startNode);
            while (openList.Count > 0)
            {
                PathFinderNode current = openList.Pop();
                if (current.Position == end)
                {
                    RetracePath(current, allNodes, resultPath);
                    return;
                }
                if (!closeList.Add((current.Position, current.IncomingDir)))
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
                    if (current.Parent != -Vector2Int.one)
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
                    if (closeList.Contains((neighborPos, dir)))
                    {
                        if (allNodes.TryGetValue((neighborPos, dir), out PathFinderNode closedNode))
                        {
                            if (newCost < closedNode.G)
                            {
                                closeList.Remove((neighborPos, dir));
                                closedNode.Parent = current.Position;
                                closedNode.G = newCost;
                                closedNode.IncomingDir = neighborPos - current.Position;
                                openList.Push(closedNode);
                                allNodes[(neighborPos, dir)] = closedNode;
                            }
                        }
                        continue;
                    }
                    if (!allNodes.TryGetValue((neighborPos, dir), out PathFinderNode neighborNode))
                    {
                        neighborNode = new(neighborPos, current)
                        {
                            G = newCost,
                            H = GetHeuristic(neighborPos, end)
                        };
                        allNodes.Add((neighborPos, dir), neighborNode);
                        openList.Push(neighborNode);
                    }
                    else if (newCost < neighborNode.G)
                    {
                        neighborNode.Parent = current.Position;
                        neighborNode.G = newCost;
                        neighborNode.IncomingDir = dir;
                        openList.Push(neighborNode);
                        allNodes[(neighborPos, dir)] = neighborNode;
                    }
                }
            }
        }
        /// <summary>
        /// 맨해튼 거리를 이용하여 휴리스틱 값을 계산하는 메서드
        /// </summary>
        /// <param name="stdPos">시작 지점</param>
        /// <param name="endPos">끝 지점</param>
        /// <returns></returns>
        private static int GetHeuristic(Vector2Int stdPos, Vector2Int endPos)
        {
            int dx = Mathf.Abs(stdPos.x - endPos.x);
            int dy = Mathf.Abs(stdPos.y - endPos.y);
            return (dx + dy) * 1000;
        }
        /// <summary>
        /// 경로 추적 메서드
        /// </summary>
        /// <param name="node">현재 노드</param>
        /// <param name="allNodes">모든 노드 딕셔너리</param>
        /// <param name="resultPath">결과 경로 리스트</param>
        private static void RetracePath(PathFinderNode node, Dictionary<(Vector2Int, Vector2Int), PathFinderNode> allNodes, List<Vector2Int> resultPath)
        {
            Vector2Int currentPos = node.Position;
            Vector2Int currentDir = node.IncomingDir;
            while (currentPos != -Vector2Int.one)
            {
                resultPath.Add(currentPos);
                if(allNodes.TryGetValue((currentPos, currentDir), out PathFinderNode currentNode))
                {
                    currentDir = currentNode.IncomingDir;
                    currentPos = currentNode.Parent;
                }
                else break;
            }
            resultPath.Reverse();
        }
    }
}