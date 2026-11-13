using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Core.Unity
{
    /// <summary>
    /// Every on-screen string of the game, as key to text (tech-decisions item 24, ui-plan D9: Korean only, no text
    /// in code). Code holds only keys; the text may contain <see cref="string.Format(string, object[])"/> placeholders.
    /// Also carries the TextMeshPro font every label uses. A missing key shows the key itself, so it is easy to spot.
    /// </summary>
    /// <example>
    /// <code>
    /// label.font = texts.Font;
    /// label.text = texts.Format("prompt.gather", "나무"); // "[E] 줍기 — 나무"
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/UI Text Table", fileName = "UiTextTable")]
    public sealed class UiTextTable : ScriptableObject
    {
        /// <summary>One key and its text.</summary>
        [Serializable]
        public struct Entry
        {
            [SerializeField] private string key;
            [SerializeField, TextArea(1, 4)] private string text;

            /// <summary>Lookup key, e.g. "prompt.gather".</summary>
            public string Key => key;

            /// <summary>Text shown to the player; may contain {0}-style placeholders.</summary>
            public string Text => text;
        }

        /// <summary>Key of "item amount" text; {0} = name, {1} = amount.</summary>
        public const string ItemAmountKey = "format.item_amount";

        /// <summary>Key of "item shortage" text; {0} = name, {1} = have, {2} = need.</summary>
        public const string ItemShortageKey = "format.item_shortage";

        /// <summary>Key of the text between two items of a list.</summary>
        public const string ItemSeparatorKey = "format.item_separator";

        [Tooltip("Font for every TextMeshPro label. Must contain Korean glyphs.")]
        [SerializeField] private TMP_FontAsset font;

        [SerializeField] private Entry[] entries;

        private Dictionary<string, string> _lookup;

        /// <summary>Font for every TextMeshPro label; null leaves the label's own font.</summary>
        public TMP_FontAsset Font => font;

        /// <summary>Text for a key; the key itself when it is missing.</summary>
        /// <param name="key">Lookup key.</param>
        public string Get(string key)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<string, string>();
                if (entries != null)
                {
                    for (var i = 0; i < entries.Length; i++)
                    {
                        _lookup[entries[i].Key] = entries[i].Text;
                    }
                }
            }

            return key != null && _lookup.TryGetValue(key, out var text) ? text : key;
        }

        /// <summary>Text for a key with its placeholders filled in.</summary>
        /// <param name="key">Lookup key.</param>
        /// <param name="args">Values for {0}, {1}, ...</param>
        public string Format(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        /// <summary>Gives a label the shared font (when one is assigned).</summary>
        /// <param name="label">The label.</param>
        public void ApplyFont(TMP_Text label)
        {
            if (font != null)
            {
                label.font = font;
            }
        }

        /// <summary>
        /// Writes an item list such as "부싯깃 1 + 나무 2". With a bag, an item the bag lacks is written as
        /// have/need ("부싯깃 0/1"; ui-plan section 8). Uses the keys <see cref="ItemAmountKey"/>,
        /// <see cref="ItemShortageKey"/> and <see cref="ItemSeparatorKey"/>.
        /// </summary>
        /// <param name="items">The items and amounts.</param>
        /// <param name="names">Item names.</param>
        /// <param name="bag">Bag to compare against; null writes plain amounts.</param>
        public string FormatItems(IReadOnlyList<ItemAmount> items, IItemNames names, IBag bag)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(Get(ItemSeparatorKey));
                }

                var name = names.GetDisplayName(items[i].Item);
                var have = bag != null ? bag.Count(items[i].Item) : items[i].Amount;
                builder.Append(have < items[i].Amount
                    ? Format(ItemShortageKey, name, have, items[i].Amount)
                    : Format(ItemAmountKey, name, items[i].Amount));
            }

            return builder.ToString();
        }

        private void OnValidate()
        {
            _lookup = null;
        }
    }
}
