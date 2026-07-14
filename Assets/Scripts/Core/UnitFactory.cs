using System;
using System.Collections.Generic;
using Encounter.NightDance.Character;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
using Encounter.NightDance.Core;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Encounter.NightDance.Character
{
    public class UnitFactory : IStartable
    {
        private readonly IObjectResolver _container;
        [Inject]
        public UnitFactory(IObjectResolver container)
        {
            _container = container;
        }
        /// <summary>
        /// 유닛을 프리팹을 사용하여 실제 게임 오브젝트로 생성하여 배치
        /// </summary>
        /// <param name="prefab">생성할 유닛 프리팹</param>
        /// <param name="data">생성할 유닛 데이터</param>
        /// <returns></returns>
        public Unit Create(GameObject prefab, UnitData data)
        {
            if (prefab == null)
            {
                Debug.LogError("[UnitFactory] 생성을 위한 Prefab이 존재하지 않습니다.");
                return null;
            }
            GameObject instance = _container.Instantiate(prefab);
            Unit unit = instance.GetComponent<Unit>();
            instance.name = data.name;
            unit.Initialize(data);
            return unit;
        }

        public void Start(){}
    }
}