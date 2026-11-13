using UnityEngine;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Every structure the placement list offers, in list order (key 1 = first). Adding a structure = adding its
    /// definition here. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Structure Catalog; list campfire, shelter, ...
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Structure Catalog", fileName = "StructureCatalog")]
    public sealed class StructureCatalog : ScriptableObject
    {
        [SerializeField] private StructureDefinition[] structures;

        /// <summary>All structure definitions, in list order.</summary>
        public StructureDefinition[] Structures => structures;
    }
}
