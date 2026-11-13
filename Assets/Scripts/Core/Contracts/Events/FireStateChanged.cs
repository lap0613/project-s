namespace Whiteout.Core.Contracts
{
    /// <summary>Published by Camp when a campfire lights, burns out, or gets fuel.</summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;FireStateChanged&gt;(e =&gt; RefreshFire(e.FireId, e.IsLit));
    /// </code>
    /// </example>
    public readonly struct FireStateChanged
    {
        /// <summary>Creates the event.</summary>
        /// <param name="fireId">Campfire number, unique within a run.</param>
        /// <param name="isLit">True when the fire is burning after the change.</param>
        /// <param name="fuelSeconds">Burning time left after the change.</param>
        public FireStateChanged(int fireId, bool isLit, float fuelSeconds)
        {
            FireId = fireId;
            IsLit = isLit;
            FuelSeconds = fuelSeconds;
        }

        /// <summary>Campfire number, unique within a run.</summary>
        public int FireId { get; }

        /// <summary>True when the fire is burning after the change.</summary>
        public bool IsLit { get; }

        /// <summary>Burning time left after the change, in seconds.</summary>
        public float FuelSeconds { get; }
    }
}
