namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Button input for interacting, the placement mode and the bag (design/ux/ui-plan.md D3, D4).
    /// Movement is <see cref="IPlayerInput"/>. "Pressed" values are true only in the frame the button went down.
    /// Views read this instead of devices, so a gamepad mapping can be added in the Input assembly alone.
    /// </summary>
    /// <example>
    /// <code>
    /// if (input.InteractPressed) { StartHolding(); }
    /// </code>
    /// </example>
    public interface IInteractionInput
    {
        /// <summary>True in the frame the interact button (E) went down.</summary>
        bool InteractPressed { get; }

        /// <summary>True while the interact button is held.</summary>
        bool InteractHeld { get; }

        /// <summary>True in the frame the placement-mode button (B) went down.</summary>
        bool PlacementPressed { get; }

        /// <summary>True in the frame the confirm button (left click) went down.</summary>
        bool ConfirmPressed { get; }

        /// <summary>True in the frame a cancel button (right click or Esc) went down.</summary>
        bool CancelPressed { get; }

        /// <summary>Zero-based index of the number key (1 = 0 ... 9 = 8) that went down this frame, or -1 when none.</summary>
        int NumberPressed { get; }

        /// <summary>Selection scroll this frame: +1 next, -1 previous, 0 none (mouse wheel).</summary>
        int SelectionScroll { get; }
    }
}
