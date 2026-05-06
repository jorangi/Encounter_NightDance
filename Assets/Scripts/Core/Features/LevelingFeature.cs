using System;
using System.Text;
using Encounter.NightDance.Status;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Core.Features
{
    public class LevelingFeature : UnitFeatureBase
    {
        public Percentage Experience{get; private set;} = new(0);
        private ReactiveProperty<Percentage> _onExperienceChangedSubject = new();
        public Observable<Percentage> OnExperienceChangedAsObservable() => _onExperienceChangedSubject;
        public int Level {get; private set;} = 1;
        private ReactiveProperty<int> _onLevelChangeSubject = new();
        public Observable<int> OnLevelChangeAsObservable() => _onLevelChangeSubject;
        const int maxLevel = 20;
        public int SP {get; private set;} = 0;
        private Stat _boost_experience;
        private Stat _boost_sp;
        private IUnitCore _owner;
        public LevelingFeature(IUnitCore owner)
        {
            _owner = owner;
            _boost_experience = new Stat(1);
            _boost_sp = new Stat(1);
        }
        public void GainExperience(int exp)
        {
            //버프 받은 경험치량 계산
            Percentage boostedExp = new Percentage(Mathf.RoundToInt(exp * _boost_experience.Value));
            Debug.Log($"획득한 경험치: {boostedExp}");
            //만렙일경우 SP 환산
            if (Level >= maxLevel)
            {
                GainSP(boostedExp);
                return;
            }
            Percentage totalAvailableExp = Experience + boostedExp;
            while (totalAvailableExp >= 100 && Level < maxLevel)
            {
                totalAvailableExp -= 100;
                LevelUp();

                // 만렙에 도달했는지 확인
                if (Level >= maxLevel)
                {
                    // 만렙 도달 시, 남은 모든 경험치를 SP로 환산
                    GainSP(totalAvailableExp);
                    totalAvailableExp = new(0); // 경험치 소모 완료
                    break; // 루프 탈출
                }
            }
            Experience = totalAvailableExp;
            _onExperienceChangedSubject.OnNext(Experience);
        }
        /// <summary>
        /// SP 획득 로직
        /// </summary>
        /// <param name="val"></param>
        public void GainSP(int val) => SP += val * _boost_sp.Value;
        public virtual void LevelUp()
        {
            Level++;
            IGrowableFeature[] growables = _owner.GetFeatures<IGrowableFeature>();
            foreach (IGrowableFeature growable in growables)
            {
                growable.ApplyGrowthOnLevelUp(Level);
            }
            _onLevelChangeSubject.OnNext(Level);
        }
    }
}