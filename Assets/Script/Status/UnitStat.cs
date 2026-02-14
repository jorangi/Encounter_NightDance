using UnityEngine;

namespace Encounter.NightDance.Status
{
    public class UnitStat : MonoBehaviour
    {
        [SerializeField] UnitData baseData;
        ObjectHealth vitality;
        ObjectMental mental;
        Stat intensity;
        Stat control;
        Stat speed;
        Stat mobility;
    }
}