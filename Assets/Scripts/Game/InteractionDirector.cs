using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// Runs the interact actions for every system (code-structure section 4: connections between systems live in Game).
    /// <para>
    /// E: each frame it asks the <see cref="IInteractionSource"/>s, highest priority first, for a target near the player;
    /// pressing E starts the first one found and E must stay held for its time (release cancels).
    /// </para>
    /// <para>
    /// Left click (ui-plan section 8.1, M1), one thing per click in this order: HUD click (not handled here), placement
    /// mode (the placement controller uses the click), a highlighted target under the cursor (this director), else the
    /// click becomes <see cref="IAttackInput.AttackClicked"/>. A target farther than the interaction range is walked to in a
    /// straight line and the action starts on arrival; WASD, another click, damage or being blocked cancels the walk (M4).
    /// A click-started action runs to its end without holding anything (M5); resting ends when the player moves.
    /// </para>
    /// Movement is locked while an action runs (core-rules section 4). A finished action needs a new press or click.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;InteractionDirector&gt;().As&lt;IInteractionStatus&gt;().As&lt;IAttackInput&gt;();
    /// </code>
    /// </example>
    public sealed class InteractionDirector : ITickable, IInteractionStatus, IAttackInput, IDisposable
    {
        private readonly IInteractionInput _input;
        private readonly PlayerActionLock _lock;
        private readonly IPlacementMenu _placement;
        private readonly IPointerWorld _pointer;
        private readonly IPlayerPosition _player;
        private readonly PointerConfig _config;
        private readonly List<IInteractionSource> _sources;
        private readonly IDisposable _diedSubscription;
        private readonly IDisposable _damagedSubscription;
        private IInteractionSource _offered;
        private IInteractionSource _active;
        private IInteractionSource _hover;
        private InteractionPrompt _hoverPrompt;
        private float _hoverHold;
        private IInteractionSource _walk;
        private InteractionPrompt _walkPrompt;
        private float _walkHold;
        private float _walkStuckSeconds;
        private float _walkLastX;
        private float _walkLastZ;
        private bool _startedByClick;
        private float _holdSeconds;
        private float _elapsed;
        private bool _playerDead;

        /// <summary>Creates the director.</summary>
        /// <param name="input">Interact button, left click and cursor.</param>
        /// <param name="sources">Every interact action in the game.</param>
        /// <param name="movementLock">Stops player movement during an action and walks the player to a far target.</param>
        /// <param name="placement">Tells whether the placement mode is open and whether it used this frame's click.</param>
        /// <param name="pointer">Where the cursor points in the world.</param>
        /// <param name="player">Player position.</param>
        /// <param name="config">Click range, hover radius and walk cancel values.</param>
        /// <param name="bus">Event channel (player death, damage).</param>
        public InteractionDirector(
            IInteractionInput input,
            IReadOnlyList<IInteractionSource> sources,
            PlayerActionLock movementLock,
            IPlacementMenu placement,
            IPointerWorld pointer,
            IPlayerPosition player,
            PointerConfig config,
            IEventBus bus)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _lock = movementLock ?? throw new ArgumentNullException(nameof(movementLock));
            _placement = placement ?? throw new ArgumentNullException(nameof(placement));
            _pointer = pointer ?? throw new ArgumentNullException(nameof(pointer));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _sources = new List<IInteractionSource>(sources ?? throw new ArgumentNullException(nameof(sources)));
            _sources.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            _diedSubscription = bus.Subscribe<PlayerDied>(_ =>
            {
                _playerDead = true;
                Stop(false);
                CancelWalk();
                _offered = null;
                _hover = null;
            });
            _damagedSubscription = bus.Subscribe<PlayerDamaged>(_ => CancelWalk());
        }

        /// <inheritdoc />
        public bool HasPrompt => TryGetPrompt(out _);

        /// <inheritdoc />
        public InteractionPrompt Prompt => TryGetPrompt(out var prompt) ? prompt : default;

        /// <inheritdoc />
        public bool IsHolding => _active != null && _holdSeconds > 0f;

        /// <inheritdoc />
        public float Progress => IsHolding ? Mathf.Clamp01(_elapsed / _holdSeconds) : 0f;

        /// <inheritdoc />
        public bool AttackClicked { get; private set; }

        /// <summary>
        /// The target to outline: the one under the cursor, the one being walked to, or the one being worked on after a click.
        /// A target near the player that only E would pick is not outlined; only the mouse picks one.
        /// </summary>
        /// <param name="prompt">The target's prompt (its position and whether the action is possible).</param>
        /// <returns>True when there is a target to outline.</returns>
        public bool TryGetHighlight(out InteractionPrompt prompt)
        {
            if (_active != null && _startedByClick)
            {
                prompt = _active.Prompt;
                return true;
            }

            if (_walk != null)
            {
                prompt = _walkPrompt;
                return true;
            }

            if (_hover != null)
            {
                prompt = _hoverPrompt;
                return true;
            }

            prompt = default;
            return false;
        }

        /// <inheritdoc />
        public void Tick()
        {
            AttackClicked = false;
            if (_playerDead)
            {
                return;
            }

            if (_active != null)
            {
                TickActive();
                return;
            }

            if (_walk != null)
            {
                TickWalk();
                return;
            }

            _offered = null;
            _hover = null;
            if (_placement.IsOpen)
            {
                return;
            }

            if (_pointer.IsOverWorld)
            {
                FindHover();
            }

            if (ClickIsFree())
            {
                if (_hover != null)
                {
                    StartPointed();
                }
                else if (_input.HasPointer)
                {
                    AttackClicked = true;
                }

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

                if (BeginAction(_sources[i], hold, false))
                {
                    break;
                }
            }
        }

        /// <summary>Unsubscribes and releases the movement lock.</summary>
        public void Dispose()
        {
            _diedSubscription.Dispose();
            _damagedSubscription.Dispose();
            CancelWalk();
            Stop(false);
        }

        private bool ClickIsFree()
        {
            return _input.ConfirmPressed && _placement.ClickConsumedFrame != Time.frameCount && !_pointer.IsOverUi;
        }

        private bool TryGetPrompt(out InteractionPrompt prompt)
        {
            if (_active != null)
            {
                prompt = _active.Prompt;
                return true;
            }

            if (_walk != null)
            {
                prompt = _walkPrompt;
                return true;
            }

            if (_hover != null)
            {
                prompt = _hoverPrompt;
                return true;
            }

            if (_offered != null)
            {
                prompt = _offered.Prompt;
                return true;
            }

            prompt = default;
            return false;
        }

        private void FindHover()
        {
            var query = new PointerQuery(_pointer, _config.ClickRange, _config.HoverRadius);
            for (var i = 0; i < _sources.Count; i++)
            {
                if (_sources[i].TryOfferPointed(query, out var hold))
                {
                    _hover = _sources[i];
                    _hoverHold = hold;
                    _hoverPrompt = _sources[i].Prompt;
                    return;
                }
            }
        }

        private void StartPointed()
        {
            var source = _hover;
            if (!_hoverPrompt.IsAvailable)
            {
                // A greyed target: Begin posts why it cannot be done; walking there would change nothing.
                source.Begin();
                return;
            }

            if (source.IsTargetInReach())
            {
                BeginAction(source, _hoverHold, true);
                return;
            }

            _walk = source;
            _walkPrompt = _hoverPrompt;
            _walkHold = _hoverHold;
            _walkStuckSeconds = 0f;
            _walkLastX = _player.X;
            _walkLastZ = _player.Z;
        }

        private bool BeginAction(IInteractionSource source, float hold, bool byClick)
        {
            if (!source.Begin())
            {
                return false;
            }

            _active = source;
            _holdSeconds = hold;
            _elapsed = 0f;
            _startedByClick = byClick;
            _lock.MovementLocked = true;
            return true;
        }

        private void TickWalk()
        {
            if (_lock.HasManualInput || ClickIsFree())
            {
                CancelWalk();
                return;
            }

            if (_walk.IsTargetInReach())
            {
                var source = _walk;
                var hold = _walkHold;
                CancelWalk();
                BeginAction(source, hold, true);
                return;
            }

            _lock.SetAutoMove(_walkPrompt.X - _player.X, _walkPrompt.Z - _player.Z);
            var dx = _player.X - _walkLastX;
            var dz = _player.Z - _walkLastZ;
            _walkLastX = _player.X;
            _walkLastZ = _player.Z;
            var slowest = _config.StuckSpeed * Time.deltaTime;
            if (dx * dx + dz * dz < slowest * slowest)
            {
                _walkStuckSeconds += Time.deltaTime;
                if (_walkStuckSeconds >= _config.StuckCancelSeconds)
                {
                    CancelWalk();
                }
            }
            else
            {
                _walkStuckSeconds = 0f;
            }
        }

        private void CancelWalk()
        {
            _walk = null;
            _lock.ClearAutoMove();
        }

        private void TickActive()
        {
            if (_startedByClick)
            {
                if (_holdSeconds <= 0f && _lock.HasManualInput)
                {
                    Stop(false);
                    return;
                }
            }
            else if (!_input.InteractHeld)
            {
                Stop(false);
                return;
            }

            if (!_active.IsStillAvailable())
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
            _startedByClick = false;
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
