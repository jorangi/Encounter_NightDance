using Encounter.NightDance.Character;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Core.Commands
{
    public class UnitSpawnCommand : CommandBase
    {
        private readonly UnitData _data;
        private readonly Vector2Int _pos;
        private readonly ReactiveProperty<Unit> _spawnedUnit;
        private readonly UnitFactory _unitFactory;
        private readonly GameObject _unitPrefab;
        /// <summary>
        /// 유닛 생성 커맨드
        /// </summary>
        /// <param name="data">유닛 데이터</param>
        /// <param name="pos">유닛 위치</param>
        /// <param name="spawnedUnit">유닛이 생성된 후 할당될 ReactiveProperty</param>
        /// <param name="unitFactory">유닛 생성 팩토리</param>
        /// <param name="unitPrefab">유닛 프리팹</param>
        public UnitSpawnCommand(UnitData data, Vector2Int pos, ReactiveProperty<Unit> spawnedUnit, UnitFactory unitFactory, GameObject unitPrefab)
        {
            _data = data;
            _pos = pos;
            _spawnedUnit = spawnedUnit;
            _unitFactory = unitFactory;
            _unitPrefab = unitPrefab;
        }
        public override bool CanExecute()
        {
            if(_data == null) return false;
            if(_unitFactory == null) return false;
            if(_unitPrefab == null) return false;
            if(_spawnedUnit == null) return false;
            return true;
        }
        public override void Execute()
        {
            Unit unit = _unitFactory.Create(_unitPrefab, _data);
            _spawnedUnit.Value = unit;
        }
        public override void Redo() => Execute();
    }
}