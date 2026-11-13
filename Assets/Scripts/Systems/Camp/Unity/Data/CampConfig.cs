using UnityEngine;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Tunable Camp values (data asset): the structure catalog, the placement mode (ui-plan section 9) and the
    /// E-action settings. Per-structure rule values live in the structure definitions. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Camp Config
    /// StructureDefinition first = config.Catalog.Structures[0];
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Camp Config", fileName = "CampConfig")]
    public sealed class CampConfig : ScriptableObject
    {
        [SerializeField] private StructureCatalog catalog;

        [Header("Placement mode (ui-plan section 9)")]
        [Tooltip("Provisional (ui-plan P3). The preview follows the mouse cursor but stops this far from the player, in meters.")]
        [SerializeField] private float maxPlaceDistance = 6f;

        [Tooltip("Provisional (ui-plan P2). One mouse wheel notch turns the preview by this many degrees.")]
        [SerializeField] private float rotationStepDegrees = 15f;

        [Tooltip("Layers that block placement (trees, rocks, placed structures). Do not include the ground or the player.")]
        [SerializeField] private LayerMask obstacleMask;

        [Tooltip("Translucent blue material for a preview that can be placed.")]
        [SerializeField] private Material previewValidMaterial;

        [Tooltip("Translucent red material for a preview that overlaps an obstacle.")]
        [SerializeField] private Material previewBlockedMaterial;

        [Header("E actions")]
        [Tooltip("Provisional. Seconds to hold E to add fuel or relight.")]
        [SerializeField] private float fuelHoldSeconds = 0.5f;

        [Tooltip("Provisional. E-action priority of fueling; the highest wins when several actions are possible.")]
        [SerializeField] private int fuelPriority = 30;

        [Tooltip("Provisional. E-action priority of resting.")]
        [SerializeField] private int restPriority = 20;

        /// <summary>The structures the placement list offers.</summary>
        public StructureCatalog Catalog => catalog;

        /// <summary>Farthest the preview (and the placed structure) can be from the player, in meters.</summary>
        public float MaxPlaceDistance => maxPlaceDistance;

        /// <summary>Degrees one wheel notch turns the preview.</summary>
        public float RotationStepDegrees => rotationStepDegrees;

        /// <summary>Layers that block placement.</summary>
        public LayerMask ObstacleMask => obstacleMask;

        /// <summary>Material of a placeable preview.</summary>
        public Material PreviewValidMaterial => previewValidMaterial;

        /// <summary>Material of a blocked preview.</summary>
        public Material PreviewBlockedMaterial => previewBlockedMaterial;

        /// <summary>Seconds to hold E to add fuel or relight.</summary>
        public float FuelHoldSeconds => fuelHoldSeconds;

        /// <summary>E-action priority of fueling.</summary>
        public int FuelPriority => fuelPriority;

        /// <summary>E-action priority of resting.</summary>
        public int RestPriority => restPriority;
    }
}
