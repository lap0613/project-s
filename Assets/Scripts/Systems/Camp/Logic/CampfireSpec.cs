using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp
{
    /// <summary>
    /// Campfire rule values, built from a data asset (core-rules sections 3 and 4): 1 wood = 60 s, at most 5 wood,
    /// +35 degrees, relight with tinder 1 + wood 2 while holding flint. The warmth radius is not here: the View
    /// measures distance and reports only "in range" (tech-decisions item 13).
    /// </summary>
    /// <example>
    /// <code>
    /// var spec = new CampfireSpec(wood, 60f, 5, 35f, relightCost, new[] { flint });
    /// </code>
    /// </example>
    public sealed class CampfireSpec
    {
        /// <summary>Creates the spec.</summary>
        /// <param name="fuelItem">Item burned as fuel.</param>
        /// <param name="burnSecondsPerFuel">Burning time one fuel item gives.</param>
        /// <param name="maxFuel">Most fuel items the fire holds.</param>
        /// <param name="warmth">Temperature bonus in degrees C while lit and in range.</param>
        /// <param name="relightCost">Items consumed to relight a burned-out fire; the fuel items among them become fuel.</param>
        /// <param name="igniterItems">Items that must be in the bag to relight but are not consumed (flint).</param>
        public CampfireSpec(
            ItemId fuelItem,
            float burnSecondsPerFuel,
            int maxFuel,
            float warmth,
            IReadOnlyList<ItemAmount> relightCost,
            IReadOnlyList<ItemId> igniterItems)
        {
            FuelItem = fuelItem;
            BurnSecondsPerFuel = burnSecondsPerFuel;
            MaxFuel = maxFuel;
            Warmth = warmth;
            RelightCost = relightCost ?? throw new ArgumentNullException(nameof(relightCost));
            IgniterItems = igniterItems ?? throw new ArgumentNullException(nameof(igniterItems));
        }

        /// <summary>Item burned as fuel.</summary>
        public ItemId FuelItem { get; }

        /// <summary>Burning time one fuel item gives, in seconds.</summary>
        public float BurnSecondsPerFuel { get; }

        /// <summary>Most fuel items the fire holds.</summary>
        public int MaxFuel { get; }

        /// <summary>Temperature bonus in degrees C while lit and in range.</summary>
        public float Warmth { get; }

        /// <summary>Items consumed to relight a burned-out fire.</summary>
        public IReadOnlyList<ItemAmount> RelightCost { get; }

        /// <summary>Items that must be in the bag to relight but are not consumed.</summary>
        public IReadOnlyList<ItemId> IgniterItems { get; }
    }
}
