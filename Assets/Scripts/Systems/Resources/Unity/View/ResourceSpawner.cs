using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Randomness;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// Fills the map with pickups at the start of a run: items go to randomly chosen candidate points, each point
    /// used once, with the first-day wood and food guaranteed near the player's start (items.md sections 1-3,
    /// core-rules section 4). The planner (Logic) chooses; this View measures distances and creates the objects.
    /// It runs after the player's start-position pick (done in the player's Start) by using a later script execution order.
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

        [Tooltip("Candidate pickup points. Each is used at most once per run; need at least as many as total pickups.")]
        [SerializeField] private Transform[] candidatePoints;

        [Tooltip("Optional parent for the spawned pickups.")]
        [SerializeField] private Transform pickupParent;

        private ResourcesConfig _config;
        private IRandom _random;
        private IPlayerPosition _player;
        private PickupFactory _factory;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="config">Pickup counts and pool size.</param>
        /// <param name="random">Random source.</param>
        /// <param name="player">Player position (the start position at this time).</param>
        /// <param name="factory">Creates the pickups.</param>
        [Inject]
        public void Construct(ResourcesConfig config, IRandom random, IPlayerPosition player, PickupFactory factory)
        {
            _config = config;
            _random = random;
            _player = player;
            _factory = factory;
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

            for (var i = 0; i < plan.Count; i++)
            {
                var point = candidatePoints[plan[i].PointIndex];
                _factory.Create(plan[i].Item, byItem[plan[i].Item].AmountPerPickup, point.position, point.rotation, pickupParent);
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
