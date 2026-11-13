namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// What an <see cref="IInteractionSource"/> needs to find the target under the cursor: the cursor, how far from
    /// the player a target may be (M3) and how close the cursor must be to a target to count as on it.
    /// </summary>
    /// <example>
    /// <code>
    /// var query = new PointerQuery(pointer, 6f, 0.7f);
    /// </code>
    /// </example>
    public readonly struct PointerQuery
    {
        /// <summary>Creates the query.</summary>
        /// <param name="pointer">The cursor in the world.</param>
        /// <param name="maxPlayerDistance">Targets farther than this from the player are ignored, in meters.</param>
        /// <param name="hoverRadius">Cursor distance to a target, measured on the target's height plane, that counts as on it, in meters.</param>
        public PointerQuery(IPointerWorld pointer, float maxPlayerDistance, float hoverRadius)
        {
            Pointer = pointer;
            MaxPlayerDistance = maxPlayerDistance;
            HoverRadius = hoverRadius;
        }

        /// <summary>The cursor in the world.</summary>
        public IPointerWorld Pointer { get; }

        /// <summary>Targets farther than this from the player are ignored, in meters.</summary>
        public float MaxPlayerDistance { get; }

        /// <summary>Cursor distance to a target that counts as on it, in meters.</summary>
        public float HoverRadius { get; }
    }
}
