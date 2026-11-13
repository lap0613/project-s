using System;
using VContainer.Unity;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Number keys select a bag slot (key 1 = first slot; ui-plan D3). While the placement mode is open the number
    /// keys belong to its list instead (ui-plan section 9). Registered as a VContainer entry point.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;BagSelectionRunner&gt;();
    /// </code>
    /// </example>
    public sealed class BagSelectionRunner : ITickable
    {
        private readonly IInteractionInput _input;
        private readonly IBag _bag;
        private readonly IPlacementMenu _placement;

        /// <summary>Creates the runner.</summary>
        /// <param name="input">Number key input.</param>
        /// <param name="bag">The bag whose slot is selected.</param>
        /// <param name="placement">Tells whether the placement mode owns the number keys.</param>
        public BagSelectionRunner(IInteractionInput input, IBag bag, IPlacementMenu placement)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _placement = placement ?? throw new ArgumentNullException(nameof(placement));
        }

        /// <inheritdoc />
        public void Tick()
        {
            var number = _input.NumberPressed;
            if (number >= 0 && !_placement.IsOpen)
            {
                _bag.SelectSlot(number);
            }
        }
    }
}
