using VContainer;
using VContainer.Unity;
using Encounter.NightDance.Character;
using Encounter.NightDance.Core;
using System;
using UnityEngine;
using MessagePipe;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Commands;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private FieldManager fieldManager;
    protected override void Configure(IContainerBuilder builder)
    {
        var options = builder.RegisterMessagePipe();
        builder.RegisterMessageBroker<EventContext>(options);
        builder.RegisterInstance(fieldManager);
        builder.RegisterInstance(unitPrefab);
        builder.RegisterEntryPoint<UnitManager>().AsSelf();
        builder.Register<UnitFactory>(Lifetime.Singleton).AsSelf();
        builder.Register<BattleCommandFactory>(Lifetime.Singleton).AsSelf();
        builder.Register<UnitCommandFactory>(Lifetime.Singleton).AsSelf();
        builder.Register<CommandInvoker>(Lifetime.Singleton).AsSelf();
        builder.RegisterComponentInHierarchy<Prototype_GameManager>();
    }
}