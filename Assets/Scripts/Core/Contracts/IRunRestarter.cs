namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Starts a new run by loading the Game scene again (docs/architecture/tech-decisions.md 12 and 34, R1).
    /// Implemented in the Bootstrap scene and registered in the Bootstrap scope, so the Game scene can call it.
    /// </summary>
    /// <example>
    /// <code>
    /// restartButton.onClick.AddListener(restarter.Restart);
    /// </code>
    /// </example>
    public interface IRunRestarter
    {
        /// <summary>Reloads the Game scene. A call made while a reload is running is ignored.</summary>
        void Restart();
    }
}
