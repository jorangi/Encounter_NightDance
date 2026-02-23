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
        Stat growth_vitality;
        Stat growth_mental;
        Stat growth_intensity;
        Stat growth_control;
        Stat growth_speed;

        private void Start()
        {
            vitality = new(baseData.MaxVitality);
            mental = new(baseData.MaxMental);
            intensity = new(baseData.Intensity);
            control = new(baseData.Control);
            speed = new(baseData.Speed);
            mobility = new(baseData.Mobility);

            growth_vitality = new(baseData.GrowthVitality);
            growth_mental = new(baseData.GrowthMental);
            growth_intensity = new(baseData.GrowthIntensity);
            growth_control = new(baseData.GrowthControl);
            growth_speed = new(baseData.GrowthSpeed);
        }
    }
}