namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The left click that nothing else used (ui-plan section 8.1, M1): not a HUD click, not a placement, not an
    /// interaction target. The combat story (story-007) reads it as the attack input. Implemented by the Game
    /// assembly's interaction director.
    /// </summary>
    /// <example>
    /// <code>
    /// if (attack.AttackClicked) { SwingToward(pointerX, pointerZ); }
    /// </code>
    /// </example>
    public interface IAttackInput
    {
        /// <summary>True only in the frame the unused left click went down.</summary>
        bool AttackClicked { get; }
    }
}
