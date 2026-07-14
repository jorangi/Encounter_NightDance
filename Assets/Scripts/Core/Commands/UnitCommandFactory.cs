using System.Collections.Generic;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Commands;
using MessagePipe;
using UnityEngine;
using VContainer;
using R3;
using Encounter.NightDance.Character;
using VContainer.Unity;

namespace Encounter.NightDance.Core.Commands
{
    public class UnitCommandFactory: IStartable
    {
        private readonly UnitFactory _unitFactory;
        private readonly GameObject _unitPrefab;
        [Inject]
        public UnitCommandFactory(UnitFactory unitFactory, GameObject unitPrefab)
        {
            _unitFactory = unitFactory;
            _unitPrefab = unitPrefab;
        }
        public UnitSpawnCommand CreateUnitSpawnCommand(UnitData data, Vector2Int pos, ReactiveProperty<Unit> unit) =>new(data, pos, unit, _unitFactory, _unitPrefab);
        public void Start(){}
    }
}