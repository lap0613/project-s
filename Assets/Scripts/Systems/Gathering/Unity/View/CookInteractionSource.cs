using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// E-action: cook a raw item at a lit campfire (hold E, 3 seconds; core-rules section 4). Needs the player inside
    /// a lit fire's warmth radius, which Camp reports through <see cref="IFireStatus"/>. If the cooked item does not
    /// fit, the raw item is kept and a "bag full" notice is posted.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;CookInteractionSource&gt;(Lifetime.Singleton).As&lt;IInteractionSource&gt;();
    /// </code>
    /// </example>
    public sealed class CookInteractionSource : IInteractionSource
    {
        private readonly CookingRules _cooking;
        private readonly IFireStatus _fire;
        private readonly IBag _bag;
        private readonly ResourceSettings _settings;
        private readonly ResourcesConfig _config;
        private readonly IEventBus _bus;
        private ItemId _target;

        /// <summary>Creates the source.</summary>
        /// <param name="cooking">Cooking rule.</param>
        /// <param name="fire">Tells whether a lit fire is in range and where it is.</param>
        /// <param name="bag">The bag.</param>
        /// <param name="settings">Cook time and item names.</param>
        /// <param name="config">Priority.</param>
        /// <param name="bus">Event channel.</param>
        public CookInteractionSource(
            CookingRules cooking,
            IFireStatus fire,
            IBag bag,
            ResourceSettings settings,
            ResourcesConfig config,
            IEventBus bus)
        {
            _cooking = cooking ?? throw new ArgumentNullException(nameof(cooking));
            _fire = fire ?? throw new ArgumentNullException(nameof(fire));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public int Priority => _config.CookPriority;

        /// <inheritdoc />
        public InteractionPrompt Prompt { get; private set; }

        /// <inheritdoc />
        public bool TryOffer(out float holdSeconds)
        {
            holdSeconds = _settings.CookSeconds;
            if (!_fire.IsInLitFireRange || !_cooking.TryFindCookable(out var raw))
            {
                return false;
            }

            var changed = raw != _target || Prompt.TextKey == null
                || Prompt.X != _fire.NearbyFireX || Prompt.Z != _fire.NearbyFireZ;
            _target = raw;
            if (changed)
            {
                Prompt = new InteractionPrompt(
                    ResourceTextKeys.CookPrompt, _settings.GetDisplayName(raw), true,
                    _fire.NearbyFireX, _fire.NearbyFireY, _fire.NearbyFireZ);
            }

            return true;
        }

        /// <inheritdoc />
        public bool TryOfferPointed(in PointerQuery query, out float holdSeconds)
        {
            holdSeconds = _settings.CookSeconds;
            if (!_fire.IsInLitFireRange
                || !query.Pointer.TryGetPointOnPlane(_fire.NearbyFireY, out var cursorX, out var cursorZ))
            {
                return false;
            }

            var dx = _fire.NearbyFireX - cursorX;
            var dz = _fire.NearbyFireZ - cursorZ;
            if (dx * dx + dz * dz > query.HoverRadius * query.HoverRadius)
            {
                return false;
            }

            return TryOffer(out holdSeconds);
        }

        /// <inheritdoc />
        public bool IsTargetInReach()
        {
            return _fire.IsInLitFireRange && _bag.Count(_target) > 0;
        }

        /// <inheritdoc />
        public bool Begin()
        {
            return true;
        }

        /// <inheritdoc />
        public bool IsStillAvailable()
        {
            return _fire.IsInLitFireRange && _bag.Count(_target) > 0;
        }

        /// <inheritdoc />
        public void Complete()
        {
            if (_cooking.TryCook(_target))
            {
                return;
            }

            if (_settings.TryGetItem(_target, out var raw) && raw.CooksInto.HasValue)
            {
                _bus.Publish(new NoticePosted(ResourceTextKeys.BagFullNotice, _settings.GetDisplayName(raw.CooksInto.Value)));
            }
        }

        /// <inheritdoc />
        public void Cancel()
        {
        }
    }
}
