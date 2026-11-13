using UnityEngine;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Campfire data (core-rules sections 3 and 4): cost wood 3 + tinder 1 with flint in the bag, 1 wood = 60 s,
    /// at most 5 wood, +35 degrees within 4 m, light a new fire with wood 2, relight with tinder 1 + wood 2. The structure's required items (flint)
    /// are also what relighting needs. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Structure > Campfire
    /// CampfireSpec spec = definition.ToCampfireSpec();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Structure/Campfire", fileName = "Structure_Campfire")]
    public sealed class CampfireDefinition : StructureDefinition
    {
        [Header("Fuel (core-rules section 4)")]
        [SerializeField] private ItemIdAsset fuelItem;

        [Tooltip("Burning time one fuel item gives, in seconds.")]
        [SerializeField] private float burnSecondsPerFuel = 60f;

        [Tooltip("Most fuel items the fire holds.")]
        [SerializeField] private int maxFuel = 5;

        [Tooltip("Items consumed to light a new, never-lit fire (wood 2, ui-plan section 8, M6). The fuel items among them become its fuel.")]
        [SerializeField] private ItemCost[] igniteCost;

        [Tooltip("Items consumed to relight a burned-out fire. The fuel items among them become its fuel.")]
        [SerializeField] private ItemCost[] relightCost;

        [Header("Warmth (core-rules section 3)")]
        [Tooltip("Temperature bonus in degrees C while lit.")]
        [SerializeField] private float warmth = 35f;

        [Tooltip("Warmth radius in meters. Cooking also needs the player inside it.")]
        [SerializeField] private float warmthRadius = 4f;

        [Header("Interaction")]
        [Tooltip("Provisional. How close the player must be to add fuel or relight, in meters.")]
        [SerializeField] private float interactRange = 2f;

        [Tooltip("Provisional (ui-plan section 8). Within this distance the fuel display is shown, in meters.")]
        [SerializeField] private float statusRange = 6f;

        /// <summary>Warmth radius in meters.</summary>
        public float WarmthRadius => warmthRadius;

        /// <summary>Fuel/relight interaction range in meters.</summary>
        public float InteractRange => interactRange;

        /// <summary>Distance within which the fuel display is shown, in meters.</summary>
        public float StatusRange => statusRange;

        /// <summary>Converts to the campfire spec the Logic uses.</summary>
        public CampfireSpec ToCampfireSpec()
        {
            return new CampfireSpec(
                fuelItem.Id, burnSecondsPerFuel, maxFuel, warmth, ItemCost.ToAmounts(igniteCost), ItemCost.ToAmounts(relightCost),
                ToRequiredItems());
        }
    }
}
