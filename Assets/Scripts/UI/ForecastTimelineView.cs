using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// The forecast timeline bar (design/ux/ui-plan.md 7.3 [B], T2-T5, T7). Builds its uGUI objects at runtime and
    /// redraws every frame from the game time, so the content flows right to left with the clock and stands still
    /// when the clock does. The left edge is "now"; the bar shows <see cref="HudConfig.TimelineRangeSeconds"/> ahead.
    /// Each day is a day segment and a night segment (their real length ratio), a day-start tick with the date,
    /// a night-start tick and one weather icon. Days without a forecast are one muted segment with a "?".
    /// Reads time through <see cref="IDayTimeSource"/> only; the forecast and the rescue request are pushed in.
    /// </summary>
    /// <example>
    /// <code>
    /// var timeline = new ForecastTimelineView(canvasRect, new Vector2(40f, -18f), config, texts, time);
    /// timeline.SetForecast(forecastEvent);
    /// timeline.Refresh(); // every frame
    /// </code>
    /// </example>
    public sealed class ForecastTimelineView : IDisposable
    {
        private const string DayCurrentKey = "hud.timeline.day_current";
        private const string DayKey = "hud.timeline.day";
        private const string UnknownKey = "hud.timeline.unknown";
        private const string TooltipKey = "hud.timeline.tooltip";
        private const string WeatherNamePrefix = "weather.";

        private sealed class Cell
        {
            public RectTransform DayBand;
            public RectTransform NightBand;
            public RectTransform UnknownBand;
            public RectTransform UnknownMark;
            public RectTransform BigTick;
            public RectTransform SmallTick;
            public RectTransform Header;
            public TMP_Text DayLabel;
            public Image IconBox;
            public Image IconBack;
            public Image Icon;
            public int ContentKey = -1;
            public int ContentVersion = -1;
            public bool HasWeather;
            public WeatherInfo Weather;
            public float IconCenterX;
        }

        private readonly HudConfig _config;
        private readonly UiTextTable _texts;
        private readonly IDayTimeSource _time;
        private readonly RectTransform _root;
        private readonly Cell[] _cells;
        private readonly Dictionary<WeatherIconShape, Sprite> _sprites = new Dictionary<WeatherIconShape, Sprite>();
        private readonly RectTransform _flag;
        private readonly RectTransform _tooltip;
        private readonly TMP_Text _tooltipLabel;
        private readonly float _dayTotal;
        private readonly float _headerWidth;

        private WeatherForecastUpdated _forecast;
        private bool _hasForecast;
        private int _forecastVersion;
        private bool _hasRescue;
        private float _rescueArrival;
        private int _hovered = -1;
        private bool _drawn;
        private float _drawnNow;
        private Vector2 _drawnSize;
        private float _drawnRange;
        private int _drawnForecastVersion;
        private bool _drawnHasRescue;
        private float _drawnRescueArrival;

        /// <summary>Builds the bar under <paramref name="parent"/>.</summary>
        /// <param name="parent">Parent rectangle; the bar is anchored to its top center.</param>
        /// <param name="anchoredPosition">Position of the bar's top-left corner relative to the parent's top center.</param>
        /// <param name="config">Sizes and colors.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="time">Game time of day.</param>
        public ForecastTimelineView(RectTransform parent, Vector2 anchoredPosition, HudConfig config, UiTextTable texts, IDayTimeSource time)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _texts = texts ?? throw new ArgumentNullException(nameof(texts));
            _time = time ?? throw new ArgumentNullException(nameof(time));
            _dayTotal = time.DayLength + time.NightLength;
            _headerWidth = config.DayHeaderPadding + config.DayLabelWidth + config.WeatherIconSize;

            var size = config.TimelineSize;
            _root = NewRect("ForecastTimeline", parent);
            _root.anchorMin = new Vector2(0.5f, 1f);
            _root.anchorMax = new Vector2(0.5f, 1f);
            _root.pivot = new Vector2(0f, 1f);
            _root.anchoredPosition = anchoredPosition;
            _root.sizeDelta = size;

            var viewport = NewRect("Viewport", _root);
            Stretch(viewport, 0f);
            viewport.gameObject.AddComponent<RectMask2D>();

            _cells = new Cell[Mathf.CeilToInt(config.TimelineRangeSeconds / _dayTotal) + 1];
            for (var k = 0; k < _cells.Length; k++)
            {
                _cells[k] = BuildCell(viewport, k);
            }

            BuildNowMarker();
            _flag = BuildFlag();
            _tooltip = BuildTooltip(out _tooltipLabel);
        }

        /// <summary>Takes the forecast of the morning. Days after it become the "?" segment.</summary>
        /// <param name="forecast">The forecast event.</param>
        public void SetForecast(WeatherForecastUpdated forecast)
        {
            _forecast = forecast;
            _hasForecast = true;
            _forecastVersion++;
        }

        /// <summary>Puts the rescue flag at now + <paramref name="arrivalDelaySeconds"/> of game time.</summary>
        /// <param name="arrivalDelaySeconds">Game seconds until rescue arrives.</param>
        public void SetRescue(float arrivalDelaySeconds)
        {
            _hasRescue = true;
            _rescueArrival = ElapsedSeconds() + arrivalDelaySeconds;
        }

        /// <summary>
        /// Redraws the bar for the current game time. Call every frame; does nothing when the game time, the bar
        /// size and range, the forecast and the rescue flag are all unchanged since the last call.
        /// </summary>
        public void Refresh()
        {
            var now = ElapsedSeconds();
            var size = _config.TimelineSize;
            var range = _config.TimelineRangeSeconds;
            if (_drawn && now == _drawnNow && size == _drawnSize && range == _drawnRange
                && _forecastVersion == _drawnForecastVersion && _hasRescue == _drawnHasRescue && _rescueArrival == _drawnRescueArrival)
            {
                return;
            }

            _drawn = true;
            _drawnNow = now;
            _drawnSize = size;
            _drawnRange = range;
            _drawnForecastVersion = _forecastVersion;
            _drawnHasRescue = _hasRescue;
            _drawnRescueArrival = _rescueArrival;

            var firstDay = Mathf.Max(_time.Day, 1);
            var width = size.x;
            var perSecond = width / range;
            var dayWidth = _time.DayLength * perSecond;
            var nightWidth = _time.NightLength * perSecond;

            for (var k = 0; k < _cells.Length; k++)
            {
                var cell = _cells[k];
                var day = firstDay + k;
                var start = ((day - 1) * _dayTotal - now) * perSecond;
                var contentKey = day * 2 + (k == 0 ? 1 : 0);
                if (cell.ContentKey != contentKey || cell.ContentVersion != _forecastVersion)
                {
                    cell.ContentKey = contentKey;
                    cell.ContentVersion = _forecastVersion;
                    SetContent(cell, day, k == 0);
                }

                if (cell.HasWeather)
                {
                    Place(cell.DayBand, start, dayWidth);
                    Place(cell.NightBand, start + dayWidth, nightWidth);
                    Place(cell.SmallTick, start + dayWidth - _config.SmallTickWidth * 0.5f, _config.SmallTickWidth);
                }
                else
                {
                    Place(cell.UnknownBand, start, _dayTotal * perSecond);
                    var markLeft = start + _config.DayHeaderPadding + _config.DayLabelWidth;
                    Place(cell.UnknownMark, markLeft, Mathf.Min(start + _dayTotal * perSecond, width) - markLeft);
                }

                Place(cell.BigTick, start - _config.BigTickWidth * 0.5f, _config.BigTickWidth);

                // The header (date and icon) stays at the left edge until its day, night included, has passed.
                var headerX = Mathf.Min(Mathf.Max(start, 0f), start + _dayTotal * perSecond - _headerWidth);
                cell.Header.anchoredPosition = new Vector2(headerX, 0f);
                if (cell.HasWeather)
                {
                    var overNight = headerX + _config.DayHeaderPadding + _config.DayLabelWidth * 0.5f >= start + dayWidth;
                    var labelColor = overNight ? _config.TimelineNightTextColor : _config.TimelineDayTextColor;
                    if (cell.DayLabel.color != labelColor)
                    {
                        cell.DayLabel.color = labelColor;
                    }
                }
                cell.IconCenterX = headerX + _config.DayHeaderPadding + _config.DayLabelWidth + _config.WeatherIconSize * 0.5f;
            }

            RefreshFlag(now, perSecond);
            if (_hovered >= 0)
            {
                PlaceTooltip(_cells[_hovered]);
            }
        }

        /// <summary>Destroys the generated icons and the bar's objects.</summary>
        public void Dispose()
        {
            foreach (var sprite in _sprites.Values)
            {
                UnityEngine.Object.Destroy(sprite.texture);
                UnityEngine.Object.Destroy(sprite);
            }

            _sprites.Clear();
            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
            }
        }

        private float ElapsedSeconds()
        {
            return _time.Day <= 0 ? 0f : (_time.Day - 1) * _dayTotal + _time.SecondsIntoDay;
        }

        private bool TryGetWeather(int day, out WeatherInfo info)
        {
            info = default;
            if (!_hasForecast)
            {
                return false;
            }

            switch (day - _forecast.Day)
            {
                case 0:
                    info = _forecast.Today;
                    return true;
                case 1:
                    info = _forecast.Tomorrow;
                    return true;
                case 2:
                    info = _forecast.DayAfter;
                    return true;
                default:
                    return false;
            }
        }

        private WeatherStyle FindStyle(string weatherId)
        {
            var styles = _config.WeatherStyles;
            for (var i = 0; i < styles.Length; i++)
            {
                if (styles[i].WeatherId == weatherId)
                {
                    return styles[i];
                }
            }

            return null;
        }

        private void SetContent(Cell cell, int day, bool isCurrent)
        {
            cell.HasWeather = TryGetWeather(day, out cell.Weather);
            cell.DayLabel.text = isCurrent ? _texts.Format(DayCurrentKey, day) : _texts.Format(DayKey, day);
            cell.DayLabel.color = cell.HasWeather ? _config.TimelineDayTextColor : _config.TimelineUnknownTextColor;

            cell.DayBand.gameObject.SetActive(cell.HasWeather);
            cell.NightBand.gameObject.SetActive(cell.HasWeather);
            cell.SmallTick.gameObject.SetActive(cell.HasWeather);
            cell.UnknownBand.gameObject.SetActive(!cell.HasWeather);
            cell.UnknownMark.gameObject.SetActive(!cell.HasWeather);

            var style = cell.HasWeather ? FindStyle(cell.Weather.Id) : null;
            cell.IconBox.gameObject.SetActive(style != null);
            var isHovered = _hovered >= 0 && _cells[_hovered] == cell;
            if (style == null)
            {
                if (isHovered)
                {
                    _hovered = -1;
                    _tooltip.gameObject.SetActive(false);
                }

                return;
            }

            if (isHovered)
            {
                WriteTooltip(cell);
            }

            if (!_sprites.TryGetValue(style.Shape, out var sprite))
            {
                sprite = WeatherIconSprites.Create(style.Shape);
                _sprites[style.Shape] = sprite;
            }

            cell.Icon.sprite = sprite;
            cell.Icon.color = style.Color;
            cell.IconBox.color = style.IsDanger ? _config.DangerBorderColor : _config.IconBackColor;
        }

        private Cell BuildCell(RectTransform viewport, int slot)
        {
            var cell = new Cell
            {
                DayBand = NewBand("Day", viewport, _config.TimelineDayColor),
                NightBand = NewBand("Night", viewport, _config.TimelineNightColor),
                UnknownBand = NewBand("Unknown", viewport, _config.TimelineUnknownColor)
            };

            var mark = NewLabel("UnknownMark", viewport, _config.TimelineUnknownTextColor, TextAlignmentOptions.Center);
            mark.text = _texts.Get(UnknownKey);
            cell.UnknownMark = mark.rectTransform;
            SetBand(cell.UnknownMark);

            cell.BigTick = NewBand("BigTick", viewport, _config.TimelineTickColor);
            cell.SmallTick = NewBand("SmallTick", viewport, _config.TimelineTickColor);
            cell.SmallTick.anchorMin = new Vector2(0f, 0.5f - _config.SmallTickHeightRatio * 0.5f);
            cell.SmallTick.anchorMax = new Vector2(0f, 0.5f + _config.SmallTickHeightRatio * 0.5f);

            var height = _config.TimelineSize.y;
            cell.Header = NewRect("Header", viewport);
            cell.Header.anchorMin = new Vector2(0f, 0.5f);
            cell.Header.anchorMax = new Vector2(0f, 0.5f);
            cell.Header.pivot = new Vector2(0f, 0.5f);
            cell.Header.sizeDelta = new Vector2(_headerWidth, height);

            var label = NewLabel("DayLabel", cell.Header, _config.TimelineDayTextColor, TextAlignmentOptions.Left);
            cell.DayLabel = label;
            label.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            label.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            label.rectTransform.pivot = new Vector2(0f, 0.5f);
            label.rectTransform.anchoredPosition = new Vector2(_config.DayHeaderPadding, 0f);
            label.rectTransform.sizeDelta = new Vector2(_config.DayLabelWidth, height);

            var iconSize = _config.WeatherIconSize;
            var box = NewRect("IconBox", cell.Header);
            box.anchorMin = new Vector2(0f, 0.5f);
            box.anchorMax = new Vector2(0f, 0.5f);
            box.pivot = new Vector2(0f, 0.5f);
            box.anchoredPosition = new Vector2(_config.DayHeaderPadding + _config.DayLabelWidth, 0f);
            box.sizeDelta = new Vector2(iconSize, iconSize);
            cell.IconBox = box.gameObject.AddComponent<Image>();
            cell.IconBox.raycastTarget = true;
            box.gameObject.AddComponent<WeatherIconHover>().Initialize(slot, OnIconEntered, OnIconExited);

            var back = NewRect("Back", box);
            Stretch(back, _config.IconBorderThickness);
            cell.IconBack = back.gameObject.AddComponent<Image>();
            cell.IconBack.color = _config.IconBackColor;
            cell.IconBack.raycastTarget = false;

            var icon = NewRect("Icon", back);
            Stretch(icon, 1f);
            cell.Icon = icon.gameObject.AddComponent<Image>();
            cell.Icon.raycastTarget = false;
            return cell;
        }

        private void BuildNowMarker()
        {
            var line = NewRect("NowMarker", _root);
            line.anchorMin = new Vector2(0f, 0f);
            line.anchorMax = new Vector2(0f, 1f);
            line.pivot = new Vector2(0.5f, 0.5f);
            line.sizeDelta = new Vector2(_config.NowMarkerWidth, 0f);
            line.anchoredPosition = Vector2.zero;
            var lineImage = line.gameObject.AddComponent<Image>();
            lineImage.color = _config.NowMarkerColor;
            lineImage.raycastTarget = false;

            var diamond = NewRect("NowDiamond", line);
            diamond.anchorMin = new Vector2(0.5f, 1f);
            diamond.anchorMax = new Vector2(0.5f, 1f);
            diamond.pivot = new Vector2(0.5f, 0.5f);
            diamond.anchoredPosition = Vector2.zero;
            diamond.sizeDelta = new Vector2(_config.NowMarkerDiamondSize, _config.NowMarkerDiamondSize);
            diamond.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var diamondImage = diamond.gameObject.AddComponent<Image>();
            diamondImage.color = _config.NowMarkerColor;
            diamondImage.raycastTarget = false;
        }

        private RectTransform BuildFlag()
        {
            var flag = NewRect("RescueFlag", _root);
            flag.anchorMin = Vector2.zero;
            flag.anchorMax = Vector2.zero;
            flag.pivot = Vector2.zero;
            flag.sizeDelta = Vector2.zero;

            var poleHeight = _config.TimelineSize.y + _config.RescueFlagRise;
            var pole = NewRect("Pole", flag);
            pole.anchorMin = Vector2.zero;
            pole.anchorMax = Vector2.zero;
            pole.pivot = new Vector2(0.5f, 0f);
            pole.anchoredPosition = Vector2.zero;
            pole.sizeDelta = new Vector2(_config.RescueFlagPoleWidth, poleHeight);
            var poleImage = pole.gameObject.AddComponent<Image>();
            poleImage.color = _config.RescueFlagColor;
            poleImage.raycastTarget = false;

            var pennant = NewRect("Pennant", flag);
            pennant.anchorMin = Vector2.zero;
            pennant.anchorMax = Vector2.zero;
            pennant.pivot = new Vector2(0f, 1f);
            pennant.anchoredPosition = new Vector2(_config.RescueFlagPoleWidth * 0.5f, poleHeight);
            pennant.sizeDelta = _config.RescueFlagSize;
            var pennantImage = pennant.gameObject.AddComponent<Image>();
            pennantImage.color = _config.RescueFlagColor;
            pennantImage.raycastTarget = false;

            flag.gameObject.SetActive(false);
            return flag;
        }

        private RectTransform BuildTooltip(out TMP_Text label)
        {
            var panel = NewRect("Tooltip", _root);
            panel.anchorMin = new Vector2(0f, 1f);
            panel.anchorMax = new Vector2(0f, 1f);
            panel.pivot = new Vector2(0.5f, 1f);
            var background = panel.gameObject.AddComponent<Image>();
            background.color = _config.TooltipPanelColor;
            background.raycastTarget = false;

            label = NewLabel("Text", panel, Color.white, TextAlignmentOptions.Center);
            label.fontSize = _config.TooltipFontSize;
            Stretch(label.rectTransform, 0f);
            panel.gameObject.SetActive(false);
            return panel;
        }

        private void OnIconEntered(int slot)
        {
            _hovered = slot;
            WriteTooltip(_cells[slot]);
            PlaceTooltip(_cells[slot]);
            _tooltip.gameObject.SetActive(true);
        }

        private void OnIconExited(int slot)
        {
            if (_hovered == slot)
            {
                _hovered = -1;
                _tooltip.gameObject.SetActive(false);
            }
        }

        private void WriteTooltip(Cell cell)
        {
            var text = _texts.Format(TooltipKey, _texts.Get(WeatherNamePrefix + cell.Weather.Id), cell.Weather.TemperatureOffset);
            _tooltipLabel.text = text;
            _tooltip.sizeDelta = _tooltipLabel.GetPreferredValues(text) + _config.TooltipPadding * 2f;
        }

        private void PlaceTooltip(Cell cell)
        {
            _tooltip.anchoredPosition = new Vector2(cell.IconCenterX, -(_config.TimelineSize.y + _config.TooltipOffset));
        }

        private void RefreshFlag(float now, float perSecond)
        {
            var remaining = _rescueArrival - now;
            var visible = _hasRescue && remaining >= 0f && remaining <= _config.TimelineRangeSeconds;
            _flag.gameObject.SetActive(visible);
            if (visible)
            {
                _flag.anchoredPosition = new Vector2(remaining * perSecond, 0f);
            }
        }

        private RectTransform NewBand(string name, RectTransform parent, Color color)
        {
            var rect = NewRect(name, parent);
            SetBand(rect);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private TMP_Text NewLabel(string name, RectTransform parent, Color color, TextAlignmentOptions alignment)
        {
            var rect = NewRect(name, parent);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            _texts.ApplyFont(label);
            label.fontSize = _config.TimelineFontSize;
            label.color = color;
            label.alignment = alignment;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            return label;
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        private static void SetBand(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = Vector2.zero;
        }

        private static void Stretch(RectTransform rect, float inset)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        private static void Place(RectTransform rect, float x, float width)
        {
            rect.anchoredPosition = new Vector2(x, 0f);
            rect.sizeDelta = new Vector2(Mathf.Max(width, 0f), 0f);
        }
    }
}
