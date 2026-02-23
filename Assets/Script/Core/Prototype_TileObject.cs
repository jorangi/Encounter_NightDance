using UnityEngine;

namespace Encounter.NightDance.Core
{
    public class Prototype_TileObject : MonoBehaviour, IFieldObject
    {
        private Vector2Int _pos;
        public Vector2Int Pos 
        { 
            get=>_pos;
            set => new Vector2Int(
                Mathf.Clamp(value.x, 0, FieldManager.fieldSize.x - 1), 
                Mathf.Clamp(value.y, 0, FieldManager.fieldSize.y -1));
        }

        void Start()
        {
            
        }
        void Update()
        {
            
        }
    }    
}