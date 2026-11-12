namespace Whiteout.Player
{
    /// <summary>A 2D vector on the ground plane (x, z), engine-free. Used for velocities and points.</summary>
    /// <example>
    /// <code>
    /// var v = new PlanarVector(1f, 0f);
    /// </code>
    /// </example>
    public readonly struct PlanarVector
    {
        /// <summary>Creates a vector.</summary>
        /// <param name="x">X component.</param>
        /// <param name="z">Z component.</param>
        public PlanarVector(float x, float z)
        {
            X = x;
            Z = z;
        }

        /// <summary>X component.</summary>
        public float X { get; }

        /// <summary>Z component.</summary>
        public float Z { get; }
    }
}
