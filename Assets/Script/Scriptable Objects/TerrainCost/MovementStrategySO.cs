using System;
using System.Collections.Generic;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.Map;
using UnityEngine;

namespace Encounter.NightDance.ScriptableObjects
{
    [Serializable]
    public struct TerrainCostData
    {
        public TerrainType terrainType;
        public int cost;
    }
    [Serializable]
    [CreateAssetMenu(fileName = "TerrainCost", menuName = "Scriptable Objects/TerrainCost")]
    public class MovementStrategySO : ScriptableObject
    {
        public List<TerrainCostData> costSettings = new();
        private Dictionary<TerrainType, int> _costMap = new();
        public void Initialize()
        {
            _costMap.Clear();
            foreach(TerrainCostData setting in costSettings)
            {
                _costMap[setting.terrainType] = setting.cost;
            }
        }
        public int GetCost(TerrainType terrainType)
        {
            if(_costMap != null && _costMap.TryGetValue(terrainType, out int cost)) return cost;
            return 999;
        }
    }
}