using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// Data asset for one item kind (design/quick-specs/items.md). A new item is a new asset in the
    /// <see cref="ResourceCatalog"/>; no code changes (code-structure section 6). Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Resource Definition
    /// ItemSpec spec = definition.ToSpec();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Resource Definition", fileName = "Resource")]
    public sealed class ResourceDefinition : ScriptableObject
    {
        [Tooltip("Identity of the item. Camp costs point at the same ItemIdAsset.")]
        [SerializeField] private ItemIdAsset item;

        [SerializeField] private string displayName;

        [Tooltip("Most that fit in one bag slot (items.md sections 2 and 3).")]
        [SerializeField] private int stackLimit = 1;

        [Tooltip("What using one from the bag does. Empty = not usable.")]
        [SerializeField] private UseEffectDefinition[] useEffects;

        [Tooltip("Item produced by cooking this one at a lit fire. Empty = cannot be cooked.")]
        [SerializeField] private ResourceDefinition cooksInto;

        [Tooltip("Object placed on the map for pickups of this item.")]
        [SerializeField] private GameObject pickupPrefab;

        /// <summary>Item identity.</summary>
        public ItemId Id => item.Id;

        /// <summary>Player-facing name.</summary>
        public string DisplayName => displayName;

        /// <summary>Object placed on the map for pickups of this item.</summary>
        public GameObject PickupPrefab => pickupPrefab;

        private void OnValidate()
        {
            stackLimit = Mathf.Max(stackLimit, 1);
        }

        /// <summary>Converts to the spec object the Logic uses.</summary>
        public ItemSpec ToSpec()
        {
            var effects = new List<IUseEffect>();
            if (useEffects != null)
            {
                for (var i = 0; i < useEffects.Length; i++)
                {
                    effects.Add(useEffects[i].CreateEffect());
                }
            }

            ItemId? cooked = cooksInto != null ? cooksInto.Id : (ItemId?)null;
            return new ItemSpec(Id, displayName, stackLimit, effects, cooked);
        }
    }
}
