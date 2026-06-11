using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounter.NightDance.Core
{
    public static class CoordinateUtility
    {
        private static Tilemap tilemap;
        private static BoundsInt bounds;
        public static void Initialize(Tilemap _tilemap)
        {
            tilemap = _tilemap;
            bounds = tilemap.cellBounds;
        }
        /// <summary>
        /// 논리 좌표(0,0 기준) -> 셀 좌표로 변환
        /// </summary>
        public static Vector3Int LogicalToCell(Vector2Int logicalPos)
        {
            int cellX = bounds.xMin + logicalPos.x;
            int cellY = bounds.yMax - logicalPos.y - 1; 
            return new Vector3Int(cellX, cellY, 0);
        }
        /// <summary>
        /// 셀 좌표 -> 논리 좌표(0,0 기준)로 변환
        /// </summary>
        public static Vector2Int CellToLogical(Vector3Int cellPos)
        {
            int logicalX = cellPos.x - bounds.xMin;
            int logicalY = (bounds.yMax - 1) - cellPos.y;
            return new Vector2Int(logicalX, logicalY);
        }
        
        /// <summary>
        /// 논리 좌표 -> 실제 월드 좌표로 변환
        /// </summary>
        public static Vector3 GetWorldPos(Vector2Int logicalPos)
        {
            Vector3Int cellPos = LogicalToCell(logicalPos);
            Vector3 worldPos = tilemap.CellToWorld(cellPos);
            
            worldPos.x += 0.5f;
            worldPos.z += 0.5f; 
            
            return worldPos; 
        }
    }
}