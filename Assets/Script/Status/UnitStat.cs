using System;
using System.Text;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public class VitalUnitStat: MonoBehaviour, IDamageable
    {
        [Header("기본 정보")]
        [SerializeField] protected UnitData baseData;
        [field: SerializeField]ObjectHealth vitality;
        [field: SerializeField]Stat intensity;
        [field: SerializeField]Stat control;
        [field: SerializeField]Stat speed;
        [field: SerializeField]Stat mobility;
        
        [field: Header("성장 & 레벨 관련")]
        [field:SerializeField] public int Experience{get; private set;} = 0;
        const int maxLevel = 20;
        [field:SerializeField] public int Level{get; private set;} = 1;
        public event Action<int> OnLevelUp;
        [field:SerializeField] public int Sp {get; private set;} = 0;
        [field: SerializeField]Stat boost_sp;
        [field: SerializeField]Stat boost_experience;
        [field: SerializeField]Stat growth_vitality;
        [field: SerializeField]Stat growth_intensity;
        [field: SerializeField]Stat growth_control;
        [field: SerializeField]Stat growth_speed;
        [field: SerializeField]int chance_vitality;
        [field: SerializeField]int chance_intensity;
        [field: SerializeField]int chance_control;
        [field: SerializeField]int chance_speed;

        public virtual void Start()
        {
            vitality = new(baseData.MaxVitality);
            intensity = new(baseData.Intensity);
            control = new(baseData.Control);
            speed = new(baseData.Speed);
            mobility = new(baseData.Mobility);

            growth_vitality = new(baseData.GrowthVitality);
            growth_intensity = new(baseData.GrowthIntensity);
            growth_control = new(baseData.GrowthControl);
            growth_speed = new(baseData.GrowthSpeed);

            boost_experience = new(1);
            boost_sp = new(1);
        }
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                int val = 100;
                Debug.Log($"경험치 {val} 획득");
                GainExperience(val);
            }
        }
        /// <summary>
        /// 피해를 입을시 ObjectHealth로 피해 함수 전가
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage) => vitality.TakeDamage(damage);
        /// <summary>
        /// 레벨 업 함수
        /// </summary>
        public virtual void LevelUp()
        {
            Level++;
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"레벨업 {Level-1} -> {Level}");
            //기본 성장률이 100 이상인 경우 확정 상승
            int base_growth_vitality = growth_vitality.Value;
            int vitality_up = 0;
            stringBuilder.Append($"체력 상승: {vitality.MaxValue.Value} -> ");
            while(base_growth_vitality >= 100)
            {
                ApplyStatByLevelUP(vitality.MaxValue);
                vitality_up ++;
                base_growth_vitality -= 100;
            }
            //여분 성장률 혹은 기본 성장률이 100 미만인 경우 성장률 찬스에 저장
            chance_vitality += base_growth_vitality;
            if (UnityEngine.Random.Range(0, 100) < chance_vitality) //여분 성장률로 성장시 여분 성장률 0으로 초기화 => 40+40+40 = 1+20%(X), 40+40+40 = 1(여분성장률은 추가 성장 불가)
            {
                ApplyStatByLevelUP(vitality.MaxValue);
                vitality_up ++;
                chance_vitality = 0;
            }
            stringBuilder.AppendLine($"{vitality.MaxValue.Value} / {vitality_up}만큼 상승");

            int base_growth_intensity = growth_intensity.Value;
            int intensity_up = 0;
            
            stringBuilder.Append($"강도 상승: {intensity.Value} -> ");
            while(base_growth_intensity >= 100)
            {
                ApplyStatByLevelUP(intensity);
                intensity_up++;
                base_growth_intensity -= 100;
            }
            chance_intensity += base_growth_intensity;
            if (UnityEngine.Random.Range(0, 100) < chance_intensity)
            {
                ApplyStatByLevelUP(intensity);
                intensity_up++;
                chance_intensity = 0;
            }
            stringBuilder.AppendLine($"{intensity.Value} / {intensity_up}만큼 상승");
            
            int base_growth_control = growth_control.Value;
            int control_up = 0;
            stringBuilder.Append($"통제 상승: {control.Value} -> ");
            while(base_growth_control >= 100)
            {
                ApplyStatByLevelUP(control);
                control_up++;
                base_growth_control -= 100;
            }
            chance_control += base_growth_control;
            if (UnityEngine.Random.Range(0, 100) < chance_control)
            {
                ApplyStatByLevelUP(control);
                control_up++;
                chance_control = 0;
            }
            stringBuilder.AppendLine($"{control.Value} / {control_up}만큼 상승");
            
            int base_growth_speed = growth_speed.Value;            
            int speed_up = 0;
            stringBuilder.Append($"속도 상승: {speed.Value} -> ");
            while(base_growth_speed >= 100)
            {
                ApplyStatByLevelUP(speed);
                speed_up ++;
                base_growth_speed -= 100;
            }
            chance_speed += base_growth_speed;
            if (UnityEngine.Random.Range(0, 100) < chance_speed)
            {
                ApplyStatByLevelUP(speed);
                speed_up++;
                chance_speed = 0;
            }
            stringBuilder.AppendLine($"{speed.Value} / {speed_up}만큼 상승");
            OnLevelUp?.Invoke(Level);
            Debug.Log(stringBuilder.ToString());
        }
        /// <summary>
        /// 레벨업 스탯 1 상승 단순화
        /// </summary>
        /// <param name="stat"></param>
        public void ApplyStatByLevelUP(Stat stat) => stat.AddModifier(new StatModifier(1.0f, StatModifierType.Flat, this));
        /// <summary>
        /// 경험치 획득 로직
        /// </summary>
        /// <param name="exp"></param>
        public void GainExperience(int exp)
        {
            //버프 받은 경험치량 계산
            int boostedExp = Mathf.RoundToInt(exp * boost_experience.Value);
            Debug.Log($"획득한 경험치: {boostedExp}");
            //만렙일경우 SP 환산
            if (Level >= maxLevel)
            {
                GainSP(boostedExp);
                return;
            }
            int totalAvailableExp = Experience + boostedExp;
            while (totalAvailableExp >= 100 && Level < maxLevel)
            {
                totalAvailableExp -= 100;
                LevelUp();

                // 만렙에 도달했는지 확인
                if (Level >= maxLevel)
                {
                    // 만렙 도달 시, 남은 모든 경험치를 SP로 환산
                    GainSP(totalAvailableExp);
                    totalAvailableExp = 0; // 경험치 소모 완료
                    break; // 루프 탈출
                }
            }
            Experience = totalAvailableExp;
        }
        /// <summary>
        /// SP 획득 로직
        /// </summary>
        /// <param name="val"></param>
        public void GainSP(int val) => Sp += val * boost_sp.Value;
    }
    public class UnitStat : VitalUnitStat, IDamageable_M
    {
        [Header("기본 정보")]
        [field: SerializeField]ObjectMental mental;
        [Header("성장 & 레벨 관련")]
        [field: SerializeField]Stat growth_mental;
        [field: SerializeField]int chance_mental;
        public override void Start()
        {
            base.Start();
            mental = new(baseData.MaxMental);
            growth_mental = new(baseData.GrowthMental);
        }
        /// <summary>
        /// 피해를 입을시 ObjectMental로 피해 함수 전가
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage_M(int damage) => mental.TakeDamage(damage);
        /// <summary>
        /// 레벨업 함수
        /// </summary>
        public override void LevelUp()
        {
            base.LevelUp();
            StringBuilder stringBuilder = new();
            int base_growth_mental = growth_mental.Value;
            int mental_up = 0;
            stringBuilder.Append($"정신 상승: {mental.MaxValue.Value} -> ");
            while(base_growth_mental >= 100)
            {
                ApplyStatByLevelUP(mental.MaxValue);
                mental_up ++;
                base_growth_mental -= 100;
            }
            chance_mental += base_growth_mental;
            if (UnityEngine.Random.Range(0, 100) < chance_mental)
            {
                ApplyStatByLevelUP(mental.MaxValue);
                mental_up++;
                chance_mental = 0;
            }
            stringBuilder.AppendLine($"{mental.MaxValue.Value} / {mental_up}만큼 상승");
            Debug.Log(stringBuilder.ToString());
        }
    }
}