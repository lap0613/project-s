using Whiteout.Core.Contracts;

namespace Whiteout.Game
{
    /// <summary>
    /// Default <see cref="IWarmthSource"/> that always reports no warmth. Used until the Camp system
    /// (story 004) supplies the real source; that story replaces this registration.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;NoWarmthSource&gt;(Lifetime.Singleton).As&lt;IWarmthSource&gt;();
    /// </code>
    /// </example>
    public sealed class NoWarmthSource : IWarmthSource
    {
        /// <inheritdoc />
        public float CurrentWarmth => 0f;
    }
}
