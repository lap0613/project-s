using Whiteout.Core.Contracts;

namespace Whiteout.Environment
{
    /// <summary>One drawable weather: what it is and how likely it is. Built from a data asset.</summary>
    /// <example>
    /// <code>
    /// var snow = new WeatherSpec(new WeatherInfo("snow", "Snow", -5f, false), 30f);
    /// </code>
    /// </example>
    public readonly struct WeatherSpec
    {
        /// <summary>Creates a spec.</summary>
        /// <param name="info">The weather description sent in events.</param>
        /// <param name="weight">Relative probability weight (40, 30, 20, 10 give 40/30/20/10 percent).</param>
        public WeatherSpec(WeatherInfo info, float weight)
        {
            Info = info;
            Weight = weight;
        }

        /// <summary>The weather description sent in events.</summary>
        public WeatherInfo Info { get; }

        /// <summary>Relative probability weight.</summary>
        public float Weight { get; }
    }
}
