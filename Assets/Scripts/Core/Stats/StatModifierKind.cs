namespace Whiteout.Core.Stats
{
    /// <summary>How a <see cref="StatModifier"/> combines into a <see cref="Stat"/>.</summary>
    public enum StatModifierKind
    {
        /// <summary>Added to the base value.</summary>
        Add,

        /// <summary>Percent-style factor. All Multiply values on a stat are summed, then applied once.</summary>
        Multiply,
    }
}
