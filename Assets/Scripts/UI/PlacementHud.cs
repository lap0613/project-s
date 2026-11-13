using System.Text;
using TMPro;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// The structure list of the placement mode (ui-plan section 9): one line per structure with its number, name
    /// and cost, the selected one marked, entries the player cannot afford greyed with have/need amounts, and a key
    /// hint line. Built from <see cref="IPlacementMenu"/>, so a new structure definition just adds a line.
    /// Redraws only when the selection or the bag changes.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign Panel (shown only while the mode is open) and its TextMeshProUGUI Label.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;PlacementHud&gt;().
    /// </code>
    /// </example>
    public sealed class PlacementHud : MonoBehaviour
    {
        private const string EntryKey = "placement.entry";
        private const string SelectedEntryKey = "placement.entry_selected";
        private const string HintKey = "placement.hint";

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text label;

        private readonly StringBuilder _builder = new StringBuilder();
        private IPlacementMenu _menu;
        private IBag _bag;
        private IItemNames _names;
        private UiTextTable _texts;
        private HudConfig _config;
        private int _shownSelection = -1;
        private int _shownBagVersion = -1;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="menu">The placement list.</param>
        /// <param name="bag">The bag (what the player can afford).</param>
        /// <param name="names">Item names.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Greyed text color.</param>
        [Inject]
        public void Construct(IPlacementMenu menu, IBag bag, IItemNames names, UiTextTable texts, HudConfig config)
        {
            _menu = menu;
            _bag = bag;
            _names = names;
            _texts = texts;
            _config = config;
        }

        private void Start()
        {
            _texts.ApplyFont(label);
            panel.SetActive(false);
        }

        private void Update()
        {
            if (panel.activeSelf != _menu.IsOpen)
            {
                panel.SetActive(_menu.IsOpen);
                _shownSelection = -1;
            }

            if (!_menu.IsOpen || (_shownSelection == _menu.SelectedIndex && _shownBagVersion == _bag.Version))
            {
                return;
            }

            _shownSelection = _menu.SelectedIndex;
            _shownBagVersion = _bag.Version;
            Redraw();
        }

        private void Redraw()
        {
            var greyHex = ColorUtility.ToHtmlStringRGB(_config.UnavailableTextColor);
            _builder.Clear();
            for (var i = 0; i < _menu.EntryCount; i++)
            {
                var cost = _menu.GetEntryCost(i);
                var required = _menu.GetEntryRequiredItems(i);
                var affordable = true;
                var costText = _texts.FormatItems(cost, _names, _bag);
                for (var c = 0; c < cost.Count; c++)
                {
                    affordable &= _bag.Count(cost[c].Item) >= cost[c].Amount;
                }

                for (var r = 0; r < required.Count; r++)
                {
                    if (_bag.Count(required[r]) >= 1)
                    {
                        continue;
                    }

                    affordable = false;
                    costText += _texts.Get(UiTextTable.ItemSeparatorKey)
                        + _texts.Format(UiTextTable.ItemShortageKey, _names.GetDisplayName(required[r]), 0, 1);
                }

                var line = _texts.Format(
                    i == _menu.SelectedIndex ? SelectedEntryKey : EntryKey, i + 1, _menu.GetEntryName(i), costText);
                if (affordable)
                {
                    _builder.Append(line);
                }
                else
                {
                    _builder.Append("<color=#").Append(greyHex).Append('>').Append(line).Append("</color>");
                }

                _builder.Append('\n');
            }

            _builder.Append(_texts.Get(HintKey));
            label.SetText(_builder);
        }
    }
}
