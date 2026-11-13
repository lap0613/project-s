using UnityEngine;

namespace Whiteout.UI
{
    /// <summary>
    /// Tunable HUD values (data asset): notices, bag slot colors and world UI offsets (design/ux/ui-plan.md
    /// sections 7.3 and 8). All provisional. Texts are in the UiTextTable; sizes and positions of the HUD objects are
    /// set on the Canvas in the scene.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > HUD Config
    /// float seconds = config.NoticeSeconds;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/HUD Config", fileName = "HudConfig")]
    public sealed class HudConfig : ScriptableObject
    {
        [Header("Notices (ui-plan 7.3 [E])")]
        [Tooltip("Most notices shown at once; the oldest is dropped.")]
        [SerializeField] private int noticeMaxLines = 3;

        [Tooltip("Seconds a notice stays.")]
        [SerializeField] private float noticeSeconds = 3f;

        [Header("Bag slots (ui-plan 7.3 [G])")]
        [Tooltip("Border color of an unselected slot.")]
        [SerializeField] private Color slotBorderColor = new Color(1f, 1f, 1f, 0.35f);

        [Tooltip("Border color of the selected slot.")]
        [SerializeField] private Color slotSelectedBorderColor = new Color(1f, 0.85f, 0.2f, 1f);

        [Header("Shared")]
        [Tooltip("Color of text for something that cannot be done now (missing materials).")]
        [SerializeField] private Color unavailableTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        [Tooltip("Normal text color.")]
        [SerializeField] private Color textColor = Color.white;

        [Header("World UI (ui-plan section 8)")]
        [Tooltip("The prompt is drawn this many meters above its target.")]
        [SerializeField] private float promptHeight = 1.5f;

        [Tooltip("The campfire fuel display is drawn this many meters above the fire.")]
        [SerializeField] private float fuelDisplayHeight = 2.2f;

        [Tooltip("Color of a fuel pip that still has fuel.")]
        [SerializeField] private Color fuelPipFilledColor = new Color(0.95f, 0.45f, 0.2f, 1f);

        [Tooltip("Color of an empty fuel pip.")]
        [SerializeField] private Color fuelPipEmptyColor = new Color(0f, 0f, 0f, 0.6f);

        /// <summary>Most notices shown at once.</summary>
        public int NoticeMaxLines => noticeMaxLines;

        /// <summary>Seconds a notice stays.</summary>
        public float NoticeSeconds => noticeSeconds;

        /// <summary>Border color of an unselected slot.</summary>
        public Color SlotBorderColor => slotBorderColor;

        /// <summary>Border color of the selected slot.</summary>
        public Color SlotSelectedBorderColor => slotSelectedBorderColor;

        /// <summary>Text color for something that cannot be done now.</summary>
        public Color UnavailableTextColor => unavailableTextColor;

        /// <summary>Normal text color.</summary>
        public Color TextColor => textColor;

        /// <summary>Height of the prompt above its target, in meters.</summary>
        public float PromptHeight => promptHeight;

        /// <summary>Height of the fuel display above the fire, in meters.</summary>
        public float FuelDisplayHeight => fuelDisplayHeight;

        /// <summary>Color of a fuel pip that still has fuel.</summary>
        public Color FuelPipFilledColor => fuelPipFilledColor;

        /// <summary>Color of an empty fuel pip.</summary>
        public Color FuelPipEmptyColor => fuelPipEmptyColor;

        private void OnValidate()
        {
            noticeMaxLines = Mathf.Max(noticeMaxLines, 1);
            noticeSeconds = Mathf.Max(noticeSeconds, 0f);
        }
    }
}
