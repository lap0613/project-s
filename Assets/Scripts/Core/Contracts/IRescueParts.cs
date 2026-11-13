namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// How many beacon parts the player has found, for the goal display (design/ux/ui-plan.md section 7.3 [C]).
    /// Implemented by the rescue system (story-006), read by the HUD. It says nothing about where or when parts
    /// appear (ui-plan D8).
    /// </summary>
    /// <example>
    /// <code>
    /// label.text = $"{parts.Collected}/{parts.Required}";
    /// </code>
    /// </example>
    public interface IRescueParts
    {
        /// <summary>Parts the player has collected so far.</summary>
        int Collected { get; }

        /// <summary>Parts the beacon needs.</summary>
        int Required { get; }
    }
}
