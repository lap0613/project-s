using Whiteout.Core.Contracts;
using UnityEngine;

namespace Whiteout.Core.Unity
{
    /// <summary>
    /// Data asset describing the map: center and ring radii (meters). Implements <see cref="IMapLayout"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create via Assets > Create > Whiteout > Map Layout, then register it as IMapLayout in a LifetimeScope.
    /// IMapLayout layout = mapLayoutDefinition;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Map Layout", fileName = "MapLayout")]
    public sealed class MapLayoutDefinition : ScriptableObject, IMapLayout
    {
        [SerializeField] private float centerX;
        [SerializeField] private float centerZ;
        [SerializeField] private float outerRadius = 200f;
        [SerializeField] private float innerRingRadius = 60f;
        [SerializeField] private float middleRingRadius = 130f;

        /// <inheritdoc />
        public float CenterX => centerX;

        /// <inheritdoc />
        public float CenterZ => centerZ;

        /// <inheritdoc />
        public float OuterRadius => outerRadius;

        /// <inheritdoc />
        public float InnerRingRadius => innerRingRadius;

        /// <inheritdoc />
        public float MiddleRingRadius => middleRingRadius;

        /// <inheritdoc />
        public float DistanceFromCenter(float x, float z)
        {
            var dx = x - centerX;
            var dz = z - centerZ;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <inheritdoc />
        public MapRing GetRing(float distance)
        {
            return MapRingClassifier.Classify(distance, innerRingRadius, middleRingRadius, outerRadius);
        }
    }
}
