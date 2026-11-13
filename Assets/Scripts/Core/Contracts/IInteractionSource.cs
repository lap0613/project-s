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

        /// <summary>
        /// Looks for a target under the mouse cursor and remembers it, like <see cref="TryOffer"/> but chosen by the cursor
        /// instead of by the player's distance (ui-plan section 8.1). Only targets within <see cref="PointerQuery.MaxPlayerDistance"/>
        /// of the player count. It may overwrite the target <see cref="TryOffer"/> remembered, so the director calls it
        /// again right before <see cref="Begin"/> on a click.
        /// </summary>
        /// <param name="query">Cursor and ranges.</param>
        /// <param name="holdSeconds">Time the action takes; 0 or less means "until the player moves" (resting).</param>
        /// <returns>True when the cursor is on a target of this source.</returns>
        bool TryOfferPointed(in PointerQuery query, out float holdSeconds);

        /// <summary>True when the player is already close enough to the remembered target to start the action (the E range).</summary>
        bool IsTargetInReach();

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
