using Whiteout.Core.Stats;
using UnityEngine;

namespace Whiteout.Core.Unity
{
    /// <summary>
    /// Editor-pickable wrapper around a <see cref="StatId"/>, so data assets can reference a stat.
    /// </summary>
    /// <example>
    /// <code>
    /// [SerializeField] private StatIdAsset stat;
    /// StatId id = stat.Id;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Stat Id", fileName = "StatId")]
    public sealed class StatIdAsset : ScriptableObject
    {
        [SerializeField] private string id;

        /// <summary>The stat id. Throws <see cref="System.ArgumentException"/> if the asset's id text is empty.</summary>
        public StatId Id => new StatId(id);
    }
}
