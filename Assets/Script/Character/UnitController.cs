using Encounter.NightDance.Status;
using UnityEngine;
using System;

namespace Encounter.NightDance.Character
{
    [RequireComponent(typeof(UnitStat))]
    public class UnitController : MonoBehaviour
    {
        private UnitStat stat;
        private void Start()
        {
            stat = stat != null ? stat : gameObject.GetComponent<UnitStat>();
        }
    }
}
