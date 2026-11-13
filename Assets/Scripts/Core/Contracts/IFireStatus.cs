namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// What the player can currently see and use of campfires. Implemented by Camp, read by the world UI
    /// (fuel pips above the nearest fire, design/ux/ui-plan.md section 8) and by Resources (cooking needs a lit fire).
    /// </summary>
    /// <example>
    /// <code>
    /// if (fire.IsInLitFireRange) { /* cooking allowed */ }
    /// if (fire.HasNearbyFire) { DrawPips(fire.NearbyFuelSeconds / fire.NearbySecondsPerFuel, fire.NearbyMaxFuel); }
    /// </code>
    /// </example>
    public interface IFireStatus
    {
        /// <summary>True when a campfire is close enough to show its status.</summary>
        bool HasNearbyFire { get; }

        /// <summary>True when the nearest nearby campfire is burning.</summary>
        bool NearbyFireLit { get; }

        /// <summary>Burning time left in the nearest nearby campfire, in seconds.</summary>
        float NearbyFuelSeconds { get; }

        /// <summary>Burning time one fuel item gives the nearest nearby campfire, in seconds.</summary>
        float NearbySecondsPerFuel { get; }

        /// <summary>Most fuel items the nearest nearby campfire holds.</summary>
        int NearbyMaxFuel { get; }

        /// <summary>World X of the nearest nearby campfire.</summary>
        float NearbyFireX { get; }

        /// <summary>World Y of the nearest nearby campfire.</summary>
        float NearbyFireY { get; }

        /// <summary>World Z of the nearest nearby campfire.</summary>
        float NearbyFireZ { get; }

        /// <summary>True when the player stands inside the warmth radius of a lit campfire.</summary>
        bool IsInLitFireRange { get; }
    }
}
