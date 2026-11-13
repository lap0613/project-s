using UnityEngine;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// Every item kind in the game. Adding an item = adding its <see cref="ResourceDefinition"/> here.
    /// Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Resource Catalog; list the ResourceDefinition assets.
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Resource Catalog", fileName = "ResourceCatalog")]
    public sealed class ResourceCatalog : ScriptableObject
    {
        [SerializeField] private ResourceDefinition[] resources;

        /// <summary>All item definitions.</summary>
        public ResourceDefinition[] Resources => resources;
    }
}
