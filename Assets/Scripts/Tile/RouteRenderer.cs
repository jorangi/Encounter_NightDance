using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Encounter.NightDance.Core;
using Encounter.NightDance.UI;
using Cysharp.Threading.Tasks;
using R3;
using Encounter.NightDance.Core.Strategies;
using System.Collections.Generic;
using Encounter.NightDance.Status;


[RequireComponent(typeof(LineRenderer))]
public class RouteRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tilemap tilemap;
    private Vector2Int unitPos = Vector2Int.zero;
    private DisposableBag disposableBag = new();

    private List<Vector2Int> currentPath = new();
    private WalkingStrategy walkingStrategy;
    private Vector2Int cachedTargetPos = new(-1, -1);

    private void Awake()
    {
        lineRenderer = lineRenderer != null ? lineRenderer : GetComponent<LineRenderer>();
    }
    private void Start()
    {
        FocusUnitService.OnFocusChangedAsObservable()
            .Subscribe(this, (u, state) =>
            {
                state.unitPos = u.Pos;
                state.ResetPath(u.Pos);
            })
            .AddTo(ref disposableBag);

        SubscribeToCameraControllerAsync().Forget();
    }
    /// <summary>
    /// 카메라 컨트롤러의 포커스 위치 변경 구독을 비동기적으로 처리
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid SubscribeToCameraControllerAsync()
    {
        while (CameraService.CameraController == null)
        {
            await UniTask.Yield();
        }

        var cameraController = CameraService.CameraController as CameraController;
        if (cameraController != null)
        {
            cameraController.OnFocusPosChanged
                .Subscribe(this, (pos, state) =>
                {
                    state.UpdatePath(pos);
                })
                .AddTo(ref disposableBag);
        }
    }
    /// <summary>
    /// 이동 전략을 초기화
    /// </summary>
    private void InitializeStrategy()
    {
        if (walkingStrategy == null)
        {
            walkingStrategy = new WalkingStrategy(MovementStrategyContainer.GetStrategySO(MovementType.Walking));
        }
    }
    /// <summary>
    /// 경로 초기화
    /// </summary>
    /// <param name="startPos"></param>
    private void ResetPath(Vector2Int startPos)
    {
        currentPath.Clear();
        currentPath.Add(startPos);
        RenderPath(currentPath);
    }
    /// <summary>
    /// 경로 렌더링
    /// </summary>
    /// <param name="paths"></param>
    private void RenderPath(List<Vector2Int> paths)
    {
        if (paths == null)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        lineRenderer.positionCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            var p = CoordinateUtility.LogicalToCell(paths[i]);
            lineRenderer.SetPosition(i, new(p.x + 0.5f, 0.1f, p.y + 0.5f));
        }
    }
    /// <summary>
    /// 경로 비용 계산
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private int CalculatePathCost(List<Vector2Int> path)
    {
        if (path == null || path.Count <= 1) return 0;
        InitializeStrategy();
        int totalCost = 0;
        for (int i = 1; i < path.Count; i++)
        {
            var tile = FieldManager.GetTile(path[i]);
            if (tile == null) return 999999;
            int tileCost = walkingStrategy.Calc(tile);
            if (tileCost >= 999) return 999999;

            totalCost += tileCost * 1000;
        }
        return totalCost;
    }
    /// <summary>
    /// 경로 업데이트
    /// </summary>
    /// <param name="targetPos"></param>
    private void UpdatePath(Vector2Int targetPos)
    {
        InitializeStrategy();
        if (currentPath.Count == 0)
        {
            ResetPath(unitPos);
        }

        var focusedUnit = FocusUnitService.CurrentTarget;
        if (focusedUnit != null && focusedUnit.Pos != unitPos)
        {
            Vector2Int newUnitPos = focusedUnit.Pos;
            int unitIndex = currentPath.IndexOf(newUnitPos);
            if (unitIndex >= 0)
            {
                currentPath.RemoveRange(0, unitIndex);
                unitPos = newUnitPos;
                RenderPath(currentPath);
            }
            else
            {
                unitPos = newUnitPos;
                ResetPath(newUnitPos);
            }
        }

        if (currentPath.Count <= 1 && currentPath[0] == targetPos)
        {
            return;
        }

        if (currentPath[currentPath.Count - 1] == targetPos)
        {
            return;
        }

        if (currentPath.Count > 1 && currentPath[currentPath.Count - 2] == targetPos)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            RenderPath(currentPath);
            return;
        }

        // 경로 연장 (인접하고 중복 방문하지 않은 경우)
        Vector2Int lastPos = currentPath[currentPath.Count - 1];
        bool isAdjacent = Mathf.Abs(lastPos.x - targetPos.x) + Mathf.Abs(lastPos.y - targetPos.y) == 1;
        bool alreadyVisited = currentPath.Contains(targetPos);
        if (isAdjacent && !alreadyVisited)
        {
            List<Vector2Int> tempPath = new(currentPath) { targetPos };
            int trajectoryCost = CalculatePathCost(tempPath);

            var optimalPath = PathFinder.GetPath(unitPos, targetPos, walkingStrategy);
            int optimalCost = CalculatePathCost(optimalPath);

            // 궤적 비용이 최적 경로 비용과 같거나 더 저렴하면 궤적 유지
            if (optimalPath != null && trajectoryCost <= optimalCost)
            {
                currentPath.Add(targetPos);
                RenderPath(currentPath);
                return;
            }
        }

        // 비효율 경로이거나 순간이동한 경우 A* 최적 경로로 덮어쓰기
        var newOptimalPath = PathFinder.GetPath(unitPos, targetPos, walkingStrategy);
        if (newOptimalPath != null)
        {
            currentPath = newOptimalPath;
            RenderPath(currentPath);
        }
    }
    public List<Vector2Int> GetRenderedPath()
    {
        return currentPath;
    }
}