using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Randomness;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Fills the map with pickups at the start of a run: items go to randomly chosen candidate points, each point
    /// used once, with the first-day wood and food guaranteed near the player's start (items.md sections 1-3,
    /// core-rules section 4). The planner (Logic) chooses; this View measures distances and creates the objects.
    /// It runs after the player's start-position pick (done in the player's Start) by using a later script execution order.
    /// <para>
    /// During the run it also places the beacon parts (core-rules sections 1 and 5, tech-decisions 34, R3): on
    /// <see cref="BeaconPartAppeared"/> the part goes to a random candidate point of the event's ring that is not
    /// used yet (any point of that ring when all are used), and a notice names the part and the ring, never the spot.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a Game scene object; assign Candidate Points (empty transforms spread over the map).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;ResourceSpawner&gt;().
    /// </code>
    /// </example>
    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class ResourceSpawner : MonoBehaviour
    {
        /// <summary>Later than the default order 0, so the player has already moved to the random start.</summary>
        private const int ExecutionOrder = 100;

        /// <summary>A beacon part pickup holds one part (it does not stack; items.md section 6).</summary>
        private const int PartAmount = 1;

        [Tooltip("Candidate pickup points. Each is used at most once per run; need at least as many as total pickups. Beacon parts also appear on them.")]
        [SerializeField] private Transform[] candidatePoints;

        [Tooltip("Optional parent for the spawned pickups.")]
        [SerializeField] private Transform pickupParent;

        private readonly List<BeaconPartAppeared> _pendingParts = new List<BeaconPartAppeared>();
        private readonly List<int> _ringPoints = new List<int>();
        private ResourcesConfig _config;
        private IRandom _random;
        private IPlayerPosition _player;
        private PickupFactory _factory;
        private IEventBus _bus;
        private IMapLayout _layout;
        private IItemNames _names;
        private IDisposable _partSubscription;
        private bool[] _usedPoints;

        /// <summary>Receives dependencies from the container and subscribes to <see cref="BeaconPartAppeared"/>.</summary>
        /// <param name="config">Pickup counts and pool size.</param>
        /// <param name="random">Random source.</param>
        /// <param name="player">Player position (the start position at this time).</param>
        /// <param name="factory">Creates the pickups.</param>
        /// <param name="bus">Event channel.</param>
        /// <param name="layout">Tells which ring a candidate point is in.</param>
        /// <param name="names">Item names (for the part notice).</param>
        [Inject]
        public void Construct(
            ResourcesConfig config,
            IRandom random,
            IPlayerPosition player,
            PickupFactory factory,
            IEventBus bus,
            IMapLayout layout,
            IItemNames names)
        {
            _config = config;
            _random = random;
            _player = player;
            _factory = factory;
            _bus = bus;
            _layout = layout;
            _names = names;
            _partSubscription?.Dispose();
            _partSubscription = bus.Subscribe<BeaconPartAppeared>(OnPartAppeared);
        }

        private void Start()
        {
            if (candidatePoints == null || candidatePoints.Length == 0)
            {
                throw new InvalidOperationException("ResourceSpawner: Candidate Points must be assigned.");
            }

            var entries = _config.SpawnEntries;
            var byItem = new Dictionary<ItemId, ResourcesConfig.SpawnEntry>();
            var guaranteed = new List<PlacementRequest>();
            var totals = new List<PlacementRequest>();
            for (var i = 0; i < entries.Count; i++)
            {
                var id = entries[i].Resource.Id;
                byItem[id] = entries[i];
                totals.Add(new PlacementRequest(id, entries[i].TotalPickups));
                if (entries[i].GuaranteedNearStart > 0)
                {
                    guaranteed.Add(new PlacementRequest(id, entries[i].GuaranteedNearStart));
                }
            }

            var plan = new ResourcePlacementPlanner(_random)
                .Plan(candidatePoints.Length, OrderByDistanceFromPlayer(), _config.GuaranteedPoolSize, guaranteed, totals);
            var used = new bool[candidatePoints.Length];
            for (var i = 0; i < plan.Count; i++)
            {
                var point = candidatePoints[plan[i].PointIndex];
                _factory.Create(plan[i].Item, byItem[plan[i].Item].AmountPerPickup, point.position, point.rotation, pickupParent);
                used[plan[i].PointIndex] = true;
            }

            _usedPoints = used;

            // A part scheduled for the first morning arrives before this start plan; it is placed now.
            for (var i = 0; i < _pendingParts.Count; i++)
            {
                PlacePart(_pendingParts[i]);
            }

            _pendingParts.Clear();
        }

        private void OnDestroy()
        {
            _partSubscription?.Dispose();
            _partSubscription = null;
        }

        private void OnPartAppeared(BeaconPartAppeared appeared)
        {
            if (_usedPoints == null)
            {
                _pendingParts.Add(appeared);
                return;
            }

            PlacePart(appeared);
        }

        private void PlacePart(BeaconPartAppeared appeared)
        {
            var noticeKey = NoticeKey(appeared.Ring);
            CollectRingPoints(appeared.Ring, true);
            if (_ringPoints.Count == 0)
            {
                CollectRingPoints(appeared.Ring, false);
            }

            if (_ringPoints.Count == 0)
            {
                throw new InvalidOperationException(
                    $"ResourceSpawner: no Candidate Point lies in the {appeared.Ring} ring, so the part '{appeared.Part}' cannot appear.");
            }

            var index = _ringPoints[_random.NextInt(0, _ringPoints.Count)];
            var point = candidatePoints[index];
            _factory.Create(appeared.Part, PartAmount, point.position, point.rotation, pickupParent);
            _usedPoints[index] = true;
            _bus.Publish(new NoticePosted(noticeKey, _names.GetDisplayName(appeared.Part)));
        }

        private void CollectRingPoints(MapRing ring, bool unusedOnly)
        {
            _ringPoints.Clear();
            for (var i = 0; i < candidatePoints.Length; i++)
            {
                if (unusedOnly && _usedPoints[i])
                {
                    continue;
                }

                var position = candidatePoints[i].position;
                if (_layout.GetRing(_layout.DistanceFromCenter(position.x, position.z)) == ring)
                {
                    _ringPoints.Add(i);
                }
            }
        }

        private static string NoticeKey(MapRing ring)
        {
            switch (ring)
            {
                case MapRing.Middle:
                    return ResourceTextKeys.PartAppearedMiddleNotice;
                case MapRing.Outer:
                    return ResourceTextKeys.PartAppearedOuterNotice;
                default:
                    throw new InvalidOperationException(
                        $"ResourceSpawner: a beacon part is scheduled for the {ring} ring; only the Middle and Outer rings are supported.");
            }
        }

        private List<int> OrderByDistanceFromPlayer()
        {
            var order = new List<int>(candidatePoints.Length);
            for (var i = 0; i < candidatePoints.Length; i++)
            {
                order.Add(i);
            }

            order.Sort((a, b) => SquaredDistance(candidatePoints[a]).CompareTo(SquaredDistance(candidatePoints[b])));
            return order;
        }

        private float SquaredDistance(Transform point)
        {
            var dx = point.position.x - _player.X;
            var dz = point.position.z - _player.Z;
            return dx * dx + dz * dz;
        }
    }
}
