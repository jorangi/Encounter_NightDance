using Encounter.NightDance.Core;
using UnityEngine;

namespace Encounter.NightDance.Character
{
    public class UnitController : MonoBehaviour
    {
        private ObjectHealth health;
        
        private delegate void DamageOn();
        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation; // 카메라의 회전에 맞춰 유닛 회전
        }
    }
}
