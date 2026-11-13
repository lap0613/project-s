using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Resources.Unity
{
    /// <summary>Use effect: raise hunger or health by a fixed amount (items.md section 3).</summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Use Effect > Restore Vital: vital = Hunger, amount = 15 (berry)
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Use Effect/Restore Vital", fileName = "Effect_RestoreVital")]
    public sealed class RestoreVitalEffectDefinition : UseEffectDefinition
    {
        [SerializeField] private VitalKind vital = VitalKind.Hunger;

        [Tooltip("How much of the gauge is restored.")]
        [SerializeField] private float amount = 15f;

        /// <inheritdoc />
        public override IUseEffect CreateEffect()
        {
            return new RestoreVitalEffect(vital, amount);
        }
    }
}
