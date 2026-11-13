namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// What the E key would do, or is doing, right now. Implemented by the Game assembly's interaction director,
    /// read by the world UI (design/ux/ui-plan.md section 8).
    /// </summary>
    /// <example>
    /// <code>
    /// if (status.HasPrompt) { Show(status.Prompt); }
    /// if (status.IsHolding) { bar.fill = status.Progress; }
    /// </code>
    /// </example>
    public interface IInteractionStatus
    {
        /// <summary>True when an E-action target exists (idle or in progress).</summary>
        bool HasPrompt { get; }

        /// <summary>Prompt of the current target; meaningful only while <see cref="HasPrompt"/>.</summary>
        InteractionPrompt Prompt { get; }

        /// <summary>True while E is held on a timed action (gather, cook, fuel).</summary>
        bool IsHolding { get; }

        /// <summary>Hold progress from 0 to 1; 0 when not holding.</summary>
        float Progress { get; }
    }
}
