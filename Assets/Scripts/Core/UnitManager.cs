using System.Collections.Generic;
using Encounter.NightDance.Character;
using UnityEngine;

namespace Encounter.NightDance.Core
{
    public class UnitManager
    {
        private readonly List<Unit> players = new();
        private readonly List<Unit> enemies = new();
        private Dictionary<Vector2Int, Unit> unitOnField = new();
        IReadOnlyList<Unit> GetPlayerUnits()
        {
            return players;
        }
        IReadOnlyList<Unit> GetEnemyUnits()
        {
            return enemies;
        }
        Unit GetUnitAt(Vector2Int pos)
        {
            unitOnField.TryGetValue(pos, out Unit unit);
            return unit;
        }
    }
}