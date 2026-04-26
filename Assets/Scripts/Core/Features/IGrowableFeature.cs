using System.Text;

namespace Encounter.NightDance.Core.Features
{
    public interface IGrowableFeature : IUnitFeature
    {
        public void ApplyGrowthOnLevelUp(int currentLevel);
    }
}