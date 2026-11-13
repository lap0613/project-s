using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Environment;
using Whiteout.Environment.Unity;
using Whiteout.Input;
using Whiteout.Player.Unity;
using Whiteout.RunFlow;
using Whiteout.RunFlow.Unity;
using Whiteout.Survival;
using Whiteout.Survival.Unity;
using Whiteout.UI;

namespace Whiteout.Game
{
    /// <summary>
    /// Scope in the Game scene (child of <see cref="BootstrapLifetimeScope"/> when loaded by the bootstrap).
    /// Registers per-run objects: player config, input, and the player controller.
    /// Optional scene views (e.g. <see cref="MapRingDebugView"/>) are injected through the scope's
    /// Auto Inject Game Objects list in the Inspector, not by searching the scene (code-structure §7).
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a GameObject in the Game scene; assign PlayerConfig and the InputSystem_Actions asset.
    /// </code>
    /// </example>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private RunFlowConfig runFlowConfig;
        [SerializeField] private EnvironmentConfig environmentConfig;
        [SerializeField] private SurvivalConfig survivalConfig;

        /// <inheritdoc />
        protected override void Configure(IContainerBuilder builder)
        {
            if (runFlowConfig == null || environmentConfig == null || survivalConfig == null)
            {
                throw new InvalidOperationException("GameLifetimeScope: RunFlow, Environment and Survival Config must be assigned.");
            }

            builder.RegisterInstance(playerConfig);
            builder.RegisterInstance(inputActions);
            builder.Register<InputSystemPlayerInput>(Lifetime.Singleton).As<IPlayerInput>();
            builder.RegisterComponentInHierarchy<PlayerController>();

            builder.RegisterInstance(runFlowConfig.ToSettings());
            builder.Register<DayClock>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DayClockRunner>();

            builder.RegisterInstance(environmentConfig);
            builder.RegisterInstance(environmentConfig.ToSettings());
            builder.Register<NoWarmthSource>(Lifetime.Singleton).As<IWarmthSource>();
            builder.Register<EnvironmentSystem>(Lifetime.Singleton).AsSelf().As<IAmbientTemperatureSource>();
            builder.RegisterComponentInHierarchy<EnvironmentView>();
            builder.RegisterComponentInHierarchy<WeatherHud>();

            builder.RegisterInstance(survivalConfig.ToSettings());
            builder.Register<SurvivalSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SurvivalRunner>();
            builder.RegisterEntryPoint<SurvivalPlayerBridge>();
            builder.RegisterComponentInHierarchy<SurvivalHud>();
        }
    }
}
