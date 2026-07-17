using System.Collections.Generic;
using Encounter.NightDance.Character;
using UnityEngine;
using VContainer;
using Encounter.NightDance.Core.Features;
using System;
using MessagePipe;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Event.Payload;
using Encounter.NightDance.Core.Commands;
using R3;
using VContainer.Unity;

namespace Encounter.NightDance.Core
{
    public class UnitManager : IDisposable, IStartable
    {
        private readonly IDisposable _subscription;
        private readonly List<Unit> _controllableUnits = new();
        private readonly List<Unit> _uncontrollableUnits = new();
        private readonly MultikeyMap<Vector2Int, EntityId, Unit> _unitOnField = new();
        private readonly GameObject _unitPrefab;
        private readonly UnitFactory _unitFactory;
        private readonly CommandInvoker _commandInvoker;
        private readonly BattleCommandFactory _battleCommandFactory;
        private readonly UnitCommandFactory _unitCommandFactory;
        [Inject]
        public UnitManager(UnitFactory unitFactory, GameObject unitPrefab, ISubscriber<EventContext> eventSubscriber, CommandInvoker commandInvoker, BattleCommandFactory battleCommandFactory, UnitCommandFactory unitCommandFactory)
        {
            _unitFactory = unitFactory;
            _unitPrefab = unitPrefab;
            _commandInvoker = commandInvoker;
            _battleCommandFactory = battleCommandFactory;
            _unitCommandFactory = unitCommandFactory;
            _subscription = eventSubscriber.Subscribe(context =>
            {
                if(context.payload is UnitSpawnPayload spawnPayload)
                {
                    ReactiveProperty<Unit> unit = new();
                    SpawnUnit(spawnPayload.data, spawnPayload.position, unit);
                }
            });
        }
        /// <summary>
        /// 조종가능한 유닛들을 반환
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Unit> GetControllableUnits()
        {
            return _controllableUnits;
        }
        /// <summary>
        /// 조종불가능한 유닛들을 반환
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Unit> GetUncontrollableUnits()
        {
            return _uncontrollableUnits;
        }
        /// <summary>
        /// 유닛 ID를 통해 유닛을 반환
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public Unit GetUnit(EntityId entityId)
        {
            if (_unitOnField.TryGetValue(entityId, out Unit unit))
            {
                return unit;
            }
            Debug.LogWarning($"해당 ID({entityId})를 가진 유닛이 없습니다.");
            return null;
        }
        /// <summary>
        /// 유닛의 위치를 통해 유닛을 반환
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Unit GetUnit(Vector2Int pos)
        {
            if (_unitOnField.TryGetValue(pos, out Unit unit))
            {
                return unit;
            }
            Debug.LogWarning($"해당 위치({pos})에 유닛이 없습니다.");
            return null;
        }
        /// <summary>
        /// 유닛을 필드에 생성
        /// </summary>
        /// <param name="data">생성할 유닛 데이터</param>
        /// <param name="pos">생성할 유닛 위치</param>
        public EntityId SpawnUnit(UnitData data, Vector2Int pos, ReactiveProperty<Unit> spawnedUnit)
        {
            var spawnUnitCommand = _unitCommandFactory.CreateUnitSpawnCommand(data, pos, spawnedUnit);
            _commandInvoker.ExecuteCommand(spawnUnitCommand);
            // TODO: IFactionFeature 완성하고 풀것
            // if (spawnedUnit.Value.GetFeature<IFactionFeature>().IsPlayable())
            // {
            //     _controllableUnits.Add(spawnedUnit.Value);
            // }
            // else
            // {
            //     _uncontrollableUnits.Add(spawnedUnit.Value);
            // }
            var id = spawnedUnit.Value.gameObject.GetEntityId();
            var placeUnitCommand = _battleCommandFactory.CreateMoveCommand(spawnedUnit.Value, pos);
            _commandInvoker.ExecuteCommand(placeUnitCommand);
            _unitOnField.Add(spawnedUnit.Value, pos, id);
            return id;
        }
        /// <summary>
        /// 유닛을 필드에서 제거
        /// </summary>
        /// <param name="id">제거할 유닛 ID</param>
        public bool DespawnUnit(EntityId id)
        {
            if (_unitOnField.TryGetValue(id, out Unit unit))
            {
                _unitOnField.Remove(id);
                _controllableUnits.Remove(unit);
                _uncontrollableUnits.Remove(unit);
                UnityEngine.Object.Destroy(unit.gameObject);
                return true;
            }
            else
            {
                Debug.LogWarning($"해당 ID({id})를 가진 유닛이 없습니다.");
                return false;
            }
        }
        public void Dispose() => _subscription.Dispose();

        public void Start(){}
    }
}