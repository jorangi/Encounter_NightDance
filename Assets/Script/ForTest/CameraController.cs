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
    const float ZoomSpeed = 10f;
    const float ROTOFFSET = 3f;
    const float ARMMAX = 10f;
    const float ARMMIN = 0f;
    [SerializeField]Transform target;
    [SerializeField]Grid grid;
    [SerializeField]Tilemap tilemap;
    CameraRot rot = CameraRot.Center;
    bool rotDirty = false;
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
        if(Input.GetKeyDown(KeyCode.W))
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
        if(Input.GetKeyDown(KeyCode.A))
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
        if(Input.GetKeyDown(KeyCode.S))
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
        if(Input.GetKeyDown(KeyCode.D))
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
        if(Input.GetKeyDown(KeyCode.Q))
        {
            rot = (CameraRot)Mathf.Max(-1, (int)rot - 1);
            rotDirty = true;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            rot = (CameraRot)Mathf.Min(1, (int)rot + 1);
            rotDirty = true;
        }
        if(Input.GetKey(KeyCode.R))
        {
            follow.VerticalArmLength = Mathf.Clamp(follow.VerticalArmLength - ZoomSpeed * Time.deltaTime, ARMMIN, ARMMAX);
        }
        if(Input.GetKey(KeyCode.F))
        {
            follow.VerticalArmLength = Mathf.Clamp(follow.VerticalArmLength + ZoomSpeed * Time.deltaTime, ARMMIN, ARMMAX);
        }
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
}
