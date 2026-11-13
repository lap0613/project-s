using System;
using Whiteout.Core.Contracts;
using Whiteout.Input;

namespace Whiteout.Game
{
    /// <summary>
    /// The <see cref="IPlayerInput"/> the player actually reads: passes the real move input through, or reports no
    /// movement while <see cref="MovementLocked"/> (gathering, cooking, fueling, resting; core-rules section 4).
    /// Keeps the Player system unaware of interactions.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;InputSystemPlayerInput&gt;(Lifetime.Singleton);
    /// builder.Register&lt;PlayerActionLock&gt;(Lifetime.Singleton).AsSelf().As&lt;IPlayerInput&gt;();
    /// </code>
    /// </example>
    public sealed class PlayerActionLock : IPlayerInput
    {
        private readonly InputSystemPlayerInput _inner;

        /// <summary>Creates the wrapper.</summary>
        /// <param name="inner">The real move input.</param>
        public PlayerActionLock(InputSystemPlayerInput inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>While true the move axes read 0.</summary>
        public bool MovementLocked { get; set; }

        /// <inheritdoc />
        public float MoveX => MovementLocked ? 0f : _inner.MoveX;

        /// <inheritdoc />
        public float MoveZ => MovementLocked ? 0f : _inner.MoveZ;
    }
}
