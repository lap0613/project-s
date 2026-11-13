using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// E-action: gather the nearest pickup (hold E, 1 second at normal speed; core-rules section 4). The time comes
    /// from <see cref="GatherRules"/>, so the hunger penalty applies. When the bag cannot take the item the pickup
    /// stays and a "bag full" notice is posted.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;GatherInteractionSource&gt;(Lifetime.Singleton).As&lt;IInteractionSource&gt;();
    /// </code>
    /// </example>
    public sealed class GatherInteractionSource : IInteractionSource
    {
        private readonly PickupRegistry _registry;
        private readonly IPlayerPosition _player;
        private readonly IBag _bag;
        private readonly GatherRules _rules;
        private readonly ResourceSettings _settings;
        private readonly ResourcesConfig _config;
        private readonly IEventBus _bus;
        private ResourcePickup _target;

        /// <summary>Creates the source.</summary>
        /// <param name="registry">Pickups on the map.</param>
        /// <param name="player">Player position.</param>
        /// <param name="bag">The bag to fill.</param>
        /// <param name="rules">Gather time.</param>
        /// <param name="settings">Item names.</param>
        /// <param name="config">Gather range and priority.</param>
        /// <param name="bus">Event channel.</param>
        public GatherInteractionSource(
            PickupRegistry registry,
            IPlayerPosition player,
            IBag bag,
            GatherRules rules,
            ResourceSettings settings,
            ResourcesConfig config,
            IEventBus bus)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public int Priority => _config.GatherPriority;

        /// <inheritdoc />
        public InteractionPrompt Prompt { get; private set; }

        /// <inheritdoc />
        public bool TryOffer(out float holdSeconds)
        {
            holdSeconds = _rules.HoldSeconds;
            if (!_registry.TryFindNearest(_player.X, _player.Z, _config.GatherRange, out var nearest))
            {
                _target = null;
                return false;
            }

            if (nearest != _target)
            {
                _target = nearest;
                var position = nearest.transform.position;
                Prompt = new InteractionPrompt(
                    ResourceTextKeys.GatherPrompt, _settings.GetDisplayName(nearest.Item), true, position.x, position.y, position.z);
            }

            return true;
        }

        /// <inheritdoc />
        public bool Begin()
        {
            if (_bag.CanAdd(_target.Item, _target.Amount))
            {
                return true;
            }

            _bus.Publish(new NoticePosted(ResourceTextKeys.BagFullNotice, _settings.GetDisplayName(_target.Item)));
            return false;
        }

        /// <inheritdoc />
        public bool IsStillAvailable()
        {
            return _target != null && PickupRegistry.Distance(_target, _player.X, _player.Z) <= _config.GatherRange;
        }

        /// <inheritdoc />
        public void Complete()
        {
            if (_target == null || !_bag.CanAdd(_target.Item, _target.Amount))
            {
                return;
            }

            var gathered = _target;
            _target = null;
            _bag.Add(gathered.Item, gathered.Amount);
            _registry.Remove(gathered);
            UnityEngine.Object.Destroy(gathered.gameObject);
            _bus.Publish(new ResourceGathered(gathered.Item, gathered.Amount));
        }

        /// <inheritdoc />
        public void Cancel()
        {
        }
    }
}
