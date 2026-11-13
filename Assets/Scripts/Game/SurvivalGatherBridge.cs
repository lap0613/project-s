using System;
using VContainer.Unity;
using Whiteout.Core.Stats;
using Whiteout.Gathering;
using Whiteout.Survival;

namespace Whiteout.Game
{
    /// <summary>
    /// Connects Survival to Resources without either knowing the other (code-structure section 4): while hunger is
    /// below 30, gather speed gets a -30% Multiply modifier (core-rules section 2), which makes gathering take longer.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;SurvivalGatherBridge&gt;();
    /// </code>
    /// </example>
    public sealed class SurvivalGatherBridge : ITickable, IDisposable
    {
        private const string HungerSource = "survival.hunger";

        private readonly SurvivalSystem _survival;
        private readonly SurvivalSettings _settings;
        private readonly GatherRules _gather;
        private IDisposable _hungerPenalty;

        /// <summary>Creates the bridge.</summary>
        /// <param name="survival">Source of the low-hunger state.</param>
        /// <param name="settings">Supplies the penalty size.</param>
        /// <param name="gather">Owner of the gather speed stat.</param>
        public SurvivalGatherBridge(SurvivalSystem survival, SurvivalSettings settings, GatherRules gather)
        {
            _survival = survival ?? throw new ArgumentNullException(nameof(survival));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _gather = gather ?? throw new ArgumentNullException(nameof(gather));
        }

        /// <inheritdoc />
        public void Tick()
        {
            var low = _survival.IsHungerLow;
            if (low && _hungerPenalty == null)
            {
                _hungerPenalty = _gather.Speed.AddModifier(
                    new StatModifier(StatModifierKind.Multiply, -_settings.GatherSpeedPenalty, HungerSource));
            }
            else if (!low && _hungerPenalty != null)
            {
                _hungerPenalty.Dispose();
                _hungerPenalty = null;
            }
        }

        /// <summary>Removes the penalty.</summary>
        public void Dispose()
        {
            _hungerPenalty?.Dispose();
        }
    }
}
