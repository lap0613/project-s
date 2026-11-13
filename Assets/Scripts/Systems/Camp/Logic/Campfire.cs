using System;

namespace Whiteout.Camp
{
    /// <summary>
    /// State of one placed campfire: fuel left and whether it burns. Created and changed by <see cref="CampSystem"/>.
    /// The View reports where the player is relative to it through <see cref="SetPlayerProximity"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Campfire fire = camp.AddCampfire(spec);
    /// fire.SetPlayerProximity(distance &lt;= warmthRadius, distance &lt;= statusRange, distance);
    /// </code>
    /// </example>
    public sealed class Campfire
    {
        internal Campfire(int id, CampfireSpec spec, float x, float y, float z)
        {
            Id = id;
            Spec = spec ?? throw new ArgumentNullException(nameof(spec));
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>World X where the fire stands (stored for display only).</summary>
        public float X { get; }

        /// <summary>World Y where the fire stands.</summary>
        public float Y { get; }

        /// <summary>World Z where the fire stands.</summary>
        public float Z { get; }

        /// <summary>Campfire number, unique within a run.</summary>
        public int Id { get; }

        /// <summary>Rule values of this fire.</summary>
        public CampfireSpec Spec { get; }

        /// <summary>Unlit, lit or burned out.</summary>
        public FireState State { get; internal set; }

        /// <summary>Burning time left, in seconds.</summary>
        public float FuelSeconds { get; internal set; }

        /// <summary>Most burning time the fire holds, in seconds.</summary>
        public float FuelCapacitySeconds => Spec.MaxFuel * Spec.BurnSecondsPerFuel;

        /// <summary>True while burning.</summary>
        public bool IsLit => State == FireState.Lit;

        /// <summary>True when the player is inside the warmth radius (set by the View).</summary>
        public bool PlayerInRange { get; private set; }

        /// <summary>True when the player is close enough to show this fire's status (set by the View).</summary>
        public bool PlayerNearby { get; private set; }

        /// <summary>Distance to the player in meters as measured by the View; used only to pick the nearest fire.</summary>
        public float PlayerDistance { get; private set; }

        /// <summary>Reports the player's position relative to this fire. The View measures; Logic only stores.</summary>
        /// <param name="inRange">Player is inside the warmth radius.</param>
        /// <param name="nearby">Player is close enough to show this fire's status.</param>
        /// <param name="distance">Measured distance in meters.</param>
        public void SetPlayerProximity(bool inRange, bool nearby, float distance)
        {
            PlayerInRange = inRange;
            PlayerNearby = nearby;
            PlayerDistance = distance;
        }
    }
}
