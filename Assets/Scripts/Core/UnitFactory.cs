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
    public class UnitFactory
    {
        private readonly IObjectResolver _container;
        [Inject]
        public UnitFactory(IObjectResolver container)
        {
            _container = container;
        }
        public Unit Create(GameObject prefab, UnitData data)
        {
            if (prefab == null)
            {
                Debug.LogError("[UnitFactory] 생성을 위한 Prefab이 존재하지 않습니다.");
                return null;
            }
            Unit instance = _container.Instantiate(prefab).GetComponent<Unit>();
            instance.name = data.name;
            instance.Initialize(data);
            return instance;
        }
    }
}