using Encounter.NightDance.Status;
using Encounter.NightDance.Core;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Encounter.NightDance.ScriptableObjects;
using Encounter.NightDance.Core.Strategies;

namespace Encounter.NightDance.Character
{
    /// <summary>
    /// 유닛의 행동을 제어하는 컴포넌트 클래스
    /// </summary>
    [RequireComponent(typeof(UnitStat))]
    public class UnitController : Prototype_TileObject, IMovable
    {
        [SerializeField] private MovementStrategySO terrainCostSO;
        private UnitStat stat;
        [SerializeField] private SpriteRenderer t_hpbar;
        private MaterialPropertyBlock mpb;
        private IMovementStrategy movementStrategy;
        private static readonly int FillAmountId = Shader.PropertyToID("_fillAmount");

        private void Awake()
        {
            stat = stat != null ? stat : gameObject.GetComponent<UnitStat>();
            movementStrategy = new WalkingStrategy(terrainCostSO);
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
        private void Start()
        {
            TestFillMaterialAmount(this.GetCancellationTokenOnDestroy()).Forget();
        }
        private async UniTaskVoid TestFillMaterialAmount(CancellationToken ct)
        {
            float currentFill = 1f;
            float lerpSpeed = 5f;
            float threshold = 0.01f;
            while(!ct.IsCancellationRequested)
            {
                float randomFill = LinearCongruentialGenerator.Instance.NextFloat();
                while(Mathf.Abs(currentFill - randomFill) > threshold)
                {
                    if(ct.IsCancellationRequested) return;
                    currentFill = Mathf.Lerp(currentFill, randomFill, lerpSpeed * Time.deltaTime);
                    t_hpbar.GetPropertyBlock(mpb);
                    mpb.SetFloat(FillAmountId, currentFill);
                    t_hpbar.SetPropertyBlock(mpb);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: ct);
                }
                currentFill = randomFill;
                t_hpbar.GetPropertyBlock(mpb);
                mpb.SetFloat(FillAmountId, currentFill);
                t_hpbar.SetPropertyBlock(mpb);
            }
        }
    }
}
