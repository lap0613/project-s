using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;
using Whiteout.Core.Randomness;

namespace Whiteout.Gathering
{
    /// <summary>
    /// Decides which candidate point gets which item at the start of a run (design/quick-specs/items.md section 1:
    /// "positions are random among candidate points"; core-rules section 4: first-day wood and food are guaranteed
    /// near the start). Each point is used at most once. Distances are measured by the View, which passes the points
    /// ordered nearest first.
    /// </summary>
    /// <example>
    /// <code>
    /// var planner = new ResourcePlacementPlanner(random);
    /// var plan = planner.Plan(pointCount, nearestFirst, poolSize, guaranteed, totals);
    /// </code>
    /// </example>
    public sealed class ResourcePlacementPlanner
    {
        private readonly IRandom _random;

        /// <summary>Creates the planner.</summary>
        /// <param name="random">Random source (a seed reproduces the layout).</param>
        public ResourcePlacementPlanner(IRandom random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <summary>Builds the plan.</summary>
        /// <param name="pointCount">Number of candidate points.</param>
        /// <param name="nearestFirst">All point indices ordered by distance from the player start, nearest first.</param>
        /// <param name="guaranteedPoolSize">The guaranteed pickups are drawn at random from this many nearest points.</param>
        /// <param name="guaranteed">Pickups that must be near the start. They count toward the totals.</param>
        /// <param name="totals">How many pickups of each item the whole map gets.</param>
        /// <returns>One assignment per pickup, every point used at most once.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are fewer points than pickups.</exception>
        public IReadOnlyList<PlacementAssignment> Plan(
            int pointCount,
            IReadOnlyList<int> nearestFirst,
            int guaranteedPoolSize,
            IReadOnlyList<PlacementRequest> guaranteed,
            IReadOnlyList<PlacementRequest> totals)
        {
            var assignments = new List<PlacementAssignment>();
            var used = new bool[pointCount];
            var placedGuaranteed = new Dictionary<ItemId, int>();

            var pool = new List<int>();
            for (var i = 0; i < Math.Min(guaranteedPoolSize, nearestFirst.Count); i++)
            {
                pool.Add(nearestFirst[i]);
            }

            Shuffle(pool);
            var poolCursor = 0;
            for (var i = 0; i < guaranteed.Count; i++)
            {
                for (var n = 0; n < guaranteed[i].Count; n++)
                {
                    if (poolCursor >= pool.Count)
                    {
                        throw new InvalidOperationException("Not enough candidate points near the start for the guaranteed pickups.");
                    }

                    var point = pool[poolCursor++];
                    used[point] = true;
                    assignments.Add(new PlacementAssignment(point, guaranteed[i].Item));
                }

                placedGuaranteed.TryGetValue(guaranteed[i].Item, out var already);
                placedGuaranteed[guaranteed[i].Item] = already + guaranteed[i].Count;
            }

            var rest = new List<int>();
            for (var i = 0; i < pointCount; i++)
            {
                if (!used[i])
                {
                    rest.Add(i);
                }
            }

            Shuffle(rest);
            var restCursor = 0;
            for (var i = 0; i < totals.Count; i++)
            {
                placedGuaranteed.TryGetValue(totals[i].Item, out var done);
                var remaining = totals[i].Count - done;
                for (var n = 0; n < remaining; n++)
                {
                    if (restCursor >= rest.Count)
                    {
                        throw new InvalidOperationException("Not enough candidate points for all resource pickups.");
                    }

                    assignments.Add(new PlacementAssignment(rest[restCursor++], totals[i].Item));
                }
            }

            return assignments;
        }

        private void Shuffle(List<int> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = _random.NextInt(0, i + 1);
                var swap = list[i];
                list[i] = list[j];
                list[j] = swap;
            }
        }
    }
}
