using UnityEngine;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Shelter data (core-rules section 4): cost wood 6, radius 3 m, no warmth. Inside the radius the player can
    /// rest with E. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Structure > Shelter
    /// float radius = definition.Radius;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Structure/Shelter", fileName = "Structure_Shelter")]
    public sealed class ShelterDefinition : StructureDefinition
    {
        [Tooltip("Rest radius in meters.")]
        [SerializeField] private float radius = 3f;

        /// <summary>Rest radius in meters.</summary>
        public float Radius => radius;
    }
}
