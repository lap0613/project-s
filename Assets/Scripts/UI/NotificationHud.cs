using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// The notice area, bottom-left (ui-plan section 7.3 [E]): at most a few lines, newest at the bottom, each gone
    /// after a few seconds (both from <see cref="HudConfig"/>). Shows every <see cref="NoticePosted"/> (bag full,
    /// missing flint, missing materials) and "the fire went out" from <see cref="FireStateChanged"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign a bottom-aligned TextMeshProUGUI as Label.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;NotificationHud&gt;().
    /// </code>
    /// </example>
    public sealed class NotificationHud : MonoBehaviour
    {
        private const string FireOutKey = "notice.fire_out";

        [SerializeField] private TMP_Text label;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<string> _lines = new List<string>();
        private readonly List<float> _expiry = new List<float>();
        private readonly StringBuilder _builder = new StringBuilder();
        private UiTextTable _texts;
        private HudConfig _config;
        private bool _dirty = true;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Line count and duration.</param>
        [Inject]
        public void Construct(IEventBus bus, UiTextTable texts, HudConfig config)
        {
            _texts = texts;
            _config = config;
            _subscriptions.Add(bus.Subscribe<NoticePosted>(e => Push(_texts.Format(e.TextKey, e.Argument))));
            _subscriptions.Add(bus.Subscribe<FireStateChanged>(e =>
            {
                if (!e.IsLit)
                {
                    Push(_texts.Get(FireOutKey));
                }
            }));
        }

        private void Start()
        {
            _texts.ApplyFont(label);
        }

        private void Update()
        {
            while (_expiry.Count > 0 && Time.time >= _expiry[0])
            {
                RemoveOldest();
            }

            if (!_dirty)
            {
                return;
            }

            _dirty = false;
            _builder.Clear();
            for (var i = 0; i < _lines.Count; i++)
            {
                if (i > 0)
                {
                    _builder.Append('\n');
                }

                _builder.Append(_lines[i]);
            }

            label.SetText(_builder);
        }

        private void Push(string text)
        {
            while (_lines.Count >= _config.NoticeMaxLines)
            {
                RemoveOldest();
            }

            _lines.Add(text);
            _expiry.Add(Time.time + _config.NoticeSeconds);
            _dirty = true;
        }

        private void RemoveOldest()
        {
            _lines.RemoveAt(0);
            _expiry.RemoveAt(0);
            _dirty = true;
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Dispose();
            }
        }
    }
}
