using Whiteout.Core.Stats;

namespace Whiteout.Survival
{
    /// <summary>Stat ids owned by the Survival system.</summary>
    /// <example>
    /// <code>
    /// var temperature = new Stat(SurvivalStats.Temperature, 100f);
    /// </code>
    /// </example>
    public static class SurvivalStats
    {
        /// <summary>Body temperature gauge (체온), 0 to max.</summary>
        public static readonly StatId Temperature = new StatId("survival.temperature");

        /// <summary>Hunger gauge (허기), 0 to max.</summary>
        public static readonly StatId Hunger = new StatId("survival.hunger");

        /// <summary>Health gauge (체력), 0 to max.</summary>
        public static readonly StatId Health = new StatId("survival.health");
    }
}
