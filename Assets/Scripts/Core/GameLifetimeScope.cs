using VContainer;
using VContainer.Unity;
using Encounter.NightDance.Character;
using Encounter.NightDance.Core;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<UnitFactory>(Lifetime.Singleton);
        builder.Register<CommandInvoker>(Lifetime.Singleton);
    }
}