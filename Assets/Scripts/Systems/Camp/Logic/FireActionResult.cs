namespace Whiteout.Camp
{
    /// <summary>Outcome of the E action on a campfire.</summary>
    public enum FireActionResult
    {
        /// <summary>One fuel item was added to a burning fire.</summary>
        FuelAdded,

        /// <summary>The first fuel item lit a newly placed fire.</summary>
        Ignited,

        /// <summary>A burned-out fire was relit.</summary>
        Relit,

        /// <summary>The fire cannot hold another fuel item.</summary>
        FuelFull,

        /// <summary>No fuel item in the bag.</summary>
        NoFuelItem,

        /// <summary>The igniter (flint) is not in the bag.</summary>
        MissingIgniter,

        /// <summary>The relight cost is not in the bag.</summary>
        MissingRelightItems,

        /// <summary>A new fire cannot be lit: the ignite items are missing.</summary>
        MissingIgniteItems,
    }
}
