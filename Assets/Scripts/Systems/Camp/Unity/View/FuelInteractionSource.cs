using System;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// E-action on the nearest campfire in reach: put in one wood, light a new fire with wood 2, or relight a
    /// burned-out fire with tinder 1 + wood 2, both while holding flint (core-rules section 4). When something is missing
    /// the prompt is greyed with have/need amounts and pressing E posts a notice (ui-plan section 8).
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;FuelInteractionSource&gt;(Lifetime.Singleton).As&lt;IInteractionSource&gt;();
    /// </code>
    /// </example>
    public sealed class FuelInteractionSource : IInteractionSource
    {
        private readonly CampSystem _camp;
        private readonly CampRegistry _registry;
        private readonly IPlayerPosition _player;
        private readonly IBag _bag;
        private readonly IItemNames _names;
        private readonly UiTextTable _texts;
        private readonly CampConfig _config;
        private readonly IEventBus _bus;
        private CampfireView _target;
        private CampfireView _promptTarget;
        private FireState _promptState;
        private int _promptBagVersion;

        /// <summary>Creates the source.</summary>
        /// <param name="camp">Campfire rules.</param>
        /// <param name="registry">Placed campfires.</param>
        /// <param name="player">Player position.</param>
        /// <param name="bag">The bag (for have/need amounts).</param>
        /// <param name="names">Item names.</param>
        /// <param name="texts">UI text table (cost formatting).</param>
        /// <param name="config">Hold time and priority.</param>
        /// <param name="bus">Event channel.</param>
        public FuelInteractionSource(
            CampSystem camp,
            CampRegistry registry,
            IPlayerPosition player,
            IBag bag,
            IItemNames names,
            UiTextTable texts,
            CampConfig config,
            IEventBus bus)
        {
            _camp = camp ?? throw new ArgumentNullException(nameof(camp));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _names = names ?? throw new ArgumentNullException(nameof(names));
            _texts = texts != null ? texts : throw new ArgumentNullException(nameof(texts));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public int Priority => _config.FuelPriority;

        /// <inheritdoc />
        public InteractionPrompt Prompt { get; private set; }

        /// <inheritdoc />
        public bool TryOffer(out float holdSeconds)
        {
            holdSeconds = _config.FuelHoldSeconds;
            _target = null;
            var best = float.MaxValue;
            var campfires = _registry.Campfires;
            for (var i = 0; i < campfires.Count; i++)
            {
                var distance = campfires[i].DistanceTo(_player);
                if (distance <= campfires[i].Definition.InteractRange && distance < best)
                {
                    best = distance;
                    _target = campfires[i];
                }
            }

            if (_target == null)
            {
                return false;
            }

            RefreshPrompt();
            return true;
        }

        /// <inheritdoc />
        public bool TryOfferPointed(in PointerQuery query, out float holdSeconds)
        {
            holdSeconds = _config.FuelHoldSeconds;
            _target = null;
            var best = float.MaxValue;
            var campfires = _registry.Campfires;
            for (var i = 0; i < campfires.Count; i++)
            {
                var fire = campfires[i];
                if (fire.DistanceTo(_player) > query.MaxPlayerDistance
                    || !query.Pointer.TryGetPointOnPlane(fire.transform.position.y, out var cursorX, out var cursorZ))
                {
                    continue;
                }

                var dx = fire.transform.position.x - cursorX;
                var dz = fire.transform.position.z - cursorZ;
                var distance = Mathf.Sqrt(dx * dx + dz * dz);
                if (distance <= Mathf.Max(query.HoverRadius, fire.Definition.FootprintRadius) && distance < best)
                {
                    best = distance;
                    _target = fire;
                }
            }

            if (_target == null)
            {
                return false;
            }

            RefreshPrompt();
            return true;
        }

        /// <inheritdoc />
        public bool IsTargetInReach()
        {
            return _target != null && _target.DistanceTo(_player) <= _target.Definition.InteractRange;
        }

        /// <inheritdoc />
        public bool Begin()
        {
            var fire = _target.Fire;
            switch (_camp.CheckFuel(fire))
            {
                case FireActionResult.MissingIgniter:
                    PostMissingRequired(fire.Spec);
                    return false;
                case FireActionResult.MissingRelightItems:
                    _bus.Publish(new NoticePosted(
                        CampTextKeys.MissingMaterialsNotice, _texts.FormatItems(fire.Spec.RelightCost, _names, _bag)));
                    return false;
                case FireActionResult.MissingIgniteItems:
                    _bus.Publish(new NoticePosted(
                        CampTextKeys.MissingMaterialsNotice, _texts.FormatItems(fire.Spec.IgniteCost, _names, _bag)));
                    return false;
                case FireActionResult.NoFuelItem:
                    _bus.Publish(new NoticePosted(
                        CampTextKeys.MissingMaterialsNotice, _names.GetDisplayName(fire.Spec.FuelItem)));
                    return false;
                case FireActionResult.FuelFull:
                    _bus.Publish(new NoticePosted(CampTextKeys.FireFullNotice, string.Empty));
                    return false;
                default:
                    return true;
            }
        }

        /// <inheritdoc />
        public bool IsStillAvailable()
        {
            return _target != null
                && _target.DistanceTo(_player) <= _target.Definition.InteractRange
                && _camp.CanFuel(_target.Fire);
        }

        /// <inheritdoc />
        public void Complete()
        {
            if (_target != null)
            {
                _camp.TryFuel(_target.Fire);
            }
        }

        /// <inheritdoc />
        public void Cancel()
        {
        }

        private void RefreshPrompt()
        {
            var fire = _target.Fire;
            if (_target == _promptTarget && fire.State == _promptState && _bag.Version == _promptBagVersion)
            {
                return;
            }

            _promptTarget = _target;
            _promptState = fire.State;
            _promptBagVersion = _bag.Version;
            string key;
            string argument;
            switch (fire.State)
            {
                case FireState.BurnedOut:
                    key = CampTextKeys.RelightPrompt;
                    argument = _texts.FormatItems(fire.Spec.RelightCost, _names, _bag);
                    break;
                case FireState.Unlit:
                    key = CampTextKeys.IgnitePrompt;
                    argument = _texts.FormatItems(fire.Spec.IgniteCost, _names, _bag);
                    break;
                default:
                    key = CampTextKeys.FuelPrompt;
                    argument = _names.GetDisplayName(fire.Spec.FuelItem);
                    break;
            }

            Prompt = new InteractionPrompt(key, argument, _camp.CanFuel(fire), fire.X, fire.Y, fire.Z);
        }

        private void PostMissingRequired(CampfireSpec spec)
        {
            for (var i = 0; i < spec.IgniterItems.Count; i++)
            {
                if (_bag.Count(spec.IgniterItems[i]) < 1)
                {
                    _bus.Publish(new NoticePosted(
                        CampTextKeys.MissingRequiredNotice, _names.GetDisplayName(spec.IgniterItems[i])));
                    return;
                }
            }
        }
    }
}
