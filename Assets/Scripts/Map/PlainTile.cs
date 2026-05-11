using Encounter.NightDance.Core;
using UnityEngine;

namespace Encounter.NightDance.Map
{
    public class PlainTile : ITile
    {
        private readonly Vector2Int _pos;
        public Vector2Int Pos => _pos;
        private TerrainType _terrain;
        public TerrainType Terrain => _terrain;
        public IFieldObject Occupant{get;set;}
        public PlainTile(Vector2Int v, TerrainType terrain, IFieldObject occupant = null)
        {
            _pos = v;
            _terrain = terrain;
            Occupant = occupant;
        }
    }
}