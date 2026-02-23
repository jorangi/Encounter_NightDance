using UnityEngine;

namespace Encounter.NightDance.Status
{
    /// <summary>
    /// 피해를 입을 수 있는 오브젝트 인터페이스
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// vitality 피해를 입는 함수
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage);
    }
    /// <summary>
    /// 정신 피해를 입을 수 있는 오브젝트 인터페이스
    /// </summary>
    public interface IDamageable_M
    {
        /// <summary>
        /// mental 피해를 입는 함수
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage_M(int damage);
    }
}
