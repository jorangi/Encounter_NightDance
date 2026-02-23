using System.Collections.Generic;
using System.Linq;
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
        private IFieldObject CheckTile(Vector2Int pos)
        {
            if(objectOnField.TryGetValue(pos, out IFieldObject objectOnTile))
            {
                return objectOnTile;
            }
            return null;
        }
    }
}