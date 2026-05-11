using Encounter.NightDance.Core;
using UnityEngine;

namespace Encounter.NightDance.Map
{
    
    public enum TerrainType
    {
        Plain,
        Forest,
        Mountain,
        Water
    }
    public interface ITile
    {
        Vector2Int Pos {get;}
        TerrainType Terrain {get;}
        IFieldObject Occupant {get; set;}
    }
}