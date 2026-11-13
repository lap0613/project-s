namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// One kind of E-key action (gather, cook, fuel a fire, rest). Systems implement it; the Game assembly's director
    /// picks the highest <see cref="Priority"/> source that has a target, shows its prompt, holds E for the offered
    /// time and locks movement meanwhile. Sources decide what is "in range" (View side, tech-decisions item 13).
    /// </summary>
    /// <example>
    /// <code>
    /// if (source.TryOffer(out var hold) &amp;&amp; source.Begin()) { /* hold E for 'hold' seconds, then source.Complete() */ }
    /// </code>
    /// </example>
    public interface IInteractionSource
    {
        /// <summary>Higher wins when several sources have a target at once.</summary>
        int Priority { get; }

        /// <summary>Prompt for the target remembered by the last successful <see cref="TryOffer"/>.</summary>
        InteractionPrompt Prompt { get; }

        /// <summary>Looks for a target right now and remembers it. Called every frame while idle.</summary>
        /// <param name="holdSeconds">How long E must be held; 0 or less means "until released" (resting).</param>
        /// <returns>True when there is a target.</returns>
        bool TryOffer(out float holdSeconds);

        /// <summary>Starts the action on the remembered target.</summary>
        /// <returns>False when refused (e.g. bag full); the source posts the reason as a notice.</returns>
        bool Begin();

        /// <summary>True while the target is still valid and in range; polled every frame while held.</summary>
        bool IsStillAvailable();

        /// <summary>The hold time finished: apply the result.</summary>
        void Complete();

        /// <summary>E was released, the target was lost, or the player died: undo anything started by <see cref="Begin"/>.</summary>
        void Cancel();
    }
}
