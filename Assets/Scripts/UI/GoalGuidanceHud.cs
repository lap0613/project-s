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
    /// The rescue goal on screen (design/ux/ui-plan.md 7.3 [C] and 7.6 N1, N2). From the first frame of a run it
    /// shows the beacon part progress under the timeline bar: a beacon icon, one icon per part (faint until the
    /// player has it, with a check mark once it is installed on the beacon) and the count, e.g. 0/3. After rescue is
    /// requested the same spot shows the time until rescue instead (tech-decisions 34). Also shows the start guide once per run (the Game scene is built
    /// again when a run restarts): a picture row (parts, beacon, rescue) with a short phrase, which stays at full
    /// size for a while and then shrinks into the progress row. The game never stops for it. It says nothing about
    /// where or when parts appear (ui-plan D8). Part states come from <see cref="IRescueParts"/>, the rescue time
    /// from <see cref="IRescueCountdown"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a full-canvas RectTransform (together with ReliefFeedbackHud).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;GoalGuidanceHud&gt;().
    /// </code>
    /// </example>
    public sealed class GoalGuidanceHud : MonoBehaviour
    {
        private const string GuideKey = "hud.goal.guide";
        private const string CountKey = "hud.parts.count";
        private const string TimerKey = "hud.rescue.timer";
        private const string TimerShortKey = "hud.rescue.timer_short";
        private const int SecondsPerMinute = 60;
        private const float RowPadding = 8f;
        private const float GuidePadding = 14f;
        private const float GuideFadeInSeconds = 0.25f;
        private static readonly GoalIconShape[] PartShapes = { GoalIconShape.Antenna, GoalIconShape.Battery, GoalIconShape.Transmitter };

        private readonly List<Sprite> _sprites = new List<Sprite>();
        private IRescueParts _parts;
        private IRescueCountdown _countdown;
        private IDayTimeSource _clock;
        private UiTextTable _texts;
        private HudConfig _config;
        private RectTransform _row;
        private Vector2 _rowSize;
        private GameObject _partsContent;
        private Image[] _partIcons;
        private GameObject[] _partChecks;
        private RescuePartState[] _shownStates;
        private TMP_Text _countLabel;
        private TMP_Text _timerLabel;
        private bool _statesShown;
        private int _shownTimerSeconds = -1;
        private RectTransform _guide;
        private CanvasGroup _guideGroup;
        private Vector2 _guideStart;
        private Vector2 _guideEnd;
        private float _guideEndScale;
        private float _guideAge;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="parts">Beacon parts and where each one is.</param>
        /// <param name="countdown">The rescue timer.</param>
        /// <param name="clock">Day and night lengths, to show the remaining time in days.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Sizes, colors and times.</param>
        [Inject]
        public void Construct(
            IRescueParts parts, IRescueCountdown countdown, IDayTimeSource clock, UiTextTable texts, HudConfig config)
        {
            _parts = parts;
            _countdown = countdown;
            _clock = clock;
            _texts = texts;
            _config = config;
        }

        private void Start()
        {
            var rowTopLeft = _config.TimelineAnchoredPosition
                + new Vector2(0f, -_config.TimelineSize.y - _config.GoalGapBelowTimeline);
            var rowSize = BuildProgressRow(rowTopLeft);
            BuildGuide(rowTopLeft, rowSize);
            Refresh();
        }

        private void Update()
        {
            Refresh();
            if (_guide != null)
            {
                AnimateGuide();
            }
        }

        private Vector2 BuildProgressRow(Vector2 topLeft)
        {
            var iconSize = _config.GoalIconSize;
            var required = Mathf.Max(_parts.Required, 1);
            var countWidth = _config.GoalCountFontSize * 2.6f;
            var width = RowPadding * 2f + iconSize + _config.GoalIconSpacing * 2f + required * iconSize + countWidth;
            var height = iconSize + RowPadding * 2f;

            _row = HudBuilder.NewRect("GoalProgress", transform);
            _rowSize = new Vector2(width, height);
            HudBuilder.PlaceTopCenter(_row, new Vector2(0f, 1f), topLeft, _rowSize);
            HudBuilder.Stretch(HudBuilder.NewImage("Back", _row, null, _config.GoalBackColor).rectTransform);

            var content = HudBuilder.NewRect("Parts", _row);
            HudBuilder.Stretch(content);
            _partsContent = content.gameObject;

            var x = RowPadding;
            AddRowIcon(content, "Beacon", GoalIconShape.Beacon, _config.GoalBeaconColor, x, iconSize);
            x += iconSize + _config.GoalIconSpacing;
            _partIcons = new Image[required];
            _partChecks = new GameObject[required];
            _shownStates = new RescuePartState[required];
            for (var i = 0; i < required; i++)
            {
                _partIcons[i] = AddRowIcon(content, "Part" + i, PartShapes[i % PartShapes.Length], _config.GoalPartMissingColor, x, iconSize);
                _partChecks[i] = AddCheck(_partIcons[i].rectTransform);
                x += iconSize;
            }

            x += _config.GoalIconSpacing;
            _countLabel = HudBuilder.NewLabel("Count", content, _texts, _config.GoalCountFontSize, _config.GoalBeaconColor, TextAlignmentOptions.MidlineLeft);
            var labelRect = _countLabel.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0f, 1f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(x, 0f);
            labelRect.sizeDelta = new Vector2(countWidth, 0f);

            _timerLabel = HudBuilder.NewLabel(
                "RescueTimer", _row, _texts, _config.RescueTimerFontSize, _config.RescueTimerColor, TextAlignmentOptions.Center);
            HudBuilder.Stretch(_timerLabel.rectTransform);
            _timerLabel.gameObject.SetActive(false);
            return _rowSize;
        }

        private GameObject AddCheck(RectTransform icon)
        {
            var sprite = GoalIconSprites.Create(GoalIconShape.Check);
            _sprites.Add(sprite);
            var image = HudBuilder.NewImage("Check", icon, sprite, _config.GoalPartInstalledCheckColor);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(_config.GoalCheckSize, _config.GoalCheckSize);
            image.gameObject.SetActive(false);
            return image.gameObject;
        }

        private Image AddRowIcon(RectTransform parent, string name, GoalIconShape shape, Color color, float x, float size)
        {
            var sprite = GoalIconSprites.Create(shape);
            _sprites.Add(sprite);
            var image = HudBuilder.NewImage(name, parent, sprite, color);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, 0f);
            rect.sizeDelta = new Vector2(size, size);
            return image;
        }

        private void BuildGuide(Vector2 rowTopLeft, Vector2 rowSize)
        {
            var iconSize = _config.GuideIconSize;
            var required = Mathf.Max(_parts.Required, 1);
            var gap = iconSize * 0.25f;
            var arrowWidth = iconSize * 0.6f;
            var phrase = _texts.Get(GuideKey);
            var iconRowWidth = required * iconSize + gap + arrowWidth + gap + iconSize + gap + arrowWidth + gap + iconSize;
            var width = Mathf.Max(iconRowWidth, phrase.Length * _config.GuideFontSize * 1.05f) + GuidePadding * 2f;
            var height = iconSize + _config.GuideFontSize * 1.5f + GuidePadding * 2.5f;

            _guideStart = new Vector2(0f, rowTopLeft.y - rowSize.y - _config.GuideTopOffset - height * 0.5f);
            _guideEnd = new Vector2(rowTopLeft.x + rowSize.x * 0.5f, rowTopLeft.y - rowSize.y * 0.5f);
            _guideEndScale = Mathf.Min(rowSize.x / width, rowSize.y / height);

            _guide = HudBuilder.NewRect("StartGuide", transform);
            HudBuilder.PlaceTopCenter(_guide, new Vector2(0.5f, 0.5f), _guideStart, new Vector2(width, height));
            _guideGroup = _guide.gameObject.AddComponent<CanvasGroup>();
            _guideGroup.alpha = 0f;
            _guideGroup.blocksRaycasts = false;
            _guideGroup.interactable = false;
            HudBuilder.Stretch(HudBuilder.NewImage("Back", _guide, null, _config.GuideBackColor).rectTransform);

            var x = (width - iconRowWidth) * 0.5f;
            var y = -GuidePadding - iconSize * 0.5f;
            for (var i = 0; i < required; i++)
            {
                AddGuideIcon("Part" + i, PartShapes[i % PartShapes.Length], _config.GoalPartHaveColor, ref x, y, iconSize);
            }

            x += gap;
            AddGuideIcon("Arrow1", GoalIconShape.Arrow, _config.GuideArrowColor, ref x, y, arrowWidth);
            x += gap;
            AddGuideIcon("Beacon", GoalIconShape.Beacon, _config.GoalBeaconColor, ref x, y, iconSize);
            x += gap;
            AddGuideIcon("Arrow2", GoalIconShape.Arrow, _config.GuideArrowColor, ref x, y, arrowWidth);
            x += gap;
            AddGuideIcon("Rescue", GoalIconShape.Rescue, _config.MorningSunColor, ref x, y, iconSize);

            var label = HudBuilder.NewLabel("Phrase", _guide, _texts, _config.GuideFontSize, _config.TextColor, TextAlignmentOptions.Center);
            label.text = phrase;
            var labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 0f);
            labelRect.pivot = new Vector2(0.5f, 0f);
            labelRect.anchoredPosition = new Vector2(0f, GuidePadding);
            labelRect.sizeDelta = new Vector2(0f, _config.GuideFontSize * 1.5f);
        }

        private void AddGuideIcon(string name, GoalIconShape shape, Color color, ref float x, float y, float width)
        {
            var sprite = GoalIconSprites.Create(shape);
            _sprites.Add(sprite);
            var image = HudBuilder.NewImage(name, _guide, sprite, color);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, _config.GuideIconSize);
            x += width;
        }

        private void AnimateGuide()
        {
            _guideAge += Time.deltaTime;
            var full = _config.GuideSeconds;
            if (_guideAge < full)
            {
                _guideGroup.alpha = Mathf.Clamp01(_guideAge / GuideFadeInSeconds);
                return;
            }

            var u = (_guideAge - full) / _config.GuideShrinkSeconds;
            if (u >= 1f)
            {
                Destroy(_guide.gameObject);
                _guide = null;
                return;
            }

            var eased = u * u * (3f - 2f * u);
            _guide.anchoredPosition = Vector2.Lerp(_guideStart, _guideEnd, eased);
            _guide.localScale = Vector3.one * Mathf.Lerp(1f, _guideEndScale, eased);
            _guideGroup.alpha = 1f - eased;
        }

        private void Refresh()
        {
            if (_countdown.IsActive)
            {
                RefreshTimer();
                return;
            }

            var changed = !_statesShown;
            var collected = 0;
            for (var i = 0; i < _partIcons.Length; i++)
            {
                var state = _parts.GetPartState(i);
                changed |= state != _shownStates[i];
                _shownStates[i] = state;
                if (state != RescuePartState.Missing)
                {
                    collected++;
                }
            }

            if (!changed)
            {
                return;
            }

            _statesShown = true;
            for (var i = 0; i < _partIcons.Length; i++)
            {
                _partIcons[i].color = _shownStates[i] == RescuePartState.Missing
                    ? _config.GoalPartMissingColor
                    : _config.GoalPartHaveColor;
                _partChecks[i].SetActive(_shownStates[i] == RescuePartState.Installed);
            }

            _countLabel.text = _texts.Format(CountKey, collected, _partIcons.Length);
        }

        private void RefreshTimer()
        {
            var totalSeconds = Mathf.CeilToInt(Mathf.Max(_countdown.RemainingSeconds, 0f));
            if (totalSeconds == _shownTimerSeconds)
            {
                return;
            }

            if (_shownTimerSeconds < 0)
            {
                _partsContent.SetActive(false);
                _timerLabel.gameObject.SetActive(true);
            }

            _shownTimerSeconds = totalSeconds;
            var fullDay = _clock.DayLength + _clock.NightLength;
            var days = fullDay > 0f ? Mathf.FloorToInt(totalSeconds / fullDay) : 0;
            var rest = Mathf.Max(Mathf.FloorToInt(totalSeconds - days * fullDay), 0);
            var minutes = rest / SecondsPerMinute;
            var seconds = rest % SecondsPerMinute;
            _timerLabel.text = days >= 1
                ? _texts.Format(TimerKey, days, minutes, seconds)
                : _texts.Format(TimerShortKey, minutes, seconds);

            // The timer text can be wider than the part row; the row only grows, so it does not jitter.
            var width = Mathf.Max(_row.sizeDelta.x, _timerLabel.preferredWidth + RowPadding * 2f);
            _row.sizeDelta = new Vector2(width, _rowSize.y);
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _sprites.Count; i++)
            {
                HudBuilder.DestroySprite(_sprites[i]);
            }
        }
    }
}
