using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Map;
using Encounter.NightDance.Status;
using Encounter.NightDance.UI;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounter.NightDance.Core
{
    public enum TileState
    {
        Normal,
        Attackable,
        Movable
    }
    public interface ITileSetter
    {
        public void SetTiles(Vector2Int[] v, TileState state);
    }
    public class FieldManager : MonoBehaviour, ITileSetter
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap highlightedTilemap;
        [SerializeField] private TileBase attackableTile;
        [SerializeField] private TileBase movableTile;
        public static Vector2Int FieldSize{private set; get;}
        private readonly Dictionary<Vector2Int, IFieldObject> objectOnField = new();
        private static Dictionary<Vector2Int, ITile> tiles = new();
        private DisposableBag _disposableBag = new();
        private void Awake()
        {
            tilemap = tilemap != null ? tilemap : gameObject.GetComponent<Tilemap>();
            int width = tilemap.cellBounds.xMax - tilemap.cellBounds.xMin;
            int height = tilemap.cellBounds.yMax - tilemap.cellBounds.yMin;
            foreach(Vector3Int _v in tilemap.cellBounds.allPositionsWithin)
            {
                Vector2Int logicalPos = CellToLogical(_v);
                tiles[logicalPos] = new PlainTile(logicalPos, TerrainType.Plain, null);
            }
            FieldSize = new(width, height);
        }
        private void Start()
        {
            FocusUnitService.OnFocusChangedAsObservable()
                .Subscribe(this, (u, state)=>{state.ShowUnitActivated(u);})
                .AddTo(ref _disposableBag);
        }
        /// <summary>
        /// 논리 좌표(0,0 기준) -> 셀 좌표로 변환
        /// </summary>
        public Vector3Int LogicalToCell(Vector2Int logicalPos)
        {
            int cellX = tilemap.cellBounds.xMin + logicalPos.x;
            int cellY = tilemap.cellBounds.yMax - logicalPos.y - 1; 
            return new Vector3Int(cellX, cellY, 0);
        }
        /// <summary>
        /// 셀 좌표 -> 논리 좌표(0,0 기준)로 변환
        /// </summary>
        public Vector2Int CellToLogical(Vector3Int cellPos)
        {
            int logicalX = cellPos.x - tilemap.cellBounds.xMin;
            int logicalY = (tilemap.cellBounds.yMax - 1) - cellPos.y;
            return new Vector2Int(logicalX, logicalY);
        }
        /// <summary>
        /// 논리 좌표 -> 실제 월드 좌표로 변환
        /// </summary>
        public Vector3 GetWorldPos(Vector2Int logicalPos)
        {
            Vector3Int cellPos = LogicalToCell(logicalPos);
            Vector3 worldPos = tilemap.CellToWorld(cellPos);
            
            worldPos.x += 0.5f;
            worldPos.z += 0.5f; 
            
            return worldPos; 
        }
        /// <summary>
        /// 타일 위치에 오브젝트가 있는지 확인하는 함수
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public IFieldObject GetTileObject(Vector2Int pos)
        {
            if(objectOnField.TryGetValue(pos, out IFieldObject objectOnTile))
            {
                return objectOnTile;
            }
            return null;
        }
        /// <summary>
        /// 월드 좌표 -> 논리 좌표로 변환
        /// </summary>
        public Vector2Int GetTilePos(Vector3 worldPos)
        {
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            return CellToLogical(cellPos);
        }
        /// <summary>
        /// 실제 셀 좌표를 오프셋 보정하여 월드 좌표로 변환하는 함수(오브젝트 오프셋 미보정)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2Int GetWorldPosWithoutOffset(Vector2Int pos)
        {
            int offsetX = tilemap.cellBounds.xMin + pos.x;
            int offsetY = tilemap.cellBounds.yMax - pos.y;
            Vector3 tilePos = tilemap.CellToWorld(new Vector3Int(offsetX, offsetY, 0));
            return new Vector2Int((int)tilePos.x, (int)tilePos.z);
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
        public static bool IsWithinField(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < FieldSize.x && pos.y >= 0 && pos.y < FieldSize.y;
        }
        public void SetObjectOnTile(IFieldObject fieldObject, Vector2Int pos)
        {
            if(IsWithinField(pos))
            {
                if(GetTileObject(pos) == null)
                {
                    objectOnField[fieldObject.Pos] = null;
                    objectOnField[pos] = fieldObject;
                    tiles[pos].Occupant = fieldObject;
                }
                else Debug.LogWarning($"[{pos}]이미 해당 위치에 {fieldObject}가 존재합니다.");
            }
        }
        public static ITile GetTile(Vector2Int pos)
        {
            if(tiles.TryGetValue(pos, out ITile tile))
            {
                return tile;
            }
            return null;
        }
        public void SetTiles(Vector2Int[] v, TileState state)
        {
            Vector3Int[] posArray = new Vector3Int[v.Length];
            TileBase[] arr = new TileBase[v.Length];
            TileBase targetTile = state switch
            {
                TileState.Attackable => attackableTile,
                TileState.Movable => movableTile,
                _ => null
            };
            for (int i = 0; i < v.Length; i++)
            {
                posArray[i] = LogicalToCell(v[i]);
                arr[i] = targetTile;
            }
            highlightedTilemap.SetTiles(posArray, arr);
        }
        public void ShowUnitActivated(IUnitCore unitCore)
        {
            if(unitCore == null)return;
            IBaseStats baseStats = unitCore.GetFeature<IBaseStats>();
            IMovable movable = unitCore.GetFeature<WalkingFeature>();
            Dictionary<Vector2Int, int> moveRange = PathFinder.GetMoveRange(unitCore.Pos, baseStats.Mobility.Value, movable._movementStrategy);
            foreach(Vector2Int v in moveRange.Keys)
            {
                SetTiles(new Vector2Int[]{v}, TileState.Movable);
            }
        }
    }
}