using System;
using Encounter.NightDance.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Tilemaps;

/// <summary>
/// 카메라 회전 방향, 기존 int enum에서 sbyte enum으로 변경하여 4byte -> 1byte로 메모리 최적화
/// </summary>
public enum CameraRot:sbyte
{
    Left = -1,
    Center = 0,
    Right = 1
}
/// <summary>
/// 카메라 컨트롤러
/// </summary>
public class CameraController : MonoBehaviour
{
    private MainAction _action;
    [SerializeField] CinemachineThirdPersonFollow follow;
    private const float ZoomSpeed = 15f;
    private const float MoveSpeed = 0.01f;
    private const float ROTOFFSET = 3f;
    private const float ARMMAX = 10f;
    private const float ARMMIN = -8f;
    [SerializeField]Transform focus;
    private float _targetDistance;
    [SerializeField]Grid grid;
    [SerializeField]Tilemap tilemap;
    CameraRot rot = CameraRot.Center;
    bool rotDirty = false;
    float rotatePos;
    float zoomPos;
    Vector2 movePos;
    [SerializeField]private FieldManager fieldManager;
    private Vector2Int focusPos = Vector2Int.zero;
    void Awake()
    {
        _action = new();
        fieldManager = fieldManager != null ? fieldManager : GetComponent<FieldManager>();
        follow = follow != null ? follow : GetComponent<CinemachineThirdPersonFollow>();
    }
    void Start()
    {
        Vector2Int clampedPos = FieldManager.ClampToField(0, 0);
        focusPos = clampedPos;
        Vector2 cellPos = fieldManager.GetTilePos(clampedPos);
        focus.position = new(cellPos.x, 0.1f, cellPos.y);
    }
    void OnEnable()
    {
        _action.Enable();
        _action.KeyboardControl.Rotation.performed += ctx => CameraRotate(ctx.ReadValue<float>() == -1f);
        _action.KeyboardControl.Move.performed += ctx =>
        {
            Vector2 v = ctx.ReadValue<Vector2>();
            v.x = Mathf.RoundToInt(v.x);
            v.y = -1 * Mathf.RoundToInt(v.y); //필드에서는 좌상단이 0,0이므로 Y는 반전
            Vector2Int clampedPos = FieldManager.ClampToField(focusPos.x + (int)v.x, focusPos.y + (int)v.y);
            focusPos = clampedPos;
            Vector2 cellPos = fieldManager.GetTilePos(clampedPos);
            focus.position = new(cellPos.x, focus.position.y, cellPos.y);
        };
    }
    void Update()
    {
        //마우스 우클릭 트리거 -> 회전, 줌인/아웃
        if(Input.GetMouseButtonDown(1))
        {
            rotatePos = Input.mousePosition.x;
            zoomPos = Input.mousePosition.y;
        }
        //마우스 휠클릭 트리거 -> 이동
        if(Input.GetMouseButtonDown(2))
        {
            movePos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(2))
        {
            //마우스 휠클릭 이동 종료 시 focus의 위치를 타일맵 셀에 고정
            //유닛 이동과의 일관성을 위해 GetTilePos를 사용
            Vector3 p = focus.position; //focus의 월드 좌표
            Vector2Int __p = new(Mathf.RoundToInt(p.x+0.5f), Mathf.RoundToInt(p.z-0.5f)); //focus의 월드 셀 좌표, focus의 좌표를 우선 보정
            // __p를 셀 좌표로 변환할 것
            Vector2Int offsetV = fieldManager.FocusOffset(__p); //보정된 focus의 셀 좌표에서 타일맵의 셀 좌표로 변환한 오프셋
            Vector2 v = fieldManager.GetTilePos(offsetV); //오프셋이 적용된 focus의 월드 좌표
            focus.position = new Vector3(v.x, focus.position.y, v.y);
            focusPos = offsetV; //focusPos 업데이트
            movePos = Vector2.zero;
        }
        float rotateDelta = Input.GetMouseButton(1) ? Input.mousePosition.x - rotatePos : 0.0f; // 우클릭 X 이동량
        float zoomDelta = Input.GetMouseButton(1) ? Input.mousePosition.y - zoomPos : 0.0f; // 우클릭 Y 이동량
        Vector2 moveDelta = Input.GetMouseButton(2) ? (Vector2)Input.mousePosition - movePos : Vector2.zero; //휠클릭 이동량
        //마우스 조작 회전
        if(Mathf.Abs(rotateDelta) > Screen.width * 0.3f)
        {
            CameraRotate(rotateDelta < 0);
        }
        //마우스 우클릭 조작 줌인/줌아웃
        if(Mathf.Abs(zoomDelta) > Screen.height * 0.1f)
        {
            CameraZoom(zoomDelta > 0);
        }
        //마우스 휠 스크롤 조작 줌인/줌아웃
        if(!Mathf.Approximately(Input.mouseScrollDelta.y, 0.0f))
        {
            CameraZoom(Input.mouseScrollDelta.y);
        }
        //줌 선형 보간(휠 스크롤)
        if(Mathf.Abs(follow.CameraDistance - _targetDistance) > 0.01f)
        {
            follow.CameraDistance = Mathf.Lerp(follow.CameraDistance, _targetDistance, Time.deltaTime * ZoomSpeed);
        }
        else //선형 보간 해제
        {
            follow.CameraDistance = _targetDistance;
        }
        //마우스 조작 이동
        if(!Mathf.Approximately(moveDelta.magnitude, 0.0f))
        {
            focus.transform.position += MoveSpeed * Time.deltaTime * new Vector3(-moveDelta.x, 0, -moveDelta.y);
            focus.transform.position = new(
                Mathf.Clamp(focus.position.x, tilemap.cellBounds.min.x+0.5f, tilemap.cellBounds.max.x-0.5f),
                focus.position.y,
                Mathf.Clamp(focus.position.z, tilemap.cellBounds.min.y+0.5f, tilemap.cellBounds.max.y-0.5f)
            );
        }
        //키보드 줌인/줌아웃
        if(Input.GetKey(KeyCode.R)) //줌인
        {
            CameraZoom(true);
        }
        if(Input.GetKey(KeyCode.F)) //줌아웃
        {
            CameraZoom(false);
        }
        //회전 감지
        if(rotDirty)
        {
            follow.ShoulderOffset = Vector3.Lerp(follow.ShoulderOffset, new((int)rot * ROTOFFSET, follow.ShoulderOffset.y, follow.ShoulderOffset.z), 0.1f);
            if(Mathf.Abs(follow.ShoulderOffset.x - (int)rot * ROTOFFSET) < 0.001f)
            {
                follow.ShoulderOffset.x = (int)rot * ROTOFFSET;
                rotDirty = false;
            }
        }
    }

