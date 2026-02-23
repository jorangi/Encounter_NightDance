using Encounter.NightDance.Status;
using Encounter.NightDance.Core;
using UnityEngine;
using System;

namespace Encounter.NightDance.Character
{
    [RequireComponent(typeof(UnitStat))]
    public class UnitController : Prototype_TileObject
    {
        private UnitStat stat;
        private void Start()
        {
            stat = stat != null ? stat : gameObject.GetComponent<UnitStat>();
        }
    }
}
