using System;
using UnityEngine.InputSystem;
using Whiteout.Core.Contracts;

namespace Whiteout.Input
{
    /// <summary>
    /// <see cref="IPlayerInput"/> backed by the Input System action "Player/Move".
    /// Enables the action on construction and disables it on <see cref="Dispose"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// using var input = new InputSystemPlayerInput(inputActionsAsset);
    /// float x = input.MoveX;
    /// </code>
    /// </example>
    public sealed class InputSystemPlayerInput : IPlayerInput, IDisposable
    {
        private const string MoveActionPath = "Player/Move";

        private readonly InputAction _move;

        /// <summary>Creates the input and enables the Move action.</summary>
        /// <param name="actions">The action asset (e.g. InputSystem_Actions).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="actions"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the asset has no "Player/Move" action.</exception>
        public InputSystemPlayerInput(InputActionAsset actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            _move = actions.FindAction(MoveActionPath, throwIfNotFound: true);
            _move.Enable();
        }

        /// <inheritdoc />
        public float MoveX => _move.ReadValue<UnityEngine.Vector2>().x;

        /// <inheritdoc />
        public float MoveZ => _move.ReadValue<UnityEngine.Vector2>().y;

        /// <summary>Disables the Move action.</summary>
        public void Dispose()
        {
            _move.Disable();
        }
    }
}
