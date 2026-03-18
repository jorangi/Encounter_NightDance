using Encounter.NightDance.Core.Feafures;

namespace Encounter.NightDance.Status
{
    public interface IUnitStat
    {
        T GetFeature<T>() where T : class, IUnitFeature;
    }
}