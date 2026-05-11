using System;
using System.Collections.Generic;
using Encounter.NightDance.ScriptableObjects;
using UnityEngine;

namespace Encounter.NightDance.Core.Strategies
{
    public enum MovementType
    {
        Walking,
        Flying,
        Cavalry
    }
    [Serializable]
    public struct MovementTypeData
    {
        public MovementType type;
        public MovementStrategySO strategy;
    }
    public class MovementStrategyContainer : MonoBehaviour
    {
        private static readonly Dictionary<MovementType, MovementStrategySO> strategies = new();
        [SerializeField]private List<MovementTypeData> strategyList;
        public void Awake()
        {
            foreach(MovementTypeData d in strategyList)
            {
                strategies[d.type] = d.strategy;
            }
        }
        public static MovementStrategySO GetStrategySO(MovementType movementType)
        {
            if(strategies.TryGetValue(movementType, out MovementStrategySO strategy))
            {
                return strategy;
            }
            return null;
        }
    }
}