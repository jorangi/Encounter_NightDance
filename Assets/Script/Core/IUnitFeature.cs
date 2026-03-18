using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Feafures
{
    /// <summary>
    /// 유닛의 기능적 특성의 최상위 인터페이스
    /// </summary>
    public interface IUnitFeature{}
    /// <summary>
    /// 생명력 특성을 가진 유닛의 인터페이스
    /// </summary>
    public interface IVitalityFeature : IUnitFeature {ObjectHealth Vitality {get;}}
    /// <summary>
    /// 정신력 특성을 가진 유닛의 인터페이스
    /// </summary>
    public interface IMentalityFeature : IUnitFeature {ObjectMental ObjectMental{get;}}
    /// <summary>
    /// 장비 관련 특성을 가진 유닛의 인터페이스
    /// </summary>
    public interface IEquipmentFeature : IUnitFeature {
        //TODO: 장비 관련 인터페이스
    }
    //TODO: 이후 기능 추가시 이하에 기재
}