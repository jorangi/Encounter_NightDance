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

namespace Encounter.NightDance.Character
{
    /// <summary>
    /// 유닛의 행동을 제어하는 컴포넌트 클래스
    /// </summary>
    // [RequireComponent(typeof(UnitStat))]
    public class UnitController : Prototype_TileObject, IMovable
    {
        [SerializeField] private MovementStrategySO _terrainCostSO;
        [SerializeField] private UnitHealthView _unitHealthView;
        [SerializeField] private UnitStat _stat;
        [SerializeField] private SpriteRenderer _tHpbar;
        private MaterialPropertyBlock mpb;
        [SerializeField]private IMovementStrategy _movementStrategy;
        private UnitHealthPresenter _unitHealthPresenter;
        private static readonly int FillAmountId = Shader.PropertyToID("_fillAmount");

        private void Awake()
        {
            _stat = _stat != null ? _stat : gameObject.GetComponent<UnitStat>();
            _movementStrategy = new WalkingStrategy(_terrainCostSO);
        }
        private void Start()
        {
            VitalityFeature vitalityFeature = _stat.GetFeature<VitalityFeature>();
            vitalityFeature.Activate();
            _unitHealthPresenter = new UnitHealthPresenter(_unitHealthView, vitalityFeature);

            FocusUnitService.SetFocus(_stat);
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                _stat.GetFeature<VitalityFeature>().TakeDamage(new Core.Datas.DamageData(null, UnityEngine.Random.Range(-101, 100), Core.Datas.DamageType.Vitality, false));
            }
        }
        /// <summary>
        /// 유닛 이동 메서드, transform의 위치를 업데이트
        /// </summary>
        /// <param name="newPos"></param>
        public void MoveTo(Vector2 newPos)
        {
            transform.position = new(newPos.x, transform.position.y, newPos.y);
            mpb = new();
            // TODO: 이동 실행, 애니메이션 등
        }
        private void OnDestroy()
        {
            _unitHealthPresenter.Dispose();    
        }
    }
}
