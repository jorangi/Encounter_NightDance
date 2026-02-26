using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LineRenderer))]
public class RouteRenderer : MonoBehaviour
{
    [SerializeField]private LineRenderer lineRenderer;
    [SerializeField]private Tilemap tilemap;
    private MainAction inputAction;
    private void Awake()
    {
        lineRenderer = lineRenderer != null ? lineRenderer : GetComponent<LineRenderer>();
        inputAction = new MainAction();
    }
    private void OnEnable()
    {
        inputAction.MouseControl.Enable();
        inputAction.MouseControl.Position.performed += MouseMove;
    }
    private void MouseMove(InputAction.CallbackContext context)
    {
        //마우스 위치
        Vector2 p = context.ReadValue<Vector2>();
        //카메라 기준 레이
        Ray ray = Camera.main.ScreenPointToRay(p);
        //충돌체 raycast말고 기하학적 raycast사용을 위한 plane 생성, 위방향 + 높이 0을 위해 zero
        Plane rayPlane = new(Vector3.up, Vector3.zero);
        //Physics.Raycast와는 달리 교차할 면으로부터 Raycast, 파라미터는 ray와 distance를 받는다.
        //즉, 무한한 면인 plane과 ray의 접점을 찾는다.
        if(rayPlane.Raycast(ray, out float h))
        {
            //ray.GetPoint는 마우스의 좌표를 내부적으로 가지고 있으며 distance를 통해 접점을 계산
            Vector3 hitPoint = ray.GetPoint(h);
            //접점을 이용해 타일 위치 받아옴
            Vector3Int cellPos = tilemap.WorldToCell(hitPoint);
            //타일 유무 조건 분기
            if(tilemap.HasTile(cellPos))
                Debug.DrawLine(hitPoint, hitPoint + Vector3.up, Color.cyan);
        }
    }
}
