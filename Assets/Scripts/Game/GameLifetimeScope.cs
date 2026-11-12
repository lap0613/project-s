using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Input;
using Whiteout.Player.Unity;

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

        /// <inheritdoc />
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(playerConfig);
            builder.RegisterInstance(inputActions);
            builder.Register<InputSystemPlayerInput>(Lifetime.Singleton).As<IPlayerInput>();
            builder.RegisterComponentInHierarchy<PlayerController>();
        }
    }
}
