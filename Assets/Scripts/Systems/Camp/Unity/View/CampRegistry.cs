using System.Collections.Generic;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// The campfires and shelters standing in the scene, so the Camp runner and E-actions can measure distances
    /// without searching the scene (code-structure section 7).
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;CampRegistry&gt;(Lifetime.Singleton);
    /// foreach (var fire in registry.Campfires) { /* measure distance */ }
    /// </code>
    /// </example>
    public sealed class CampRegistry
    {
        private readonly List<CampfireView> _campfires = new List<CampfireView>();
        private readonly List<ShelterView> _shelters = new List<ShelterView>();

        /// <summary>Placed campfires.</summary>
        public IReadOnlyList<CampfireView> Campfires => _campfires;

        /// <summary>Placed shelters.</summary>
        public IReadOnlyList<ShelterView> Shelters => _shelters;

        /// <summary>Starts tracking a campfire.</summary>
        /// <param name="campfire">The campfire view.</param>
        public void Add(CampfireView campfire) => _campfires.Add(campfire);

        /// <summary>Stops tracking a campfire.</summary>
        /// <param name="campfire">The campfire view.</param>
        public void Remove(CampfireView campfire) => _campfires.Remove(campfire);

        /// <summary>Starts tracking a shelter.</summary>
        /// <param name="shelter">The shelter view.</param>
        public void Add(ShelterView shelter) => _shelters.Add(shelter);

        /// <summary>Stops tracking a shelter.</summary>
        /// <param name="shelter">The shelter view.</param>
        public void Remove(ShelterView shelter) => _shelters.Remove(shelter);
    }
}
