using System;
using UnityEngine.InputSystem;
using Whiteout.Core.Contracts;

namespace Whiteout.Input
{
    /// <summary>
    /// <see cref="IInteractionInput"/> backed by Input System actions created here: E interact, B placement mode,
    /// left click confirm, right click / Esc cancel, number keys 1-9, mouse wheel, cursor position. Bindings are fixed for now;
    /// a gamepad mapping is added by giving these actions more bindings (tech-decisions items 5 and 24).
    /// Enables the actions on construction and disables them on <see cref="Dispose"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;InputSystemInteractionInput&gt;(Lifetime.Singleton).As&lt;IInteractionInput&gt;();
    /// </code>
    /// </example>
    public sealed class InputSystemInteractionInput : IInteractionInput, IDisposable
    {
        private const int NumberKeyCount = 9;
        private const string NumberBindingFormat = "<Keyboard>/{0}";

        private readonly InputAction _interact = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
        private readonly InputAction _placement = new InputAction("Placement", InputActionType.Button, "<Keyboard>/b");
        private readonly InputAction _confirm = new InputAction("Confirm", InputActionType.Button, "<Mouse>/leftButton");
        private readonly InputAction _cancel = new InputAction("Cancel", InputActionType.Button, "<Mouse>/rightButton");
        private readonly InputAction _scroll = new InputAction("WheelScroll", InputActionType.Value, "<Mouse>/scroll/y");
        private readonly InputAction[] _numbers = new InputAction[NumberKeyCount];

        /// <summary>Creates the actions and enables them.</summary>
        public InputSystemInteractionInput()
        {
            _cancel.AddBinding("<Keyboard>/escape");
            for (var i = 0; i < NumberKeyCount; i++)
            {
                _numbers[i] = new InputAction("Number" + (i + 1), InputActionType.Button, string.Format(NumberBindingFormat, i + 1));
                _numbers[i].Enable();
            }

            _interact.Enable();
            _placement.Enable();
            _confirm.Enable();
            _cancel.Enable();
            _scroll.Enable();
        }

        /// <inheritdoc />
        public bool InteractPressed => _interact.WasPressedThisFrame();

        /// <inheritdoc />
        public bool InteractHeld => _interact.IsPressed();

        /// <inheritdoc />
        public bool PlacementPressed => _placement.WasPressedThisFrame();

        /// <inheritdoc />
        public bool ConfirmPressed => _confirm.WasPressedThisFrame();

        /// <inheritdoc />
        public bool CancelPressed => _cancel.WasPressedThisFrame();

        /// <inheritdoc />
        public int NumberPressed
        {
            get
            {
                for (var i = 0; i < NumberKeyCount; i++)
                {
                    if (_numbers[i].WasPressedThisFrame())
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <inheritdoc />
        public int WheelScroll
        {
            get
            {
                var value = _scroll.ReadValue<float>();
                return value > 0f ? 1 : value < 0f ? -1 : 0;
            }
        }

        /// <inheritdoc />
        public bool HasPointer => Mouse.current != null;

        /// <inheritdoc />
        public float PointerX => Mouse.current != null ? Mouse.current.position.ReadValue().x : 0f;

        /// <inheritdoc />
        public float PointerY => Mouse.current != null ? Mouse.current.position.ReadValue().y : 0f;

        /// <summary>Disables and releases the actions.</summary>
        public void Dispose()
        {
            _interact.Dispose();
            _placement.Dispose();
            _confirm.Dispose();
            _cancel.Dispose();
            _scroll.Dispose();
            for (var i = 0; i < NumberKeyCount; i++)
            {
                _numbers[i].Dispose();
            }
        }
    }
}
