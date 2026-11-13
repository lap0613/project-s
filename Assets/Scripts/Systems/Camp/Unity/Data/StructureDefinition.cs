using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Data asset for something the player places with B (items.md section 7). A new structure (story 006's signal
    /// beacon) is a new subclass asset in the <see cref="StructureCatalog"/> plus a view on its prefab; the placement
    /// code and the list do not change (code-structure section 6). Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// StructureSpec spec = definition.ToSpec();
    /// </code>
    /// </example>
    public abstract class StructureDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [Tooltip("Object created when placed. Its StructureView (if any) is initialized with this definition.")]
        [SerializeField] private GameObject prefab;

        [Tooltip("Visual-only object shown as the translucent preview in placement mode.")]
        [SerializeField] private GameObject previewPrefab;

        [Tooltip("Items consumed on placement.")]
        [SerializeField] private ItemCost[] cost;

        [Tooltip("Items that must be in the bag but are not consumed (flint).")]
        [SerializeField] private ItemIdAsset[] requiredItems;

        [Tooltip("Provisional. Radius in meters that must be free of obstacles to place this.")]
        [SerializeField] private float footprintRadius = 0.75f;

        /// <summary>Structure id.</summary>
        public string Id => id;

        /// <summary>Player-facing name.</summary>
        public string DisplayName => displayName;

        /// <summary>Object created when placed.</summary>
        public GameObject Prefab => prefab;

        /// <summary>Visual-only preview object.</summary>
        public GameObject PreviewPrefab => previewPrefab;

        /// <summary>Radius in meters that must be free of obstacles.</summary>
        public float FootprintRadius => footprintRadius;

        /// <summary>The required (not consumed) items as ids.</summary>
        public IReadOnlyList<ItemId> ToRequiredItems()
        {
            var required = new List<ItemId>();
            if (requiredItems != null)
            {
                for (var i = 0; i < requiredItems.Length; i++)
                {
                    required.Add(requiredItems[i].Id);
                }
            }

            return required;
        }

        /// <summary>Converts to the spec object the Logic uses.</summary>
        public StructureSpec ToSpec()
        {
            return new StructureSpec(id, displayName, ItemCost.ToAmounts(cost), ToRequiredItems());
        }
    }
}
