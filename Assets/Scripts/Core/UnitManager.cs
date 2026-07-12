using System.Collections.Generic;
using Encounter.NightDance.Character;
using UnityEngine;
using VContainer;
using Encounter.NightDance.Core.Features;

namespace Encounter.NightDance.Core
{
    public class UnitManager
    {
        private readonly List<Unit> _controllableUnits = new();
        private readonly List<Unit> _uncontrollableUnits = new();
        private readonly MultikeyMap<Vector2Int, EntityId, Unit> _unitOnField = new();
        private readonly GameObject _unitPrefab;
        private readonly UnitFactory _unitFactory;
        [Inject]
        public UnitManager(UnitFactory unitFactory, GameObject unitPrefab)
        {
            _unitFactory = unitFactory;
            _unitPrefab = unitPrefab;
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
        public EntityId SpawnUnit(UnitData data, Vector2Int pos)
        {
            Unit unit = _unitFactory.Create(_unitPrefab, data);
            if (unit.GetFeature<IFactionFeature>().IsPlayable())
            {
                _controllableUnits.Add(unit);
            }
            else
            {
                _uncontrollableUnits.Add(unit);
            }
            var id = unit.gameObject.GetEntityId();
            _unitOnField.Add(unit, pos, id);
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
                Object.Destroy(unit.gameObject);
                return true;
            }
            else
            {
                Debug.LogWarning($"해당 ID({id})를 가진 유닛이 없습니다.");
                return false;
            }
        }
    }
}