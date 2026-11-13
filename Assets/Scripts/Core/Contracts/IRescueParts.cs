namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The beacon parts and where each one is, for the goal display (design/ux/ui-plan.md section 7.3 [C]).
    /// Implemented by Camp from the bag and the beacon, read by the HUD. It says nothing about where or when parts
    /// appear (ui-plan D8).
    /// </summary>
    /// <example>
    /// <code>
    /// label.text = $"{parts.Collected}/{parts.Required}";
    /// bool installed = parts.GetPartState(0) == RescuePartState.Installed;
    /// </code>
    /// </example>
    public interface IRescueParts
    {
        /// <summary>Parts the player holds or has installed.</summary>
        int Collected { get; }

        /// <summary>Parts the beacon needs.</summary>
        int Required { get; }

        /// <summary>Where one part is.</summary>
        /// <param name="index">Part index in install order, 0 to <see cref="Required"/> - 1.</param>
        RescuePartState GetPartState(int index);
    }
}
