using System;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// E-action on the signal beacon in reach (core-rules section 1, tech-decisions 34): install one part from the
    /// bag (antenna, then battery, then transmitter), or request rescue once all are installed. Without a part the
    /// prompt is greyed with installed/required and pressing E posts a notice (ui-plan section 8). After rescue is
    /// requested it offers nothing.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;BeaconInteractionSource&gt;(Lifetime.Singleton).As&lt;IInteractionSource&gt;();
    /// </code>
    /// </example>
    public sealed class BeaconInteractionSource : IInteractionSource
    {
        private readonly SignalBeaconSystem _system;
        private readonly CampRegistry _registry;
        private readonly IPlayerPosition _player;
        private readonly IBag _bag;
        private readonly IItemNames _names;
        private readonly SignalBeaconDefinition _definition;
        private readonly IEventBus _bus;
        private SignalBeaconView _target;
        private SignalBeaconView _promptTarget;
        private int _promptInstalled = -1;
        private int _promptBagVersion = -1;

        /// <summary>Creates the source.</summary>
        /// <param name="system">Beacon rules.</param>
        /// <param name="registry">The placed beacon.</param>
        /// <param name="player">Player position.</param>
        /// <param name="bag">The bag (prompt refresh).</param>
        /// <param name="names">Item names.</param>
        /// <param name="config">Holds the beacon definition (range, hold time, priority).</param>
        /// <param name="bus">Event channel.</param>
        public BeaconInteractionSource(
            SignalBeaconSystem system,
            CampRegistry registry,
            IPlayerPosition player,
            IBag bag,
            IItemNames names,
            CampConfig config,
            IEventBus bus)
        {
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _names = names ?? throw new ArgumentNullException(nameof(names));
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _definition = config.SignalBeacon != null
                ? config.SignalBeacon
                : throw new InvalidOperationException("BeaconInteractionSource: Camp Config needs a Signal Beacon definition.");
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public int Priority => _definition.InteractPriority;

        /// <inheritdoc />
        public InteractionPrompt Prompt { get; private set; }

        /// <inheritdoc />
        public bool TryOffer(out float holdSeconds)
        {
            holdSeconds = _definition.ActionHoldSeconds;
            _target = null;
            var beacon = _registry.Beacon;
            if (beacon == null || !HasAction() || beacon.DistanceTo(_player) > _definition.InteractRange)
            {
                return false;
            }

            _target = beacon;
            RefreshPrompt();
            return true;
        }

        /// <inheritdoc />
        public bool TryOfferPointed(in PointerQuery query, out float holdSeconds)
        {
            holdSeconds = _definition.ActionHoldSeconds;
            _target = null;
            var beacon = _registry.Beacon;
            if (beacon == null
                || !HasAction()
                || beacon.DistanceTo(_player) > query.MaxPlayerDistance
                || !query.Pointer.TryGetPointOnPlane(beacon.transform.position.y, out var cursorX, out var cursorZ))
            {
                return false;
            }

            var dx = beacon.transform.position.x - cursorX;
            var dz = beacon.transform.position.z - cursorZ;
            if (Mathf.Sqrt(dx * dx + dz * dz) > Mathf.Max(query.HoverRadius, _definition.FootprintRadius))
            {
                return false;
            }

            _target = beacon;
            RefreshPrompt();
            return true;
        }

        /// <inheritdoc />
        public bool IsTargetInReach()
        {
            return _target != null && _target.DistanceTo(_player) <= _definition.InteractRange;
        }

        /// <inheritdoc />
        public bool Begin()
        {
            switch (_system.Check())
            {
                case BeaconActionResult.PartInstalled:
                case BeaconActionResult.RescueRequested:
                    return true;
                case BeaconActionResult.MissingPart:
                    _bus.Publish(new NoticePosted(CampTextKeys.BeaconMissingPartNotice, string.Empty));
                    return false;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool IsStillAvailable()
        {
            if (_target == null || _target.DistanceTo(_player) > _definition.InteractRange)
            {
                return false;
            }

            var check = _system.Check();
            return check == BeaconActionResult.PartInstalled || check == BeaconActionResult.RescueRequested;
        }

        /// <inheritdoc />
        public void Complete()
        {
            if (_target == null)
            {
                return;
            }

            switch (_system.TryAct())
            {
                case BeaconActionResult.PartInstalled:
                    if (_system.Beacon.IsComplete)
                    {
                        _bus.Publish(new NoticePosted(CampTextKeys.BeaconCompletedNotice, string.Empty));
                    }

                    break;
                case BeaconActionResult.RescueRequested:
                    _bus.Publish(new NoticePosted(CampTextKeys.RescueRequestedNotice, string.Empty));
                    break;
            }
        }

        /// <inheritdoc />
        public void Cancel()
        {
        }

        private bool HasAction()
        {
            var check = _system.Check();
            return check != BeaconActionResult.NoBeacon && check != BeaconActionResult.NothingToDo;
        }

        private void RefreshPrompt()
        {
            var beacon = _system.Beacon;
            if (_target == _promptTarget && beacon.InstalledCount == _promptInstalled && _bag.Version == _promptBagVersion)
            {
                return;
            }

            _promptTarget = _target;
            _promptInstalled = beacon.InstalledCount;
            _promptBagVersion = _bag.Version;
            if (beacon.IsComplete)
            {
                Prompt = new InteractionPrompt(CampTextKeys.BeaconRequestPrompt, string.Empty, true, beacon.X, beacon.Y, beacon.Z);
            }
            else if (_system.NextInstallablePart(out var part))
            {
                Prompt = new InteractionPrompt(
                    CampTextKeys.BeaconInstallPrompt, _names.GetDisplayName(part), true, beacon.X, beacon.Y, beacon.Z);
            }
            else
            {
                Prompt = new InteractionPrompt(
                    CampTextKeys.BeaconMissingPrompt, beacon.InstalledCount + "/" + beacon.PartCount, false,
                    beacon.X, beacon.Y, beacon.Z);
            }
        }
    }
}
