namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Where the mouse cursor points in the world (design/ux/ui-plan.md sections 8.1 and 9). Implemented in the Game
    /// assembly from the camera; Camp and Gathering read it without knowing about cameras or UI.
    /// </summary>
    /// <example>
    /// <code>
    /// if (pointer.TryGetPointOnPlane(player.Y, out var x, out var z)) { preview.MoveTo(x, z); }
    /// </code>
    /// </example>
    public interface IPointerWorld
    {
        /// <summary>True when the cursor exists and is not over a HUD element.</summary>
        bool IsOverWorld { get; }

        /// <summary>True when the cursor is over a HUD element (bag slot, menu, button).</summary>
        bool IsOverUi { get; }

        /// <summary>Where the cursor ray crosses a horizontal plane.</summary>
        /// <param name="planeY">World Y of the plane.</param>
        /// <param name="x">World X of the crossing.</param>
        /// <param name="z">World Z of the crossing.</param>
        /// <returns>False when there is no cursor or the ray never reaches the plane.</returns>
        bool TryGetPointOnPlane(float planeY, out float x, out float z);
    }
}
