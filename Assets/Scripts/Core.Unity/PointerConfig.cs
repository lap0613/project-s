using UnityEngine;

namespace Whiteout.Core.Unity
{
    /// <summary>
    /// Mouse-click interaction values (data asset; design/ux/ui-plan.md section 8.1 and 8.2, M1-M5). All provisional until
    /// the developer has played them. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Pointer Config
    /// var query = new PointerQuery(pointer, config.ClickRange, config.HoverRadius);
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Pointer Config", fileName = "PointerConfig")]
    public sealed class PointerConfig : ScriptableObject
    {
        [Header("Click targets (M3)")]
        [Tooltip("Provisional (M3). Only targets this close to the player can be highlighted and clicked, in meters.")]
        [SerializeField] private float clickRange = 6f;

        [Tooltip("Provisional. How close the cursor must be to a target (measured on the target's height) to count as on it, in meters.")]
        [SerializeField] private float hoverRadius = 0.7f;

        [Header("Walk to a far target (M4)")]
        [Tooltip("Provisional (M4). The click walk is cancelled when the player is blocked this long, in seconds.")]
        [SerializeField] private float stuckCancelSeconds = 1f;

        [Tooltip("Provisional. Moving slower than this toward the target counts as blocked, in meters per second.")]
        [SerializeField] private float stuckSpeed = 0.3f;

        [Header("Highlight (M2)")]
        [Tooltip("Provisional. Outline color of a target the action can be done on now.")]
        [SerializeField] private Color highlightAvailableColor = new Color(1f, 0.92f, 0.35f, 1f);

        [Tooltip("Provisional. Outline color of a target the action cannot be done on now (missing materials).")]
        [SerializeField] private Color highlightUnavailableColor = new Color(0.55f, 0.55f, 0.55f, 1f);

        [Tooltip("Provisional. The outline is the target's mesh scaled by this and drawn behind it.")]
        [SerializeField] private float outlineScale = 1.08f;

        [Tooltip("Unlit material with Cull Front, used to draw the outline hull.")]
        [SerializeField] private Material outlineMaterial;

        /// <summary>Most distance from the player at which a target can be clicked, in meters.</summary>
        public float ClickRange => clickRange;

        /// <summary>Cursor distance to a target that counts as on it, in meters.</summary>
        public float HoverRadius => hoverRadius;

        /// <summary>Seconds blocked before the click walk is cancelled.</summary>
        public float StuckCancelSeconds => stuckCancelSeconds;

        /// <summary>Speed toward the target below which the player counts as blocked, in meters per second.</summary>
        public float StuckSpeed => stuckSpeed;

        /// <summary>Outline color of an available target.</summary>
        public Color HighlightAvailableColor => highlightAvailableColor;

        /// <summary>Outline color of an unavailable target.</summary>
        public Color HighlightUnavailableColor => highlightUnavailableColor;

        /// <summary>Scale of the outline hull.</summary>
        public float OutlineScale => outlineScale;

        /// <summary>Material of the outline hull.</summary>
        public Material OutlineMaterial => outlineMaterial;

        private void OnValidate()
        {
            clickRange = Mathf.Max(0.5f, clickRange);
            hoverRadius = Mathf.Max(0.1f, hoverRadius);
            stuckCancelSeconds = Mathf.Max(0.1f, stuckCancelSeconds);
            stuckSpeed = Mathf.Max(0f, stuckSpeed);
            outlineScale = Mathf.Max(1f, outlineScale);
        }
    }
}
