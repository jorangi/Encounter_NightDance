namespace Encounter.NightDance.Core.Features
{
    public enum Faction
    {
        None,
        Player,
        Enemy,
        Ally
    }
    public interface IFactionFeature : IUnitFeature
    {
        /// <summary>
        /// 팩션 타입 프로퍼티
        /// </summary>
        public Faction _faction {get; set;}
        /// <summary>
        /// 우호적인 팩션인지 확인
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsFriendlyFaction(IFactionFeature other);
        /// <summary>
        /// 적대적인 팩션인지 확인
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsHostileFaction(IFactionFeature other);
        public bool IsPlayable();
    }
}