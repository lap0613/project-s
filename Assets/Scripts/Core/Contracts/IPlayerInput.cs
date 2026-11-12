namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Player input as seen by gameplay. Axes are camera-aligned world axes:
    /// W moves toward +Z, D moves toward +X.
    /// </summary>
    /// <example>
    /// <code>
    /// float x = input.MoveX;
    /// float z = input.MoveZ;
    /// </code>
    /// </example>
    public interface IPlayerInput
    {
        /// <summary>Horizontal move axis in [-1, 1] (D = +1).</summary>
        float MoveX { get; }

        /// <summary>Vertical move axis in [-1, 1] (W = +1).</summary>
        float MoveZ { get; }
    }
}
