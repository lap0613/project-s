namespace Whiteout.Camp
{
    /// <summary>Outcome of the E action on the signal beacon (core-rules section 1, tech-decisions 34).</summary>
    public enum BeaconActionResult
    {
        /// <summary>One part moved from the bag onto the beacon.</summary>
        PartInstalled,

        /// <summary>The beacon still needs parts and the bag holds none of them.</summary>
        MissingPart,

        /// <summary>The beacon was complete and rescue is now requested.</summary>
        RescueRequested,

        /// <summary>Rescue was already requested; nothing is left to do.</summary>
        NothingToDo,

        /// <summary>No beacon is placed yet.</summary>
        NoBeacon,
    }
}
