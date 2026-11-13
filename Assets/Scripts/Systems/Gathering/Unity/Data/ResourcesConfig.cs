using System;
using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Tunable Resources values (data asset): bag, gathering, cooking, starting items and how many pickups the map
    /// gets. Rules: design/quick-specs/items.md sections 1-3 and 8, core-rules.md section 4.
    /// Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Resources Config
    /// ResourceSettings settings = config.ToSettings();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Resources Config", fileName = "ResourcesConfig")]
    public sealed class ResourcesConfig : ScriptableObject
    {
        /// <summary>One item kind and a count.</summary>
        [Serializable]
        public struct StackEntry
        {
            [SerializeField] private ResourceDefinition resource;
            [SerializeField] private int amount;

            /// <summary>The item.</summary>
            public ResourceDefinition Resource => resource;

            /// <summary>How many.</summary>
            public int Amount => amount;
        }

        /// <summary>How many pickups of one item kind the map gets.</summary>
        [Serializable]
        public struct SpawnEntry
        {
            [Tooltip("The item placed on the map.")]
            [SerializeField] private ResourceDefinition resource;

            [Tooltip("Pickups of this item on the whole map (items.md sections 2 and 3).")]
            [SerializeField] private int totalPickups;

            [Tooltip("Of those, how many are guaranteed near the start position (first-day supply). They count toward the total.")]
            [SerializeField] private int guaranteedNearStart;

            [Tooltip("Provisional. How many items one pickup gives.")]
            [SerializeField] private int amountPerPickup;

            /// <summary>The item placed on the map.</summary>
            public ResourceDefinition Resource => resource;

            /// <summary>Pickups on the whole map.</summary>
            public int TotalPickups => totalPickups;

            /// <summary>Pickups guaranteed near the start.</summary>
            public int GuaranteedNearStart => guaranteedNearStart;

            /// <summary>Items one pickup gives.</summary>
            public int AmountPerPickup => amountPerPickup;
        }

        [Header("Items")]
        [SerializeField] private ResourceCatalog catalog;

        [Header("Bag (items.md section 1)")]
        [Tooltip("Bag slots before modifiers.")]
        [SerializeField] private float baseBagSlots = 6f;

        [Tooltip("What the bag holds at the start (items.md section 8: flint 1, tinder 2, wood 3, berry 1).")]
        [SerializeField] private StackEntry[] startingItems;

        [Header("Gather and cook (core-rules section 4)")]
        [Tooltip("Gather speed before modifiers (1 = normal).")]
        [SerializeField] private float baseGatherSpeed = 1f;

        [Tooltip("Seconds to hold E to gather at normal speed.")]
        [SerializeField] private float gatherSeconds = 1f;

        [Tooltip("Seconds to hold E to cook.")]
        [SerializeField] private float cookSeconds = 3f;

        [Tooltip("Provisional. How close the player must be to a pickup to gather it, in meters.")]
        [SerializeField] private float gatherRange = 2f;

        [Tooltip("Provisional. E-action priority; the highest wins when several actions are possible at once.")]
        [SerializeField] private int gatherPriority = 10;

        [Tooltip("Provisional. E-action priority of cooking.")]
        [SerializeField] private int cookPriority = 40;

        [Header("Bag slot menu (ui-plan D3, D5)")]
        [Tooltip("Provisional. Menu position of 'use'; the lowest order is the first entry and the default action.")]
        [SerializeField] private int useActionOrder;

        [Tooltip("Provisional. Menu position of 'drop'.")]
        [SerializeField] private int dropActionOrder = 100;

        [Header("Map pickups (items.md sections 2-3, core-rules section 4)")]
        [SerializeField] private SpawnEntry[] spawnEntries;

        [Tooltip("Provisional. The guaranteed pickups are drawn at random from this many candidate points nearest the start.")]
        [SerializeField] private int guaranteedPoolSize = 20;

        /// <summary>Gather range in meters.</summary>
        public float GatherRange => gatherRange;

        /// <summary>Gather E-action priority.</summary>
        public int GatherPriority => gatherPriority;

        /// <summary>Cook E-action priority.</summary>
        public int CookPriority => cookPriority;

        /// <summary>Menu position of the 'use' entry.</summary>
        public int UseActionOrder => useActionOrder;

        /// <summary>Menu position of the 'drop' entry.</summary>
        public int DropActionOrder => dropActionOrder;

        /// <summary>Finds the definition of an item in the catalog.</summary>
        /// <param name="item">Item id.</param>
        /// <param name="definition">The definition when found.</param>
        /// <returns>True when the catalog has the item.</returns>
        public bool TryGetDefinition(ItemId item, out ResourceDefinition definition)
        {
            for (var i = 0; i < catalog.Resources.Length; i++)
            {
                if (catalog.Resources[i].Id == item)
                {
                    definition = catalog.Resources[i];
                    return true;
                }
            }

            definition = null;
            return false;
        }

        /// <summary>How many nearest candidate points the guaranteed pickups are drawn from.</summary>
        public int GuaranteedPoolSize => guaranteedPoolSize;

        /// <summary>Map pickup counts per item.</summary>
        public IReadOnlyList<SpawnEntry> SpawnEntries => spawnEntries;

        private void OnValidate()
        {
            baseBagSlots = Mathf.Max(baseBagSlots, 0f);
            gatherSeconds = Mathf.Max(gatherSeconds, 0f);
            cookSeconds = Mathf.Max(cookSeconds, 0f);
            guaranteedPoolSize = Mathf.Max(guaranteedPoolSize, 0);
        }

        /// <summary>Converts to the settings object the Logic uses.</summary>
        public ResourceSettings ToSettings()
        {
            var specs = new List<ItemSpec>();
            for (var i = 0; i < catalog.Resources.Length; i++)
            {
                specs.Add(catalog.Resources[i].ToSpec());
            }

            var start = new List<ItemAmount>();
            if (startingItems != null)
            {
                for (var i = 0; i < startingItems.Length; i++)
                {
                    start.Add(new ItemAmount(startingItems[i].Resource.Id, startingItems[i].Amount));
                }
            }

            return new ResourceSettings(specs, baseBagSlots, baseGatherSpeed, gatherSeconds, cookSeconds, start);
        }
    }
}
