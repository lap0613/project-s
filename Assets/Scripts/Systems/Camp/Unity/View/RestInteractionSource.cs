using System;
using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// E-action inside a shelter: rest for as long as E is held (core-rules section 2). Resting is reported to
    /// Survival through <see cref="IRestTarget"/>; it ends when E is released or the player leaves the radius.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;RestInteractionSource&gt;(Lifetime.Singleton).As&lt;IInteractionSource&gt;();
    /// </code>
    /// </example>
    public sealed class RestInteractionSource : IInteractionSource
    {
        private readonly CampRegistry _registry;
        private readonly IPlayerPosition _player;
        private readonly IRestTarget _rest;
        private readonly CampConfig _config;
        private ShelterView _target;

        /// <summary>Creates the source.</summary>
        /// <param name="registry">Placed shelters.</param>
        /// <param name="player">Player position.</param>
        /// <param name="rest">Receives the resting state.</param>
        /// <param name="config">Priority.</param>
        public RestInteractionSource(CampRegistry registry, IPlayerPosition player, IRestTarget rest, CampConfig config)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _rest = rest ?? throw new ArgumentNullException(nameof(rest));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc />
        public int Priority => _config.RestPriority;

        /// <inheritdoc />
        public InteractionPrompt Prompt { get; private set; }

        /// <inheritdoc />
        public bool TryOffer(out float holdSeconds)
        {
            holdSeconds = 0f;
            var shelters = _registry.Shelters;
            for (var i = 0; i < shelters.Count; i++)
            {
                if (!shelters[i].Contains(_player))
                {
                    continue;
                }

                if (shelters[i] != _target)
                {
                    _target = shelters[i];
                    var position = _target.transform.position;
                    Prompt = new InteractionPrompt(CampTextKeys.RestPrompt, string.Empty, true, position.x, position.y, position.z);
                }

                return true;
            }

            _target = null;
            return false;
        }

        /// <inheritdoc />
        public bool TryOfferPointed(in PointerQuery query, out float holdSeconds)
        {
            holdSeconds = 0f;
            var shelters = _registry.Shelters;
            for (var i = 0; i < shelters.Count; i++)
            {
                var shelter = shelters[i];
                var position = shelter.transform.position;
                var px = position.x - _player.X;
                var pz = position.z - _player.Z;
                if (px * px + pz * pz > query.MaxPlayerDistance * query.MaxPlayerDistance
                    || !query.Pointer.TryGetPointOnPlane(position.y, out var cursorX, out var cursorZ))
                {
                    continue;
                }

                var dx = position.x - cursorX;
                var dz = position.z - cursorZ;
                var radius = Mathf.Max(query.HoverRadius, shelter.PointerRadius);
                if (dx * dx + dz * dz <= radius * radius)
                {
                    _target = shelter;
                    Prompt = new InteractionPrompt(CampTextKeys.RestPrompt, string.Empty, true, position.x, position.y, position.z);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool IsTargetInReach()
        {
            return _target != null && _target.Contains(_player);
        }

        /// <inheritdoc />
        public bool Begin()
        {
            _rest.SetResting(true);
            return true;
        }

        /// <inheritdoc />
        public bool IsStillAvailable()
        {
            return _target != null && _target.Contains(_player);
        }

        /// <inheritdoc />
        public void Complete()
        {
            _rest.SetResting(false);
        }

        /// <inheritdoc />
        public void Cancel()
        {
            _rest.SetResting(false);
        }
    }
}
