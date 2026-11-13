using System;
using Whiteout.Core.Contracts;
using Whiteout.Input;

namespace Whiteout.Game
{
    /// <summary>
    /// The <see cref="IPlayerInput"/> the player actually reads: passes the real move input through, or reports no
    /// movement while <see cref="MovementLocked"/> (gathering, cooking, fueling, resting; core-rules section 4),
    /// or walks a fixed direction while an auto-move is set (clicking a far target, ui-plan section 8.1 M4). The real
    /// keyboard input stays readable through <see cref="HasManualInput"/> so the walk can be cancelled by WASD.
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

        private float _autoX;
        private float _autoZ;

        /// <summary>While true the move axes read 0.</summary>
        public bool MovementLocked { get; set; }

        /// <summary>True while an auto-move direction is set.</summary>
        public bool IsAutoMoving { get; private set; }

        /// <summary>True when the player is pressing a movement key right now, whatever the lock or auto-move says.</summary>
        public bool HasManualInput => _inner.MoveX != 0f || _inner.MoveZ != 0f;

        /// <inheritdoc />
        public float MoveX => MovementLocked ? 0f : IsAutoMoving ? _autoX : _inner.MoveX;

        /// <inheritdoc />
        public float MoveZ => MovementLocked ? 0f : IsAutoMoving ? _autoZ : _inner.MoveZ;

        /// <summary>Makes the player walk in a direction until <see cref="ClearAutoMove"/>.</summary>
        /// <param name="x">Direction X (any length; it is normalized).</param>
        /// <param name="z">Direction Z (any length; it is normalized).</param>
        public void SetAutoMove(float x, float z)
        {
            var length = (float)Math.Sqrt(x * x + z * z);
            if (length <= 0f)
            {
                ClearAutoMove();
                return;
            }

            _autoX = x / length;
            _autoZ = z / length;
            IsAutoMoving = true;
        }

        /// <summary>Stops the auto-move; the keyboard drives the player again.</summary>
        public void ClearAutoMove()
        {
            IsAutoMoving = false;
            _autoX = 0f;
            _autoZ = 0f;
        }
    }
}
