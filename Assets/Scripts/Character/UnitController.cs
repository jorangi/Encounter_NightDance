using Encounter.NightDance.Status;
using Encounter.NightDance.Core;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Encounter.NightDance.ScriptableObjects;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.UI.Presenter;
using Encounter.NightDance.UI.View;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI;
using System.Text;
using Encounter.NightDance.Map;
using System.Collections.Generic;

namespace Encounter.NightDance.Character
{
    /// <summary>
    /// 유닛의 행동을 제어하는 컴포넌트 클래스
    /// </summary>
    // [RequireComponent(typeof(UnitStat))]
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitStat _stat;
        private MaterialPropertyBlock mpb;
        private static readonly int FillAmountId = Shader.PropertyToID("_fillAmount");
        public override string ToString() => _stat == null ? "null" : _stat.name;
        /// <summary>
        /// 유닛 이동 메서드, transform의 위치를 업데이트
        /// </summary>
        /// <param name="newPos"></param>
        public void MoveTo(Vector3 newPos)
        {
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
            mpb = new();
            // TODO: 이동 실행, 애니메이션 등
        }
    }
}
