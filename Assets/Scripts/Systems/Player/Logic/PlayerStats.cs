using Whiteout.Core.Stats;

namespace Whiteout.Player
{
    /// <summary>Stat ids owned by the Player system.</summary>
    /// <example>
    /// <code>
    /// var speed = new Stat(PlayerStats.MoveSpeed, 4f);
    /// </code>
    /// </example>
    public static class PlayerStats
    {
        /// <summary>Move speed in meters per second.</summary>
        public static readonly StatId MoveSpeed = new StatId("player.move_speed");
    }
}
