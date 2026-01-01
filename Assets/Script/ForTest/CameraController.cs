using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CameraRot:sbyte
{
    Left = -1,
    Center = 0,
    Right = 1
}
public class CameraController : MonoBehaviour
{
    
    CinemachineCamera cam;
    CinemachineThirdPersonFollow follow;
    const float ZoomSpeed = 15f;
    const float ROTOFFSET = 3f;
    const float ARMMAX = 10f;
    const float ARMMIN = -6f;
    [SerializeField]Transform target;
    [SerializeField]Grid grid;
    [SerializeField]Tilemap tilemap;
    CameraRot rot = CameraRot.Center;
    bool rotDirty = false;
    float rotatePos;
    float zoomPos;
    Vector2 movePos;
    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        follow = GetComponent<CinemachineThirdPersonFollow>();
        Debug.Log($"tilemap Size: {tilemap.size}");
        Debug.Log($"tilemap Cell Bounds Size: {tilemap.cellBounds.size}");
        Debug.Log($"tilemap Cell Bounds: {tilemap.cellBounds.min} to {tilemap.cellBounds.max}");
    }
    void Update()
    {
        //마우스 우클릭 트리거 -> 회전, 줌인/아웃
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Clicked");
            rotatePos = Input.mousePosition.x;
            zoomPos = Input.mousePosition.y;
        }
        //마우스 휠클릭 트리거 -> 이동
        if(Input.GetMouseButtonDown(2))
        {
            movePos = Input.mousePosition;
        }
        float rotateDelta = Input.GetMouseButton(1) ? Input.mousePosition.x - rotatePos : 0.0f; // 우클릭 X 이동량
        float zoomDelta = Input.GetMouseButton(1) ? Input.mousePosition.y - zoomPos : 0.0f; // 우클릭 Y 이동량
        Vector2 moveDelta = Input.GetMouseButton(2) ? (Vector2)Input.mousePosition - movePos : Vector2.zero; //휠클릭 이동량
        //마우스 조작 회전
        if(Mathf.Abs(rotateDelta) > Screen.width * 0.3f)
        {
            CameraRotate(rotateDelta < 0);
        }
        //마우스 조작 줌인/줌아웃
        if(Mathf.Abs(zoomDelta) > Screen.height * 0.1f)
        {
            CameraZoom(zoomDelta > 0);
        }
        if(!Mathf.Approximately(Input.mouseScrollDelta.y, 0.0f))
        {
            CameraZoom(Input.mouseScrollDelta.y > 0);
        }
        //마우스 조작 이동
        if(!Mathf.Approximately(moveDelta.magnitude, 0.0f))
        {
            Vector3 pickPos = new(Mathf.Clamp(target.position.x - moveDelta.x * 0.01f, tilemap.cellBounds.min.x, tilemap.cellBounds.max.x - 1),
                                  target.position.y,
                                  Mathf.Clamp(target.position.z - moveDelta.y * 0.01f, tilemap.cellBounds.min.y, tilemap.cellBounds.max.y - 1));
            bool hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
            if(hasTile)
                target.position = pickPos;
            else
            {
                Debug.Log("No Tile Ahead");
            }
        }
        //키보드 이동 조작
        if(Input.GetKeyDown(KeyCode.W)) //상향
        {
            Vector3 pickPos = new(target.position.x, target.position.y, Mathf.Clamp(target.position.z + 1, tilemap.cellBounds.min.y, tilemap.cellBounds.max.y - 1));
            bool hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
            if(hasTile)
                target.position = new(target.position.x, target.position.y, Mathf.Clamp(target.position.z + 1, tilemap.cellBounds.min.y, tilemap.cellBounds.max.y - 1));
            else
            {
                while(!hasTile && pickPos.z < tilemap.cellBounds.max.y - 1)
                {
                    pickPos.z += 1;
                    hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
                }
                if(hasTile)target.position = pickPos;
                else Debug.Log("No Tile Ahead");
            }
        }
        if(Input.GetKeyDown(KeyCode.A)) //좌향
        {
            Vector3 pickPos = new(Mathf.Clamp(target.position.x - 1, tilemap.cellBounds.min.x, tilemap.cellBounds.max.x - 1), target.position.y, target.position.z);
            bool hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
            if(hasTile)
                target.position = new(Mathf.Clamp(target.position.x - 1, tilemap.cellBounds.min.x, tilemap.cellBounds.max.x - 1), target.position.y, target.position.z);
            else
            {
                while(!hasTile && pickPos.x > tilemap.cellBounds.min.x)
                {
                    pickPos.x -= 1;
                    hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
                }
                if(hasTile)target.position = pickPos;
                else Debug.Log("No Tile Ahead");
            }
        }
        if(Input.GetKeyDown(KeyCode.S)) //하향
        {
            Vector3 pickPos = new(target.position.x, target.position.y, Mathf.Clamp(target.position.z - 1, tilemap.cellBounds.min.y, tilemap.cellBounds.max.y - 1));
            bool hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
            if(hasTile)
                target.position = new(target.position.x, target.position.y, Mathf.Clamp(target.position.z - 1, tilemap.cellBounds.min.y, tilemap.cellBounds.max.y - 1));
            else
            {
                while(!hasTile && pickPos.z > tilemap.cellBounds.min.y)
                {
                    pickPos.z -= 1;
                    hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
                }
                if(hasTile)target.position = pickPos;
                else Debug.Log($"{target.position}에 타일이 존재하지 않아 취소됨.");
            }
        }
        if(Input.GetKeyDown(KeyCode.D)) //우향
        {
            Vector3 pickPos = new(Mathf.Clamp(target.position.x + 1, tilemap.cellBounds.min.x, tilemap.cellBounds.max.x - 1), target.position.y, target.position.z);
            bool hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
            if(hasTile)
                target.position = new(Mathf.Clamp(target.position.x + 1, tilemap.cellBounds.min.x, tilemap.cellBounds.max.x - 1), target.position.y, target.position.z);
            else
            {
                while(!hasTile && pickPos.x < tilemap.cellBounds.max.x - 1)
                {
                    pickPos.x += 1;
                    hasTile = tilemap.HasTile(grid.WorldToCell(pickPos));
                }
                if(hasTile)target.position = pickPos;
                else Debug.Log("No Tile Ahead");
            }
        }
        //키보드 회전
        if(Input.GetKeyDown(KeyCode.Q)) //좌회전
        {
            CameraRotate(true);
        }
        if(Input.GetKeyDown(KeyCode.E)) //우회전
        {
            CameraRotate(false);
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
    private void CameraRotate(bool isLeft)
    {
        rotatePos = Input.mousePosition.x;
        rot = isLeft ? (CameraRot)Mathf.Max(-1, (int)rot - 1) : (CameraRot)Mathf.Min(1, (int)rot + 1);
        rotDirty = true;
    }
    private void CameraZoom(bool isIn)
    {
        float zoomStep = isIn ? -ZoomSpeed : ZoomSpeed;
        follow.VerticalArmLength = Mathf.Clamp(follow.VerticalArmLength + zoomStep * Time.deltaTime, ARMMIN, ARMMAX);
    }
}
