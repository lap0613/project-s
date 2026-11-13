using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Whiteout.UI
{
    /// <summary>
    /// One bag slot in the bottom row (ui-plan section 7.3 [G]): its text, its border, and pointer clicks.
    /// Left click selects the slot; right click opens the slot menu. Created and driven by <see cref="BagHud"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // On the slot prefab root (needs a raycast-target Graphic); assign Label and Border.
    /// slot.Bind(bagHud, 0);
    /// </code>
    /// </example>
    public sealed class BagSlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text label;

        [Tooltip("Outline graphic recolored when the slot is selected.")]
        [SerializeField] private Graphic border;

        private BagHud _owner;
        private int _index;

        /// <summary>The slot's text label.</summary>
        public TMP_Text Label => label;

        /// <summary>Connects the slot to its row.</summary>
        /// <param name="owner">The bag HUD.</param>
        /// <param name="index">Zero-based slot index.</param>
        public void Bind(BagHud owner, int index)
        {
            _owner = owner;
            _index = index;
        }

        /// <summary>Sets the slot's text and border color.</summary>
        /// <param name="text">Text to show.</param>
        /// <param name="borderColor">Border color (selected or not).</param>
        public void Show(string text, Color borderColor)
        {
            label.text = text;
            border.color = borderColor;
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _owner.OpenMenu(_index, eventData.position);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                _owner.Select(_index);
            }
        }
    }
}
