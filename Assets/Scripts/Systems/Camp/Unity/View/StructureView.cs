using UnityEngine;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Base of the component on a placed structure's prefab root. The placement controller creates the prefab
    /// (dependencies are injected by VContainer) and then calls <see cref="Initialize"/> with the definition it was
    /// placed from. A new structure kind adds its own subclass.
    /// </summary>
    /// <example>
    /// <code>
    /// if (instance.TryGetComponent&lt;StructureView&gt;(out var view)) { view.Initialize(definition); }
    /// </code>
    /// </example>
    public abstract class StructureView : MonoBehaviour
    {
        /// <summary>Called once, right after the structure is placed.</summary>
        /// <param name="definition">The definition the structure was placed from.</param>
        public abstract void Initialize(StructureDefinition definition);
    }
}
