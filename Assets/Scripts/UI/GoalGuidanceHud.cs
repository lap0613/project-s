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
    /// player has it) and the count, e.g. 0/3. Also shows the start guide once per run (the Game scene is built
    /// again when a run restarts): a picture row (parts, beacon, rescue) with a short phrase, which stays at full
    /// size for a while and then shrinks into the progress row. The game never stops for it. It says nothing about
    /// where or when parts appear (ui-plan D8). Part counts come from <see cref="IRescueParts"/>.
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
        private const float RowPadding = 8f;
        private const float GuidePadding = 14f;
        private const float GuideFadeInSeconds = 0.25f;
        private static readonly GoalIconShape[] PartShapes = { GoalIconShape.Antenna, GoalIconShape.Battery, GoalIconShape.Transmitter };

        private readonly List<Sprite> _sprites = new List<Sprite>();
        private IRescueParts _parts;
        private UiTextTable _texts;
        private HudConfig _config;
        private Image[] _partIcons;
        private TMP_Text _countLabel;
        private int _shownCollected = -1;
        private RectTransform _guide;
        private CanvasGroup _guideGroup;
        private Vector2 _guideStart;
        private Vector2 _guideEnd;
        private float _guideEndScale;
        private float _guideAge;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="parts">Collected and required beacon parts.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Sizes, colors and times.</param>
        [Inject]
        public void Construct(IRescueParts parts, UiTextTable texts, HudConfig config)
        {
            _parts = parts;
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

            var row = HudBuilder.NewRect("GoalProgress", transform);
            HudBuilder.PlaceTopCenter(row, new Vector2(0f, 1f), topLeft, new Vector2(width, height));
            HudBuilder.Stretch(HudBuilder.NewImage("Back", row, null, _config.GoalBackColor).rectTransform);

            var x = RowPadding;
            AddRowIcon(row, "Beacon", GoalIconShape.Beacon, _config.GoalBeaconColor, x, iconSize);
            x += iconSize + _config.GoalIconSpacing;
            _partIcons = new Image[required];
            for (var i = 0; i < required; i++)
            {
                _partIcons[i] = AddRowIcon(row, "Part" + i, PartShapes[i % PartShapes.Length], _config.GoalPartMissingColor, x, iconSize);
                x += iconSize;
            }

            x += _config.GoalIconSpacing;
            _countLabel = HudBuilder.NewLabel("Count", row, _texts, _config.GoalCountFontSize, _config.GoalBeaconColor, TextAlignmentOptions.MidlineLeft);
            var labelRect = _countLabel.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0f, 1f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(x, 0f);
            labelRect.sizeDelta = new Vector2(countWidth, 0f);
            return new Vector2(width, height);
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
            var collected = Mathf.Clamp(_parts.Collected, 0, _partIcons.Length);
            if (collected == _shownCollected)
            {
                return;
            }

            _shownCollected = collected;
            for (var i = 0; i < _partIcons.Length; i++)
            {
                _partIcons[i].color = i < collected ? _config.GoalPartHaveColor : _config.GoalPartMissingColor;
            }

            _countLabel.text = _texts.Format(CountKey, collected, _partIcons.Length);
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
