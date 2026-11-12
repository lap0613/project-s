using System;

namespace Whiteout.Player
{
    /// <summary>
    /// Turns move input into a planar velocity. Stateless and engine-free.
    /// Input magnitude is clamped to 1, so diagonals are not faster than straight moves.
    /// </summary>
    /// <example>
    /// <code>
    /// var logic = new PlayerMoveLogic();
    /// PlanarVector v = logic.ComputeVelocity(1f, 1f, 4f); // length 4
    /// </code>
    /// </example>
    public sealed class PlayerMoveLogic
    {
        /// <summary>Computes velocity in meters per second.</summary>
        /// <param name="inputX">Move axis X.</param>
        /// <param name="inputZ">Move axis Z.</param>
        /// <param name="speed">Speed in meters per second at full input.</param>
        /// <returns>Zero for zero (or NaN) input, otherwise input direction * speed * min(|input|, 1).</returns>
        public PlanarVector ComputeVelocity(float inputX, float inputZ, float speed)
        {
            var magnitude = (float)Math.Sqrt(inputX * inputX + inputZ * inputZ);
            if (!(magnitude > 0f))
            {
                return new PlanarVector(0f, 0f);
            }

            var scale = magnitude > 1f ? speed / magnitude : speed;
            return new PlanarVector(inputX * scale, inputZ * scale);
        }
    }
}
