using UnityEngine;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Data asset describing what using a consumable does. Subclass it (and add an <see cref="IUseEffect"/>)
    /// for a new kind of effect; plain value changes need no new code. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// IUseEffect effect = definition.CreateEffect();
    /// </code>
    /// </example>
    public abstract class UseEffectDefinition : ScriptableObject
    {
        /// <summary>Builds the Logic object that applies the effect.</summary>
        public abstract IUseEffect CreateEffect();
    }
}
