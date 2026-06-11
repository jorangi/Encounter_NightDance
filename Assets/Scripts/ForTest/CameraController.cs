using System;
using Encounter.NightDance.Core;
using Unity.Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Tilemaps;
using System.Threading;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

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
    private Vector2 _moveInput;
    private CancellationTokenSource _moveCts;
    private const float _moveIntervalSeconds = 0.1f;
    private const float INPUT_THRESHOLD = 0.05f;
    private float _lastMoveTime;
    private Vector2Int focusPos = Vector2Int.zero;
    private float _zoomInput;
    private const float ZOOM_SENSITIVITY = 0.2f;
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
        Vector3 worldPos = CoordinateUtility.GetWorldPos(clampedPos);
        focus.position = new(worldPos.x, 0.1f, worldPos.z);
    }
    void OnEnable()
    {
        _action.Enable();
        _action.Camera.Rotation.started += OnRotateCamera;
        _action.Camera.Move.performed += OnMovePerformed;
        _action.Camera.Move.canceled += OnMoveCanceled;
    }
    void OnDisable()
    {
        _moveCts?.Cancel();
        _action.Camera.Rotation.started -= OnRotateCamera;
        _action.Camera.Move.performed -= OnMovePerformed;
        _action.Camera.Move.canceled -= OnMoveCanceled;
        _action.Disable();
    }
    void Update()
    {
        //마우스 우클릭 트리거 -> 회전, 줌인/아웃
        if(_action.MouseControl.Right.WasPressedThisFrame())
        {
            rotatePos = _action.MouseControl.Position.ReadValue<Vector2>().x;
            zoomPos = _action.MouseControl.Position.ReadValue<Vector2>().y;
        }
        //마우스 휠클릭 트리거 -> 이동
        if(_action.MouseControl.Middle.WasPressedThisFrame())
        {
            movePos = _action.MouseControl.Position.ReadValue<Vector2>();
        }
        if(_action.MouseControl.Middle.WasReleasedThisFrame())
        {
            Vector2Int logicalPos = fieldManager.GetTilePos(focus.position);
            logicalPos = FieldManager.ClampToField(logicalPos.x, logicalPos.y);
            Vector3 snappedWorldPos = CoordinateUtility.GetWorldPos(logicalPos);
            focus.position = new Vector3(snappedWorldPos.x, focus.position.y, snappedWorldPos.z);
            focusPos = logicalPos;
            movePos = Vector2.zero;
        }
        Vector2 mousePos = _action.MouseControl.Position.ReadValue<Vector2>();
        float rotateDelta = _action.MouseControl.Right.IsPressed() ? mousePos.x - rotatePos : 0.0f; // 우클릭 X 이동량
        float zoomDelta = _action.MouseControl.Right.IsPressed() ? mousePos.y - zoomPos : 0.0f; // 우클릭 Y 이동량
        Vector2 moveDelta = _action.MouseControl.Middle.IsPressed() ? mousePos - movePos : Vector2.zero; //휠클릭 이동량
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
        if(!Mathf.Approximately(_action.MouseControl.Scroll.ReadValue<float>(), 0.0f))
        {
            _zoomInput = _action.MouseControl.Scroll.ReadValue<float>();
        }
        if(_action.Camera.Zoom.ReadValue<float>() != 0)
        {
            _zoomInput = _action.Camera.Zoom.ReadValue<float>() * ZOOM_SENSITIVITY; //키보드 줌 입력은 마우스 휠 입력보다 덜 민감하게 처리
        }
        CameraZoom(_zoomInput);
        _zoomInput = 0.0f;
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
    /// 비동기 키보드 이동 UniTask, 홀드용
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTaskVoid Move(CancellationToken token)
    {
        Move();
        _lastMoveTime = Time.time;
        if(await UniTask.Delay(500, cancellationToken: token).SuppressCancellationThrow()) return;
        
        while(!token.IsCancellationRequested)
        {
            Move();
            _lastMoveTime = Time.time;
            await UniTask.Delay((int)(_moveIntervalSeconds * 1000), cancellationToken: token);
        }
    }
    /// <summary>
    /// 이동 입력 처리
    /// </summary>
    /// <param name="ctx"></param>
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 _curInput = ctx.ReadValue<Vector2>();
            _curInput.x = Mathf.RoundToInt(_curInput.x);
            _curInput.y = -1 * Mathf.RoundToInt(_curInput.y);


            //키보드 이동이 바뀌면 초기화
            if(_curInput != _moveInput)
            {
                if(Time.time - _lastMoveTime < INPUT_THRESHOLD) return;
                _moveInput = _curInput;
                _moveCts?.Cancel();
                _moveCts = new();
                Move(_moveCts.Token).Forget();
            }
    }
    /// <summary>
    /// 이동 입력 취소
    /// </summary>
    /// <param name="ctx"></param>
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveCts?.Cancel();
        _moveInput = Vector2.zero;
    }
    /// <summary>
    /// 포커스 이동 구현
    /// </summary>
    private void Move()
    {
        Vector2 input = _action.Camera.Move.ReadValue<Vector2>();
        if(input == Vector2.zero) return;
        int moveX = Mathf.RoundToInt(input.x);
        int moveY = -Mathf.RoundToInt(input.y); //필드에서는 좌상단이 0,0이므로 Y는 반전
        Vector2Int clampedPos = FieldManager.ClampToField(focusPos.x + moveX, focusPos.y + moveY);
        focusPos = clampedPos;
        Vector3 worldPos = CoordinateUtility.GetWorldPos(clampedPos);
        focus.position = new Vector3(worldPos.x, focus.position.y, worldPos.z);
    }
    /// <summary>
    /// 회전 입력 처리
    /// </summary>
    /// <param name="ctx"></param>
    private void OnRotateCamera(InputAction.CallbackContext ctx)
    {
        CameraRotate(ctx.ReadValue<float>() < 0f);
    }
    /// <summary>
    /// 카메라 회전 구현
    /// </summary>
    /// <param name="isLeft"></param>
    private void CameraRotate(bool isLeft)
    {
        rotatePos = _action.MouseControl.Position.ReadValue<Vector2>().x;
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
