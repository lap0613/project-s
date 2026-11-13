using System;
using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>An item and an amount in a structure's cost, as edited in the Inspector.</summary>
    /// <example>
    /// <code>
    /// [SerializeField] private ItemCost[] cost;
    /// IReadOnlyList&lt;ItemAmount&gt; amounts = ItemCost.ToAmounts(cost);
    /// </code>
    /// </example>
    [Serializable]
    public struct ItemCost
    {
        [SerializeField] private ItemIdAsset item;
        [SerializeField] private int amount;

        /// <summary>Converts Inspector entries to the amounts the Logic uses; null gives an empty list.</summary>
        /// <param name="costs">Inspector entries.</param>
        public static IReadOnlyList<ItemAmount> ToAmounts(ItemCost[] costs)
        {
            var amounts = new List<ItemAmount>();
            if (costs != null)
            {
                for (var i = 0; i < costs.Length; i++)
                {
                    amounts.Add(new ItemAmount(costs[i].item.Id, costs[i].amount));
                }
            }

            return amounts;
        }
    }
}
