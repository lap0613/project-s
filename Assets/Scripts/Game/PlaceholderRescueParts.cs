using System;
using Whiteout.Core.Contracts;
using Whiteout.UI;

namespace Whiteout.Game
{
    /// <summary>
    /// Stand-in <see cref="IRescueParts"/> until the rescue system exists (story-006): always 0 of the configured
    /// number. Story-006 replaces its registration in the Game scope with the real source; the HUD needs no change.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;PlaceholderRescueParts&gt;(Lifetime.Singleton).As&lt;IRescueParts&gt;();
    /// </code>
    /// </example>
    public sealed class PlaceholderRescueParts : IRescueParts
    {
        /// <summary>Creates the stand-in.</summary>
        /// <param name="config">Supplies the required part count.</param>
        public PlaceholderRescueParts(HudConfig config)
        {
            Required = (config ?? throw new ArgumentNullException(nameof(config))).PlaceholderPartsRequired;
        }

        /// <inheritdoc />
        public int Collected => 0;

        /// <inheritdoc />
        public int Required { get; }
    }
}
