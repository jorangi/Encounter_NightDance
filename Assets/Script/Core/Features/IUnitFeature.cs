using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Features
{
    /// <summary>
    /// 유닛의 기능적 특성의 최상위 인터페이스
    /// </summary>
    public interface IUnitFeature
    {
        /// <summary>
        /// 유닛에 특성이 등록될 때 호출되는 메서드, 유닛의 코어 인터페이스를 매개변수로 받아 필요한 초기화 작업 수행 가능
        /// </summary>
        /// <param name="owner"></param>
        public void OnRegister(IUnitCore owner);
        /// <summary>
        /// 유닛에 특성이 해제될 때 호출되는 메서드, 유닛의 코어 인터페이스를 매개변수로 받아 필요한 정리 작업 수행 가능
        /// </summary>
        /// <param name="owner"></param>
        public void OnUnregister(IUnitCore owner);
    }
    /// <summary>
    /// 활성화 여부를 위한 추상 클래스, 인터페이스보다 이걸 상속받는걸 권장
    /// </summary>
    public abstract class UnitFeatureBase : IUnitFeature
    {
        public virtual void OnRegister(IUnitCore owner){}
        public virtual void OnUnregister(IUnitCore owner){}
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        public bool IsActive {get; private set;}
    }
    /// <summary>
    /// 생명력 특성을 가진 유닛의 인터페이스
    /// </summary>
    public class VitalityFeature : UnitFeatureBase
    {
        ObjectHealth Vitality {get;}
        Stat Growth_vitality{get;}

        public override void OnRegister(IUnitCore owner)
        {
        }
        public override void OnUnregister(IUnitCore owner)
        {
        }
        /// <summary>
        /// ObjectHealth의 TakeDamage를 호출하는 메서드
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            if(!IsActive) return;
            Vitality.TakeDamage(damage);
        }
        public void LevelUp(int amount)
        {
            Vitality.MaxValue.IncreaseBaseValue(amount);
            
        }
    }
    /// <summary>
    /// 정신력 특성을 가진 유닛의 인터페이스
    /// </summary>
    public interface MentalityFeature : IUnitFeature {ObjectMental ObjectMental{get;}}
    /// <summary>
    /// 장비 관련 특성을 가진 유닛의 인터페이스
    /// </summary>
    public interface IEquipmentFeature : IUnitFeature {
        //TODO: 장비 관련 인터페이스
    }
    //TODO: 이후 기능 추가시 이하에 기재
}