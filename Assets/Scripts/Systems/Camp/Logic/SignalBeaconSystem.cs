using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Camp
{
    /// <summary>
    /// Rules of the signal beacon (core-rules section 1, tech-decisions 34). One beacon per run. Each action installs
    /// one part from the bag, the first missing one in spec order that the bag holds; an installed part never comes
    /// back. The last part publishes <see cref="SignalBeaconCompleted"/>. An action on the complete beacon publishes
    /// <see cref="RescueRequested"/> once, with the wait converted to game seconds through the day length. Also the
    /// <see cref="IRescueParts"/> the goal display reads: it works before the beacon is placed.
    /// </summary>
    /// <example>
    /// <code>
    /// var system = new SignalBeaconSystem(spec, bag, bus, clock);
    /// system.Place(x, y, z);
    /// if (system.TryAct() == BeaconActionResult.PartInstalled) { /* show the part */ }
    /// </code>
    /// </example>
    public sealed class SignalBeaconSystem : IRescueParts
    {
        private readonly SignalBeaconSpec _spec;
        private readonly IBag _bag;
        private readonly IEventBus _bus;
        private readonly IDayTimeSource _clock;

        /// <summary>Creates the system with no beacon placed.</summary>
        /// <param name="spec">Parts and rescue wait.</param>
        /// <param name="bag">Holds the parts before they are installed.</param>
        /// <param name="bus">Event channel.</param>
        /// <param name="clock">Day and night lengths, to turn the wait in days into game seconds.</param>
        public SignalBeaconSystem(SignalBeaconSpec spec, IBag bag, IEventBus bus, IDayTimeSource clock)
        {
            _spec = spec ?? throw new ArgumentNullException(nameof(spec));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>Parts and rescue wait.</summary>
        public SignalBeaconSpec Spec => _spec;

        /// <summary>The placed beacon, or null until it is placed.</summary>
        public SignalBeacon Beacon { get; private set; }

        /// <inheritdoc />
        public int Required => _spec.Parts.Count;

        /// <inheritdoc />
        public int Collected
        {
            get
            {
                var collected = 0;
                for (var i = 0; i < _spec.Parts.Count; i++)
                {
                    if (GetPartState(i) != RescuePartState.Missing)
                    {
                        collected++;
                    }
                }

                return collected;
            }
        }

        /// <inheritdoc />
        public RescuePartState GetPartState(int index)
        {
            if (index < 0 || index >= _spec.Parts.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (Beacon != null && Beacon.IsPartInstalled(index))
            {
                return RescuePartState.Installed;
            }

            return _bag.Count(_spec.Parts[index]) > 0 ? RescuePartState.Held : RescuePartState.Missing;
        }

        /// <summary>Registers the beacon frame that was just placed.</summary>
        /// <param name="x">World X.</param>
        /// <param name="y">World Y (ground height).</param>
        /// <param name="z">World Z.</param>
        /// <returns>The new beacon.</returns>
        /// <exception cref="InvalidOperationException">A beacon is already placed (one per run).</exception>
        public SignalBeacon Place(float x, float y, float z)
        {
            if (Beacon != null)
            {
                throw new InvalidOperationException("SignalBeaconSystem: a signal beacon is already placed (one per run).");
            }

            Beacon = new SignalBeacon(_spec.Parts.Count, x, y, z);
            return Beacon;
        }

        /// <summary>The part the next action would install: the first missing one in spec order that the bag holds.</summary>
        /// <param name="part">The part, when the result is true.</param>
        /// <returns>False when there is no beacon, it is complete, or the bag holds none of the missing parts.</returns>
        public bool NextInstallablePart(out ItemId part)
        {
            var index = NextInstallableIndex();
            part = index >= 0 ? _spec.Parts[index] : default;
            return index >= 0;
        }

        /// <summary>What <see cref="TryAct"/> would do right now, without changing anything.</summary>
        /// <returns>The result <see cref="TryAct"/> would give.</returns>
        public BeaconActionResult Check()
        {
            if (Beacon == null)
            {
                return BeaconActionResult.NoBeacon;
            }

            if (Beacon.RescueRequested)
            {
                return BeaconActionResult.NothingToDo;
            }

            if (Beacon.IsComplete)
            {
                return BeaconActionResult.RescueRequested;
            }

            return NextInstallableIndex() >= 0 ? BeaconActionResult.PartInstalled : BeaconActionResult.MissingPart;
        }

        /// <summary>
        /// Does the E action: installs exactly one part, or requests rescue when the beacon is complete.
        /// </summary>
        /// <returns>What happened. Only PartInstalled and RescueRequested change anything.</returns>
        public BeaconActionResult TryAct()
        {
            var result = Check();
            switch (result)
            {
                case BeaconActionResult.PartInstalled:
                    var index = NextInstallableIndex();
                    if (!_bag.TryRemove(_spec.Parts[index], 1))
                    {
                        return BeaconActionResult.MissingPart;
                    }

                    Beacon.Install(index);
                    if (Beacon.IsComplete)
                    {
                        _bus.Publish(new SignalBeaconCompleted());
                    }

                    return result;
                case BeaconActionResult.RescueRequested:
                    Beacon.RescueRequested = true;
                    _bus.Publish(new RescueRequested(_spec.RescueWaitDays * (_clock.DayLength + _clock.NightLength)));
                    return result;
                default:
                    return result;
            }
        }

        private int NextInstallableIndex()
        {
            if (Beacon == null || Beacon.IsComplete)
            {
                return -1;
            }

            for (var i = 0; i < _spec.Parts.Count; i++)
            {
                if (!Beacon.IsPartInstalled(i) && _bag.Count(_spec.Parts[i]) > 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
