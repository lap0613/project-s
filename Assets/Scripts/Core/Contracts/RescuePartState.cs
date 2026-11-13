namespace Whiteout.Core.Contracts
{
    /// <summary>Where one beacon part is, for the goal display (design/ux/ui-plan.md section 7.3 [C], N1).</summary>
    public enum RescuePartState
    {
        /// <summary>The player does not have the part.</summary>
        Missing,

        /// <summary>The part is in the bag.</summary>
        Held,

        /// <summary>The part is installed on the signal beacon.</summary>
        Installed,
    }
}
