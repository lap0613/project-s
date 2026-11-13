namespace Whiteout.Camp
{
    /// <summary>Where a campfire is in its life (core-rules section 4).</summary>
    public enum FireState
    {
        /// <summary>Just placed, never lit. The first wood put in lights it.</summary>
        Unlit,

        /// <summary>Burning: gives warmth and allows cooking.</summary>
        Lit,

        /// <summary>Ran out of fuel. Needs relighting (tinder 1 + wood 2, flint in the bag).</summary>
        BurnedOut,
    }
}
