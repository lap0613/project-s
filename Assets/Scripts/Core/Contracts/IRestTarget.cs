namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Receives "the player is resting in a shelter" (core-rules section 2). Implemented by Survival, set by Camp.
    /// </summary>
    /// <example>
    /// <code>
    /// rest.SetResting(true);
    /// </code>
    /// </example>
    public interface IRestTarget
    {
        /// <summary>Starts or stops resting.</summary>
        /// <param name="resting">True while the player rests inside a shelter.</param>
        void SetResting(bool resting);
    }
}
