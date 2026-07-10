using VContainer;
using VContainer.Unity;
using Encounter.NightDance.Character;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<UnitFactory>(Lifetime.Singleton);
    }
}