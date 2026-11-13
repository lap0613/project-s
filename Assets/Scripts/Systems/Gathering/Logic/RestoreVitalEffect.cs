using System;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>Raises hunger or health by a fixed amount (berry +15 hunger, herb +20 health, ...).</summary>
    /// <example>
    /// <code>
    /// new RestoreVitalEffect(VitalKind.Health, 20f).Apply(vitals);
    /// </code>
    /// </example>
    public sealed class RestoreVitalEffect : IUseEffect
    {
        private readonly VitalKind _kind;
        private readonly float _amount;

        /// <summary>Creates the effect.</summary>
        /// <param name="kind">Which gauge.</param>
        /// <param name="amount">How much to add.</param>
        public RestoreVitalEffect(VitalKind kind, float amount)
        {
            _kind = kind;
            _amount = amount;
        }

        /// <inheritdoc />
        public void Apply(IVitalsReceiver vitals)
        {
            if (vitals == null)
            {
                throw new ArgumentNullException(nameof(vitals));
            }

            vitals.Restore(_kind, _amount);
        }
    }
}