    /// <summary>
    /// 카메라 회전 구현
    /// </summary>
    /// <param name="isLeft"></param>
    private void CameraRotate(bool isLeft)
    {
        rotatePos = Input.mousePosition.x;
        rot = isLeft ? (CameraRot)Mathf.Max(-1, (int)rot - 1) : (CameraRot)Mathf.Min(1, (int)rot + 1);
        rotDirty = true;
    }
    /// <summary>
    /// 카메라 줌인/줌아웃 구현
    /// </summary>
    /// <param name="isIn"></param>
    private void CameraZoom(bool isIn)
    {
        float zoomAmount = isIn ? -ZoomSpeed : ZoomSpeed;
       _targetDistance = Mathf.Clamp(follow.CameraDistance + 8 * zoomAmount * Time.deltaTime, ARMMIN, ARMMAX);
        // Debug.Log($"Camera Distance: {follow.CameraDistance}");
    }
    /// <summary>
    /// 카메라 줌인/줌아웃 구현 (스크롤 입력)
    /// </summary>
    /// <param name="scrollInput"></param>
    private void CameraZoom(float scrollInput)
    {
        float zoomAmount = -scrollInput;
        _targetDistance += zoomAmount;
        _targetDistance = Mathf.Clamp(_targetDistance, ARMMIN, ARMMAX);
    }
}
