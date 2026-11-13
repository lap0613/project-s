namespace Whiteout.Camp
{
    /// <summary>Outcome of trying to place a structure.</summary>
    public enum PlaceResult
    {
        /// <summary>Paid and placed.</summary>
        Placed,

        /// <summary>An item that must be in the bag (flint) is missing.</summary>
        MissingRequiredItem,

        /// <summary>Part of the cost is missing.</summary>
        MissingCost,
    }
}
