using UnityEngine;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 필드 상의 오브젝트를 나타내는 컴포넌트 클래스
    /// </summary>
    public class Prototype_TileObject : MonoBehaviour, IFieldObject
    {
        private Vector2Int _pos;
        public Vector2Int Pos 
        { 
            get=>_pos;
            set {
                _pos = new Vector2Int(
                Mathf.Clamp(value.x, 0, FieldManager.fieldSize.x - 1), 
                Mathf.Clamp(value.y, 0, FieldManager.fieldSize.y -1));
            }
        }
        private Vector2 worldPos;
        public Vector2 WorldPos { 
            get => worldPos;
            set
            {
                worldPos = value;
                transform.position = new Vector3(worldPos.x, 0, worldPos.y);
            }
        }
    }
}