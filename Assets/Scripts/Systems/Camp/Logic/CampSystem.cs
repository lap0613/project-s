using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Camp
{
    /// <summary>
    /// Placing structures and running campfires (core-rules sections 3 and 4, items.md section 7).
    /// Placement pays the cost from the bag and needs the required items (flint) present.
    /// A new campfire is unlit; the first wood lights it. One wood burns 60 s, at most 5 wood.
    /// A burned-out fire is relit with tinder 1 + wood 2 (flint needed); that wood becomes its fuel.
    /// Warmth is the single warmest lit fire whose radius contains the player; fires are not summed.
    /// Positions are judged by the View and passed in as flags (tech-decisions item 13).
    /// Time moves only through <see cref="Tick"/>. A structure whose spec has a <see cref="StructureSpec.MaxCount"/>
    /// can be placed only that many times per run (tech-decisions 34, R4); the others have no limit.
    /// </summary>
    /// <example>
    /// <code>
    /// var camp = new CampSystem(bag, bus);
    /// if (camp.TryPlace(campfireStructure, x, z) == PlaceResult.Placed) { var fire = camp.AddCampfire(campfireSpec, x, y, z); }
    /// camp.Tick(0.016f);
    /// float warmth = camp.CurrentWarmth;
    /// </code>
    /// </example>
    public sealed class CampSystem : IWarmthSource, IFireStatus
    {
        private readonly IBag _bag;
        private readonly IEventBus _bus;
        private readonly List<Campfire> _fires = new List<Campfire>();
        private readonly Dictionary<string, int> _placedCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        /// <summary>Creates the system with no structures.</summary>
        /// <param name="bag">Pays placement, fuel and relight costs.</param>
        /// <param name="bus">Event channel.</param>
        public CampSystem(IBag bag, IEventBus bus)
        {
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <summary>All placed campfires.</summary>
        public IReadOnlyList<Campfire> Campfires => _fires;

        /// <inheritdoc />
        public float CurrentWarmth
        {
            get
            {
                var warmest = 0f;
                for (var i = 0; i < _fires.Count; i++)
                {
                    if (_fires[i].IsLit && _fires[i].PlayerInRange)
                    {
                        warmest = Math.Max(warmest, _fires[i].Spec.Warmth);
                    }
                }

                return warmest;
            }
        }

        /// <inheritdoc />
        public bool HasNearbyFire => FindNearby() != null;

        /// <inheritdoc />
        public bool NearbyFireLit => FindNearby()?.IsLit ?? false;

        /// <inheritdoc />
        public float NearbyFuelSeconds => FindNearby()?.FuelSeconds ?? 0f;

        /// <inheritdoc />
        public float NearbySecondsPerFuel => FindNearby()?.Spec.BurnSecondsPerFuel ?? 0f;

        /// <inheritdoc />
        public int NearbyMaxFuel => FindNearby()?.Spec.MaxFuel ?? 0;

        /// <inheritdoc />
        public float NearbyFireX => FindNearby()?.X ?? 0f;

        /// <inheritdoc />
        public float NearbyFireY => FindNearby()?.Y ?? 0f;

        /// <inheritdoc />
        public float NearbyFireZ => FindNearby()?.Z ?? 0f;

        /// <inheritdoc />
        public bool IsInLitFireRange
        {
            get
            {
                for (var i = 0; i < _fires.Count; i++)
                {
                    if (_fires[i].IsLit && _fires[i].PlayerInRange)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Pays for a structure and publishes <see cref="StructurePlaced"/>. The caller creates the scene object.
        /// </summary>
        /// <param name="spec">The structure to place.</param>
        /// <param name="x">World X of the placement.</param>
        /// <param name="z">World Z of the placement.</param>
        /// <returns>What happened; on a failure result nothing was consumed.</returns>
        public PlaceResult TryPlace(StructureSpec spec, float x, float z)
        {
            var result = CheckPlace(spec);
            if (result != PlaceResult.Placed || !_bag.TryRemoveAll(spec.Cost))
            {
                return result == PlaceResult.Placed ? PlaceResult.MissingCost : result;
            }

            _placedCounts.TryGetValue(spec.Id, out var placed);
            _placedCounts[spec.Id] = placed + 1;
            _bus.Publish(new StructurePlaced(spec.Id, x, z));
            return PlaceResult.Placed;
        }

        /// <summary>True when the structure has a count limit and that many are already placed in this run.</summary>
        /// <param name="spec">The structure.</param>
        public bool IsLimitReached(StructureSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return spec.MaxCount > 0 && _placedCounts.TryGetValue(spec.Id, out var placed) && placed >= spec.MaxCount;
        }

        /// <summary>What <see cref="TryPlace"/> would return right now, without changing anything.</summary>
        /// <param name="spec">The structure.</param>
        public PlaceResult CheckPlace(StructureSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            if (IsLimitReached(spec))
            {
                return PlaceResult.LimitReached;
            }

            if (!HasAll(spec.RequiredItems))
            {
                return PlaceResult.MissingRequiredItem;
            }

            for (var i = 0; i < spec.Cost.Count; i++)
            {
                if (_bag.Count(spec.Cost[i].Item) < spec.Cost[i].Amount)
                {
                    return PlaceResult.MissingCost;
                }
            }

            return PlaceResult.Placed;
        }

        /// <summary>Registers a newly placed, unlit campfire.</summary>
        /// <param name="spec">Its rule values.</param>
        /// <param name="x">World X where it stands.</param>
        /// <param name="y">World Y where it stands.</param>
        /// <param name="z">World Z where it stands.</param>
        /// <returns>The fire's state object.</returns>
        public Campfire AddCampfire(CampfireSpec spec, float x, float y, float z)
        {
            var fire = new Campfire(_fires.Count + 1, spec, x, y, z);
            _fires.Add(fire);
            return fire;
        }

        /// <summary>What <see cref="TryFuel"/> would do right now, without changing anything.</summary>
        /// <param name="fire">The campfire.</param>
        public FireActionResult CheckFuel(Campfire fire)
        {
            if (fire == null)
            {
                throw new ArgumentNullException(nameof(fire));
            }

            var spec = fire.Spec;
            if (fire.State != FireState.Lit)
            {
                if (!HasAll(spec.IgniterItems))
                {
                    return FireActionResult.MissingIgniter;
                }

                var cost = fire.State == FireState.BurnedOut ? spec.RelightCost : spec.IgniteCost;
                for (var i = 0; i < cost.Count; i++)
                {
                    if (_bag.Count(cost[i].Item) < cost[i].Amount)
                    {
                        return fire.State == FireState.BurnedOut
                            ? FireActionResult.MissingRelightItems
                            : FireActionResult.MissingIgniteItems;
                    }
                }

                return fire.State == FireState.BurnedOut ? FireActionResult.Relit : FireActionResult.Ignited;
            }

            if (fire.FuelSeconds + spec.BurnSecondsPerFuel > fire.FuelCapacitySeconds)
            {
                return FireActionResult.FuelFull;
            }

            if (_bag.Count(spec.FuelItem) < 1)
            {
                return FireActionResult.NoFuelItem;
            }

            return FireActionResult.FuelAdded;
        }

        /// <summary>True when the E action on this fire would do something.</summary>
        /// <param name="fire">The campfire.</param>
        public bool CanFuel(Campfire fire)
        {
            var result = CheckFuel(fire);
            return result == FireActionResult.FuelAdded || result == FireActionResult.Ignited || result == FireActionResult.Relit;
        }

        /// <summary>
        /// The E action on a campfire: add one fuel item (lighting a new fire), or relight a burned-out one.
        /// Publishes <see cref="FireStateChanged"/> on success.
        /// </summary>
        /// <param name="fire">The campfire.</param>
        /// <returns>What happened; on a failure result nothing was consumed.</returns>
        public FireActionResult TryFuel(Campfire fire)
        {
            var result = CheckFuel(fire);
            var spec = fire.Spec;
            switch (result)
            {
                case FireActionResult.Relit:
                case FireActionResult.Ignited:
                    var cost = result == FireActionResult.Relit ? spec.RelightCost : spec.IgniteCost;
                    if (!_bag.TryRemoveAll(cost))
                    {
                        return result == FireActionResult.Relit
                            ? FireActionResult.MissingRelightItems
                            : FireActionResult.MissingIgniteItems;
                    }

                    var fuelItems = 0;
                    for (var i = 0; i < cost.Count; i++)
                    {
                        if (cost[i].Item == spec.FuelItem)
                        {
                            fuelItems += cost[i].Amount;
                        }
                    }

                    fire.FuelSeconds = Math.Min(fuelItems * spec.BurnSecondsPerFuel, fire.FuelCapacitySeconds);
                    break;
                case FireActionResult.FuelAdded:
                    if (!_bag.TryRemove(spec.FuelItem, 1))
                    {
                        return FireActionResult.NoFuelItem;
                    }

                    fire.FuelSeconds += spec.BurnSecondsPerFuel;
                    break;
                default:
                    return result;
            }

            fire.State = fire.FuelSeconds > 0f ? FireState.Lit : FireState.BurnedOut;
            _bus.Publish(new FireStateChanged(fire.Id, fire.IsLit, fire.FuelSeconds));
            return result;
        }

        /// <summary>Burns fuel in every lit fire; a fire that runs out goes out and publishes <see cref="FireStateChanged"/>.</summary>
        /// <param name="dt">Elapsed seconds; zero or negative is ignored.</param>
        public void Tick(float dt)
        {
            if (dt <= 0f)
            {
                return;
            }

            for (var i = 0; i < _fires.Count; i++)
            {
                var fire = _fires[i];
                if (!fire.IsLit)
                {
                    continue;
                }

                fire.FuelSeconds = Math.Max(fire.FuelSeconds - dt, 0f);
                if (fire.FuelSeconds <= 0f)
                {
                    fire.State = FireState.BurnedOut;
                    _bus.Publish(new FireStateChanged(fire.Id, false, 0f));
                }
            }
        }

        private bool HasAll(IReadOnlyList<ItemId> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (_bag.Count(items[i]) < 1)
                {
                    return false;
                }
            }

            return true;
        }

        private Campfire FindNearby()
        {
            Campfire nearest = null;
            for (var i = 0; i < _fires.Count; i++)
            {
                if (_fires[i].PlayerNearby && (nearest == null || _fires[i].PlayerDistance < nearest.PlayerDistance))
                {
                    nearest = _fires[i];
                }
            }

            return nearest;
        }
    }
}
