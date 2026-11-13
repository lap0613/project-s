using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// The bag: a row of slots at the bottom of the HUD, always visible (ui-plan sections 7.3 [G] and 10, D5).
    /// The number of slots follows the bag-slots stat. The selected slot has a highlighted border.
    /// Right-clicking a slot opens a menu whose entries are every registered <see cref="IBagAction"/> that applies
    /// to the slot's item, lowest order first (use, drop; story 008 adds equip by registering another action).
    /// Redraws only when the bag's version changes.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign Slot Row (Horizontal Layout Group), Slot Prefab, Menu Root (inactive panel
    /// // with a Vertical Layout Group) and Menu Entry Prefab (Button with a TextMeshProUGUI child).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;BagHud&gt;().
    /// </code>
    /// </example>
    public sealed class BagHud : MonoBehaviour
    {
        private const string FilledSlotKey = "bag.slot_filled";
        private const string EmptySlotKey = "bag.slot_empty";

        [SerializeField] private RectTransform slotRow;
        [SerializeField] private BagSlotView slotPrefab;
        [SerializeField] private RectTransform menuRoot;
        [SerializeField] private Button menuEntryPrefab;

        private readonly List<BagSlotView> _slots = new List<BagSlotView>();
        private readonly List<IBagAction> _actions = new List<IBagAction>();
        private IBag _bag;
        private IInteractionInput _input;
        private UiTextTable _texts;
        private HudConfig _config;
        private int _shownVersion = -1;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bag">The bag to show.</param>
        /// <param name="actions">Every slot menu entry in the game.</param>
        /// <param name="input">Cancel button (closes the menu).</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Slot colors.</param>
        [Inject]
        public void Construct(
            IBag bag, IReadOnlyList<IBagAction> actions, IInteractionInput input, UiTextTable texts, HudConfig config)
        {
            _bag = bag;
            _input = input;
            _texts = texts;
            _config = config;
            _actions.AddRange(actions);
            _actions.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        /// <summary>Selects a slot (mouse path; number keys do the same through the Resources system).</summary>
        /// <param name="index">Zero-based slot index.</param>
        public void Select(int index)
        {
            _bag.SelectSlot(index);
        }

        /// <summary>Opens the slot menu at a screen position. Does nothing for an empty slot.</summary>
        /// <param name="index">Zero-based slot index.</param>
        /// <param name="screenPosition">Where the pointer is.</param>
        public void OpenMenu(int index, Vector2 screenPosition)
        {
            CloseMenu();
            var slot = _bag.GetSlot(index);
            if (slot.IsEmpty)
            {
                return;
            }

            for (var i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                if (!action.AppliesTo(slot.Item))
                {
                    continue;
                }

                var entry = Instantiate(menuEntryPrefab, menuRoot);
                var entryLabel = entry.GetComponentInChildren<TMP_Text>();
                _texts.ApplyFont(entryLabel);
                entryLabel.text = _texts.Get(action.LabelKey);
                entry.onClick.AddListener(() =>
                {
                    action.Execute(index);
                    CloseMenu();
                });
            }

            menuRoot.position = screenPosition;
            menuRoot.gameObject.SetActive(true);
        }

        private void Start()
        {
            menuRoot.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (menuRoot.gameObject.activeSelf && _input.CancelPressed)
            {
                CloseMenu();
            }

            if (_bag.Version == _shownVersion && _slots.Count == _bag.SlotCount)
            {
                return;
            }

            _shownVersion = _bag.Version;
            CloseMenu();
            Redraw();
        }

        private void Redraw()
        {
            var count = _bag.SlotCount;
            while (_slots.Count < count)
            {
                var slot = Instantiate(slotPrefab, slotRow);
                slot.Bind(this, _slots.Count);
                _texts.ApplyFont(slot.Label);
                _slots.Add(slot);
            }

            while (_slots.Count > count)
            {
                Destroy(_slots[_slots.Count - 1].gameObject);
                _slots.RemoveAt(_slots.Count - 1);
            }

            for (var i = 0; i < count; i++)
            {
                var info = _bag.GetSlot(i);
                var text = info.IsEmpty
                    ? _texts.Format(EmptySlotKey, i + 1)
                    : _texts.Format(FilledSlotKey, i + 1, info.DisplayName, info.Count);
                _slots[i].Show(text, i == _bag.SelectedSlot ? _config.SlotSelectedBorderColor : _config.SlotBorderColor);
            }
        }

        private void CloseMenu()
        {
            if (!menuRoot.gameObject.activeSelf && menuRoot.childCount == 0)
            {
                return;
            }

            for (var i = menuRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(menuRoot.GetChild(i).gameObject);
            }

            menuRoot.gameObject.SetActive(false);
        }
    }
}
