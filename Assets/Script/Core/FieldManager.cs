using System;
using System.Collections.Generic;
using System.Linq;
using Encounter.NightDance.Character;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounter.NightDance.Core
{
    public class FieldManager : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        public static Vector2Int FieldSize{private set; get;}
        private readonly Dictionary<Vector2Int, IFieldObject> objectOnField = new();
        private void Start()
        {
            tilemap = tilemap != null ? tilemap : gameObject.GetComponent<Tilemap>();
            int width = tilemap.cellBounds.xMax - tilemap.cellBounds.xMin;
            int height = tilemap.cellBounds.yMax - tilemap.cellBounds.yMin;
            FieldSize = new(width, height);
        }
        /// <summary>
        /// 타일 위치에 오브젝트가 있는지 확인하는 함수
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private IFieldObject CheckUnitOnTile(Vector2Int pos)
        {
            if(objectOnField.TryGetValue(pos, out IFieldObject objectOnTile))
            {
                return objectOnTile;
            }
            return null;
        }
        /// <summary>
        /// 보정된 셀 좌표를 입력하여 월드 좌표로 변환하는 함수(오브젝트 오프셋 보정)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 GetTilePos(Vector2Int pos)
        {
            Vector2 result = GetTilePosWithoutOffset(pos);
            result.x += 0.5f;
            result.y -= 0.5f;
            return result;
        }
        /// <summary>
        /// 실제 셀 좌표를 오프셋 보정하여 월드 좌표로 변환하는 함수(오브젝트 오프셋 미보정)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2Int GetTilePosWithoutOffset(Vector2Int pos)
        {
            int offsetX = tilemap.cellBounds.xMin + pos.x;
            int offsetY = tilemap.cellBounds.yMax - pos.y;
            Vector3 tilePos = tilemap.CellToWorld(new Vector3Int(offsetX, offsetY, 0));
            return new Vector2Int((int)tilePos.x, (int)tilePos.z);
        }
        [Obsolete("이 함수는 MoveCommand로 대체되었습니다.")]
        /// <summary>
        /// 유닛의 위치를 설정하는 함수, 타일에 이미 오브젝트가 존재하는 경우 위치 변경 불가
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="pos"></param>
        public void SetUnitPos(UnitController unit, Vector2Int pos)
        {
            if(CheckUnitOnTile(pos) != null)
            {
                Debug.Log("이미 오브젝트가 존재하는 타일입니다.");
                return;
            }
            Vector2Int tilePos = GetTilePosWithoutOffset(pos);
            unit.SetPos(pos, tilePos);
            objectOnField[tilePos] = unit;
        }
        public static Vector2Int ClampToField(int x, int y)
        {
            int clampedX = Mathf.Clamp(x, 0, FieldSize.x - 1);
            int clampedY = Mathf.Clamp(y, 0, FieldSize.y - 1);
            return new Vector2Int(clampedX, clampedY);
        }
        public Vector2Int FocusOffset(Vector2Int v)
        {
            int offsetX = Mathf.Abs(Mathf.Abs(tilemap.cellBounds.xMin + 1) + v.x);
            int offsetY = Mathf.Abs(v.y - Mathf.Abs(tilemap.cellBounds.yMax - 1));
            return new(offsetX, offsetY);
        }
    }
}