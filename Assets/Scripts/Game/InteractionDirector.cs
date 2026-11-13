using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Game
{
    /// <summary>
    /// Runs the E key for every system (code-structure section 4: connections between systems live in Game).
    /// Each frame it asks the <see cref="IInteractionSource"/>s, highest priority first, for a target; the first
    /// one found is the prompt. Pressing E starts it; holding E for its time completes it; releasing E, losing the
    /// target or dying cancels it. Movement is locked while an action runs (core-rules section 4). A finished action
    /// needs a new press to start the next one. While the placement mode is open no action starts.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;InteractionDirector&gt;().As&lt;IInteractionStatus&gt;();
    /// </code>
    /// </example>
    public sealed class InteractionDirector : ITickable, IInteractionStatus, IDisposable
    {
        private readonly IInteractionInput _input;
        private readonly PlayerActionLock _lock;
        private readonly IPlacementMenu _placement;
        private readonly List<IInteractionSource> _sources;
        private readonly IDisposable _diedSubscription;
        private IInteractionSource _offered;
        private IInteractionSource _active;
        private float _holdSeconds;
        private float _elapsed;
        private bool _playerDead;

        /// <summary>Creates the director.</summary>
        /// <param name="input">Interact button.</param>
        /// <param name="sources">Every E-action in the game.</param>
        /// <param name="movementLock">Stops player movement during an action.</param>
        /// <param name="placement">Tells whether the placement mode is open.</param>
        /// <param name="bus">Event channel (player death).</param>
        public InteractionDirector(
            IInteractionInput input,
            IReadOnlyList<IInteractionSource> sources,
            PlayerActionLock movementLock,
            IPlacementMenu placement,
            IEventBus bus)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _lock = movementLock ?? throw new ArgumentNullException(nameof(movementLock));
            _placement = placement ?? throw new ArgumentNullException(nameof(placement));
            _sources = new List<IInteractionSource>(sources ?? throw new ArgumentNullException(nameof(sources)));
            _sources.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            _diedSubscription = bus.Subscribe<PlayerDied>(_ =>
            {
                _playerDead = true;
                Stop(false);
                _offered = null;
            });
        }

        /// <inheritdoc />
        public bool HasPrompt => _active != null || _offered != null;

        /// <inheritdoc />
        public InteractionPrompt Prompt => _active != null ? _active.Prompt : _offered != null ? _offered.Prompt : default;

        /// <inheritdoc />
        public bool IsHolding => _active != null && _holdSeconds > 0f;

        /// <inheritdoc />
        public float Progress => IsHolding ? Mathf.Clamp01(_elapsed / _holdSeconds) : 0f;

        /// <inheritdoc />
        public void Tick()
        {
            if (_playerDead)
            {
                return;
            }

            if (_active != null)
            {
                TickActive();
                return;
            }

            _offered = null;
            if (_placement.IsOpen)
            {
                return;
            }

            var pressed = _input.InteractPressed;
            for (var i = 0; i < _sources.Count; i++)
            {
                if (!_sources[i].TryOffer(out var hold))
                {
                    continue;
                }

                if (_offered == null)
                {
                    _offered = _sources[i];
                }

                if (!pressed)
                {
                    break;
                }

                if (_sources[i].Begin())
                {
                    _active = _sources[i];
                    _holdSeconds = hold;
                    _elapsed = 0f;
                    _lock.MovementLocked = true;
                    break;
                }
            }
        }

        /// <summary>Unsubscribes and releases the movement lock.</summary>
        public void Dispose()
        {
            _diedSubscription.Dispose();
            Stop(false);
        }

        private void TickActive()
        {
            if (!_input.InteractHeld || !_active.IsStillAvailable())
            {
                Stop(false);
                return;
            }

            if (_holdSeconds <= 0f)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed >= _holdSeconds)
            {
                Stop(true);
            }
        }

        private void Stop(bool completed)
        {
            if (_active == null)
            {
                return;
            }

            var source = _active;
            _active = null;
            _lock.MovementLocked = false;
            if (completed)
            {
                source.Complete();
            }
            else
            {
                source.Cancel();
            }
        }
    }
}
