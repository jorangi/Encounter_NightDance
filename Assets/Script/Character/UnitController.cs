using Encounter.NightDance.Status;
using UnityEngine;
using System;

namespace Encounter.NightDance.Character
{
    [RequireComponent(typeof(UnitStat))]
    public class UnitController : MonoBehaviour
    {
        private UnitStat stat;
        private void Awake()
        {
            stat = stat != null ? stat : gameObject.AddComponent<UnitStat>();
        }
        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation; // 카메라의 회전에 맞춰 유닛 회전
        }
    }
}
