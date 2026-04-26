using UnityEngine;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 이동 가능한 오브젝트 인터페이스
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// 인터페이스 이동 메서드, 구현체에서 이동 로직과 애니메이션 등을 처리
        /// </summary>
        /// <param name="newPos"></param>
        void MoveTo(Vector2 newPos);
    }
}