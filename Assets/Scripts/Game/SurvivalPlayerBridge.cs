using System;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Stats;
using Whiteout.Player.Unity;
using Whiteout.Survival;

namespace Whiteout.Game
{
    /// <summary>
    /// Connects Survival to the player without Survival knowing Player (code-structure section 3):
    /// applies the low-body-temperature move speed penalty as a Multiply modifier on
    /// <see cref="PlayerController.MoveSpeedStat"/>, and disables the controller when <see cref="PlayerDied"/> arrives.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;SurvivalPlayerBridge&gt;();
    /// </code>
    /// </example>
    public sealed class SurvivalPlayerBridge : ITickable, IDisposable
    {
        private const string ColdSource = "survival.cold";

        private readonly SurvivalSystem _survival;
        private readonly SurvivalSettings _settings;
        private readonly PlayerController _player;
        private readonly IDisposable _diedSubscription;
        private IDisposable _coldPenalty;

        /// <summary>Creates the bridge and subscribes to <see cref="PlayerDied"/>.</summary>
        /// <param name="survival">Source of the low-temperature state.</param>
        /// <param name="settings">Supplies the penalty size.</param>
        /// <param name="player">The player whose speed is changed and whose input stops on death.</param>
        /// <param name="bus">Event channel.</param>
        public SurvivalPlayerBridge(SurvivalSystem survival, SurvivalSettings settings, PlayerController player, IEventBus bus)
        {
            _survival = survival ?? throw new ArgumentNullException(nameof(survival));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            // Unity's == also catches a destroyed object, which ?? would let through.
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player;
            _diedSubscription = bus.Subscribe<PlayerDied>(_ =>
            {
                if (_player != null)
                {
                    _player.enabled = false;
                }
            });
        }

        /// <inheritdoc />
        public void Tick()
        {
            if (_player == null)
            {
                return;
            }

            var low = _survival.IsTemperatureLow;
            if (low && _coldPenalty == null)
            {
                _coldPenalty = _player.MoveSpeedStat.AddModifier(
                    new StatModifier(StatModifierKind.Multiply, -_settings.MoveSpeedPenalty, ColdSource));
            }
            else if (!low && _coldPenalty != null)
            {
                _coldPenalty.Dispose();
                _coldPenalty = null;
            }
        }

        /// <summary>Unsubscribes and removes the penalty.</summary>
        public void Dispose()
        {
            _diedSubscription.Dispose();
            _coldPenalty?.Dispose();
        }
    }
}
