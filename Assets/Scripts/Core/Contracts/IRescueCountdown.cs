namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The rescue timer, for the HUD (design/ux/ui-plan.md section 7.3 [C]). Implemented by RunFlow, which starts it
    /// on <see cref="RescueRequested"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// if (countdown.IsActive) { ShowSeconds(countdown.RemainingSeconds); }
    /// </code>
    /// </example>
    public interface IRescueCountdown
    {
        /// <summary>True once rescue has been requested.</summary>
        bool IsActive { get; }

        /// <summary>Game seconds until rescue arrives; 0 before the request and after the arrival.</summary>
        float RemainingSeconds { get; }
    }
}
