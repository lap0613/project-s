using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Stats;

namespace Whiteout.Survival
{
    /// <summary>
    /// The three hurdles: body temperature, hunger and health (design/quick-specs/core-rules.md section 2).
    /// Time moves only through <see cref="Tick"/>. Ambient temperature is read through
    /// <see cref="IAmbientTemperatureSource"/>. When health reaches 0, <see cref="PlayerDied"/> is published once
    /// and ticking stops. Each gauge is a <see cref="Stat"/>: the current value lives in the base value
    /// (kept between 0 and the maximum) and modifiers can be attached by other systems.
    /// If both body temperature and hunger are 0 on the killing tick, the cause is the one that drained more
    /// health that tick (freezing on a tie), a provisional rule from tech-decisions.md item 23.
    /// While resting in a shelter (<see cref="SetResting"/>) health regenerates from a lower threshold and faster.
    /// Items restore gauges through <see cref="Restore"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// var survival = new SurvivalSystem(settings, ambient, bus);
    /// survival.Tick(0.016f);
    /// bool slow = survival.IsTemperatureLow;
    /// </code>
    /// </example>
    public sealed class SurvivalSystem : IRestTarget, IVitalsReceiver
    {
        private readonly SurvivalSettings _settings;
        private readonly IAmbientTemperatureSource _ambient;
        private readonly IEventBus _bus;

        /// <summary>Creates the system with all gauges at the starting value.</summary>
        /// <param name="settings">Rule values.</param>
        /// <param name="ambient">Ambient temperature at the player.</param>
        /// <param name="bus">Event channel for <see cref="PlayerDied"/>.</param>
        public SurvivalSystem(SurvivalSettings settings, IAmbientTemperatureSource ambient, IEventBus bus)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _ambient = ambient ?? throw new ArgumentNullException(nameof(ambient));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));

            Temperature = new Stat(SurvivalStats.Temperature, settings.StartValue);
            Hunger = new Stat(SurvivalStats.Hunger, settings.StartValue);
            Health = new Stat(SurvivalStats.Health, settings.StartValue);
        }

        /// <summary>Body temperature (체온). Change the base value to heal or hurt; attach modifiers for bonuses.</summary>
        public Stat Temperature { get; }

        /// <summary>Hunger (허기). Food raises the base value (story 004).</summary>
        public Stat Hunger { get; }

        /// <summary>Health (체력).</summary>
        public Stat Health { get; }

        /// <summary>True when body temperature is below the low threshold (move speed penalty).</summary>
        public bool IsTemperatureLow => Temperature.Value < _settings.LowThreshold;

        /// <summary>True when hunger is below the low threshold (gather speed penalty, faster cooling).</summary>
        public bool IsHungerLow => Hunger.Value < _settings.LowThreshold;

        /// <summary>True after health reached 0. Ticking does nothing from then on.</summary>
        public bool IsDead { get; private set; }

        /// <summary>True while the player rests in a shelter (set by Camp through <see cref="IRestTarget"/>).</summary>
        public bool IsResting { get; private set; }

        /// <inheritdoc />
        public void SetResting(bool resting)
        {
            IsResting = resting;
        }

        /// <inheritdoc />
        public void Restore(VitalKind kind, float amount)
        {
            if (IsDead)
            {
                return;
            }

            Change(kind == VitalKind.Hunger ? Hunger : Health, amount);
        }

        /// <summary>Advances the hurdles by <paramref name="dt"/> seconds.</summary>
        /// <param name="dt">Elapsed seconds; zero or negative is ignored.</param>
        public void Tick(float dt)
        {
            if (IsDead || dt <= 0f)
            {
                return;
            }

            var temperatureRate = _ambient.CurrentTemperature * _settings.TemperaturePerDegree;
            if (temperatureRate < 0f && IsHungerLow)
            {
                temperatureRate *= _settings.ColdHungerMultiplier;
            }

            Change(Temperature, temperatureRate * dt);
            Change(Hunger, -_settings.HungerDrainPerSecond * dt);

            var freezing = Temperature.Value <= 0f ? _settings.FreezingDamagePerSecond * dt : 0f;
            var starvation = Hunger.Value <= 0f ? _settings.StarvationDamagePerSecond * dt : 0f;

            if (freezing + starvation > 0f)
            {
                Change(Health, -(freezing + starvation));
                if (Health.Value <= 0f)
                {
                    Die(freezing >= starvation ? DeathCause.Freezing : DeathCause.Starvation);
                }
            }
            else
            {
                var threshold = IsResting ? _settings.RestRegenThreshold : _settings.RegenThreshold;
                if (Temperature.Value >= threshold && Hunger.Value >= threshold)
                {
                    var regen = _settings.HealthRegenPerSecond * (IsResting ? _settings.RestRegenMultiplier : 1f);
                    Change(Health, regen * dt);
                }
            }
        }

        private void Change(Stat stat, float delta)
        {
            stat.BaseValue = Math.Min(Math.Max(stat.BaseValue + delta, 0f), _settings.MaxValue);
        }

        private void Die(DeathCause cause)
        {
            IsDead = true;
            _bus.Publish(new PlayerDied(cause));
        }
    }
}
