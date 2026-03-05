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
        public static Vector2Int fieldSize{private set; get;}
        private Dictionary<Vector2Int, IFieldObject> objectOnField = new();
        private void Start()
        {
            tilemap = tilemap != null ? tilemap : gameObject.GetComponent<Tilemap>();
            int width = tilemap.cellBounds.xMax - tilemap.cellBounds.xMin;
            int height = tilemap.cellBounds.yMax - tilemap.cellBounds.yMin;
            fieldSize = new(width, height);
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
        public Vector2Int GetTilePos(Vector2Int pos)
        {
            int offsetX = tilemap.cellBounds.xMin + pos.x;
            int offsetY = tilemap.cellBounds.yMax - pos.y;
            Vector3 tilePos = tilemap.CellToWorld(new Vector3Int(offsetX, offsetY, 0));
            return new Vector2Int((int)tilePos.x, (int)tilePos.z);
        }
        public void SetUnitPos(UnitController unit, Vector2Int pos)
        {
            Vector2Int tilePos = GetTilePos(pos);
            if(CheckUnitOnTile(tilePos) == null)
            {
                unit.Pos = tilePos;
                objectOnField[tilePos] = unit;
            }
        }
    }
}