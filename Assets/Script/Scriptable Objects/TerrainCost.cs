using System;
using System.Collections.Generic;
using Encounter.NightDance.Map;
using UnityEngine;

namespace Encounter.NightDance.ScriptableObjects
{
    [Serializable]
    public struct TerrainCostData
    {
        public TerrainType terrainType;
        public float cost;
    }
    [CreateAssetMenu(fileName = "TerrainCost", menuName = "ScriptableObjects/TerrainCost")]
    public class TerrainCost : ScriptableObject
    {
        public List<TerrainCostData> costSettings = new();
        private Dictionary<TerrainType, float> _costMap = new();
        public void Initialize()
        {
            _costMap.Clear();
            foreach(TerrainCostData setting in costSettings)
            {
                _costMap[setting.terrainType] = setting.cost;
            }
        }
        public float GetCost(TerrainType terrainType)
        {
            if(_costMap != null && _costMap.TryGetValue(terrainType, out float cost)) return cost;
            return 999f;
        }
    }
}