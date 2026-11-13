namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The player's world position and facing, so systems do not reference the Player system. Meters, Unity axes.
    /// </summary>
    /// <example>
    /// <code>
    /// float dx = pickupX - player.X;
    /// float previewX = player.X + player.ForwardX * distance;
    /// </code>
    /// </example>
    public interface IPlayerPosition
    {
        /// <summary>World X.</summary>
        float X { get; }

        /// <summary>World Y (feet).</summary>
        float Y { get; }

        /// <summary>World Z.</summary>
        float Z { get; }

        /// <summary>X of the unit facing direction on the ground plane (last move direction).</summary>
        float ForwardX { get; }

        /// <summary>Z of the unit facing direction on the ground plane (last move direction).</summary>
        float ForwardZ { get; }
    }
}
