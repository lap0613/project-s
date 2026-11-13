using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Whiteout.Camp;
using Whiteout.Camp.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;
using Whiteout.Environment;
using Whiteout.Environment.Unity;
using Whiteout.Input;
using Whiteout.Player.Unity;
using Whiteout.Resources;
using Whiteout.Resources.Unity;
using Whiteout.RunFlow;
using Whiteout.RunFlow.Unity;
using Whiteout.Survival;
using Whiteout.Survival.Unity;
using Whiteout.UI;

namespace Whiteout.Game
{
    /// <summary>
    /// Scope in the Game scene (child of <see cref="BootstrapLifetimeScope"/> when loaded by the bootstrap).
    /// Registers per-run objects: player, input, day clock, environment, survival, resources (bag, pickups),
    /// camp (campfires, shelters, placement mode), the E-key director and the HUD.
    /// Optional scene views (e.g. <see cref="MapRingDebugView"/>) are injected through the scope's
    /// Auto Inject Game Objects list in the Inspector, not by searching the scene (code-structure §7).
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a GameObject in the Game scene; assign every config asset, the UI text table and the
    /// // InputSystem_Actions asset.
    /// </code>
    /// </example>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private RunFlowConfig runFlowConfig;
        [SerializeField] private EnvironmentConfig environmentConfig;
        [SerializeField] private SurvivalConfig survivalConfig;
        [SerializeField] private ResourcesConfig resourcesConfig;
        [SerializeField] private CampConfig campConfig;
        [SerializeField] private UiTextTable uiTexts;
        [SerializeField] private HudConfig hudConfig;

        /// <inheritdoc />
        protected override void Configure(IContainerBuilder builder)
        {
            if (playerConfig == null || inputActions == null || runFlowConfig == null || environmentConfig == null || survivalConfig == null
                || resourcesConfig == null || campConfig == null || uiTexts == null || hudConfig == null)
            {
                throw new InvalidOperationException(
                    "GameLifetimeScope: Player Config, Input Actions, RunFlow, Environment, Survival, Resources, Camp and HUD Config and the UI Text Table must be assigned.");
            }

            builder.RegisterInstance(playerConfig);
            builder.RegisterInstance(inputActions);
            builder.Register<InputSystemPlayerInput>(Lifetime.Singleton);
            builder.Register<PlayerActionLock>(Lifetime.Singleton).AsSelf().As<IPlayerInput>();
            builder.Register<InputSystemInteractionInput>(Lifetime.Singleton).As<IInteractionInput>();
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterEntryPoint<PlayerPositionSource>();
            builder.RegisterEntryPoint<InteractionDirector>();

            builder.RegisterInstance(uiTexts);
            builder.RegisterInstance(hudConfig);

            builder.RegisterInstance(runFlowConfig.ToSettings());
            builder.Register<DayClock>(Lifetime.Singleton).AsSelf().As<IDayTimeSource>();
            builder.RegisterEntryPoint<DayClockRunner>();

            builder.RegisterInstance(environmentConfig);
            builder.RegisterInstance(environmentConfig.ToSettings());
            builder.Register<EnvironmentSystem>(Lifetime.Singleton).AsSelf().As<IAmbientTemperatureSource>();
            builder.RegisterComponentInHierarchy<EnvironmentView>();
            builder.RegisterComponentInHierarchy<WeatherHud>();

            builder.RegisterInstance(survivalConfig.ToSettings());
            builder.Register<SurvivalSystem>(Lifetime.Singleton).AsSelf().As<IRestTarget>().As<IVitalsReceiver>();
            builder.RegisterEntryPoint<SurvivalRunner>();
            builder.RegisterEntryPoint<SurvivalPlayerBridge>();
            builder.RegisterComponentInHierarchy<SurvivalHud>();

            builder.RegisterInstance(resourcesConfig);
            builder.RegisterInstance(resourcesConfig.ToSettings()).AsSelf().As<IItemNames>();
            builder.Register<Bag>(Lifetime.Singleton).AsSelf().As<IBag>();
            builder.Register<GatherRules>(Lifetime.Singleton);
            builder.Register<CookingRules>(Lifetime.Singleton);
            builder.Register<ConsumableRules>(Lifetime.Singleton);
            builder.Register<PickupRegistry>(Lifetime.Singleton);
            builder.Register<PickupFactory>(Lifetime.Singleton);
            builder.Register<GatherInteractionSource>(Lifetime.Singleton).As<IInteractionSource>();
            builder.Register<CookInteractionSource>(Lifetime.Singleton).As<IInteractionSource>();
            builder.Register<UseItemAction>(Lifetime.Singleton).As<IBagAction>();
            builder.Register<DropItemAction>(Lifetime.Singleton).As<IBagAction>();
            builder.RegisterEntryPoint<BagSelectionRunner>();
            builder.RegisterEntryPoint<SurvivalGatherBridge>();
            builder.RegisterComponentInHierarchy<ResourceSpawner>();

            builder.RegisterInstance(campConfig);
            builder.Register<CampSystem>(Lifetime.Singleton).AsSelf().As<IWarmthSource>().As<IFireStatus>();
            builder.Register<CampRegistry>(Lifetime.Singleton);
            builder.Register<FuelInteractionSource>(Lifetime.Singleton).As<IInteractionSource>();
            builder.Register<RestInteractionSource>(Lifetime.Singleton).As<IInteractionSource>();
            builder.RegisterEntryPoint<CampRunner>();
            builder.RegisterEntryPoint<PlacementController>();

            builder.RegisterComponentInHierarchy<BagHud>();
            builder.RegisterComponentInHierarchy<PlacementHud>();
            builder.RegisterComponentInHierarchy<NotificationHud>();
            builder.RegisterComponentInHierarchy<WorldUiHud>();
        }
    }
}
