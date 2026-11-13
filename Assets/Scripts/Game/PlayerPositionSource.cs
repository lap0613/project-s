using System;
using UnityEngine;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Player.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// <see cref="IPlayerPosition"/> read from the player object, so Resources and Camp do not reference the Player
    /// system (code-structure section 4). Facing is the last move direction, which is also where the model looks;
    /// before the first move it is +Z.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;PlayerPositionSource&gt;().As&lt;IPlayerPosition&gt;();
    /// </code>
    /// </example>
    public sealed class PlayerPositionSource : IPlayerPosition, ITickable
    {
        private readonly Transform _player;
        private readonly IPlayerInput _input;
        private readonly float _feetOffset;
        private Vector2 _forward = Vector2.up;

        /// <summary>Creates the source.</summary>
        /// <param name="player">The player object.</param>
        /// <param name="input">Move input (decides the facing).</param>
        public PlayerPositionSource(PlayerController player, IPlayerInput input)
        {
            // Unity's == also catches a destroyed object, which ?? would let through.
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            _player = player.transform;
            var body = player.GetComponent<CharacterController>();
            _feetOffset = body.center.y - body.height * 0.5f;
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <inheritdoc />
        public float X => _player.position.x;

        /// <inheritdoc />
        public float Y => _player.position.y + _feetOffset;

        /// <inheritdoc />
        public float Z => _player.position.z;

        /// <inheritdoc />
        public float ForwardX => _forward.x;

        /// <inheritdoc />
        public float ForwardZ => _forward.y;

        /// <inheritdoc />
        public void Tick()
        {
            var move = new Vector2(_input.MoveX, _input.MoveZ);
            if (move.sqrMagnitude > 0f)
            {
                _forward = move.normalized;
            }
        }
    }
}
