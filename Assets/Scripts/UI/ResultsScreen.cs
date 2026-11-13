using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// The results screen (design/ux/ui-plan.md S10, core-rules section 1, tech-decisions 34). Hidden until
    /// <see cref="RunEnded"/>; then a full-screen backdrop that blocks clicks and a centered panel fade in: a title
    /// for the way the run ended, the days survived, the cause of death (only after a death), the chosen augments
    /// (none yet: augments are story-005) and a restart button that calls <see cref="IRunRestarter"/> once.
    /// Built in code, so it needs no prefab. The fade uses unscaled time.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on an empty RectTransform stretched over the HUD canvas, as its last child.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;ResultsScreen&gt;().
    /// </code>
    /// </example>
    public sealed class ResultsScreen : MonoBehaviour
    {
        private const string TitleKeyPrefix = "result.title.";
        private const string RescuedTitleSuffix = "Rescued";
        private const string DaysKey = "result.days";
        private const string CauseKey = "result.cause";
        private const string CauseKeyPrefix = "result.cause.";
        private const string AugmentsKey = "result.augments";
        private const string NoAugmentsKey = "result.augments.none";
        private const string RestartKey = "result.restart";
        private const float LineHeightRatio = 1.4f;

        private IRunRestarter _restarter;
        private UiTextTable _texts;
        private HudConfig _config;
        private IDisposable _endedSubscription;
        private GameObject _root;
        private CanvasGroup _group;
        private TMP_Text _titleLabel;
        private TMP_Text _daysLabel;
        private TMP_Text _causeLabel;
        private TMP_Text _augmentsLabel;
        private Button _restartButton;
        private RunEnded _result;
        private bool _hasResult;
        private bool _shown;
        private bool _restartClicked;
        private float _fadeAge;

        /// <summary>Receives dependencies from the container and subscribes to <see cref="RunEnded"/>.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="restarter">Starts a new run.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Fade time, sizes and colors.</param>
        [Inject]
        public void Construct(IEventBus bus, IRunRestarter restarter, UiTextTable texts, HudConfig config)
        {
            _restarter = restarter;
            _texts = texts;
            _config = config;
            _endedSubscription?.Dispose();
            _endedSubscription = bus.Subscribe<RunEnded>(OnRunEnded);
        }

        private void Start()
        {
            Build();
            if (_hasResult)
            {
                Show();
            }
        }

        private void Update()
        {
            if (!_shown || _group.alpha >= 1f)
            {
                return;
            }

            _fadeAge += Time.unscaledDeltaTime;
            _group.alpha = _config.ResultFadeSeconds > 0f ? Mathf.Clamp01(_fadeAge / _config.ResultFadeSeconds) : 1f;
        }

        private void OnDestroy()
        {
            _endedSubscription?.Dispose();
            _endedSubscription = null;
        }

        private void OnRunEnded(RunEnded ended)
        {
            if (_hasResult)
            {
                return;
            }

            _result = ended;
            _hasResult = true;
            if (_root != null)
            {
                Show();
            }
        }

        private void Build()
        {
            var rootRect = HudBuilder.NewRect("Results", transform);
            HudBuilder.Stretch(rootRect);
            _root = rootRect.gameObject;
            _group = _root.AddComponent<CanvasGroup>();
            _group.alpha = 0f;

            var backdrop = HudBuilder.NewImage("Backdrop", rootRect, null, _config.ResultBackdropColor);
            backdrop.raycastTarget = true;
            HudBuilder.Stretch(backdrop.rectTransform);

            var panel = HudBuilder.NewImage("Panel", rootRect, null, _config.ResultPanelColor).rectTransform;
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            panel.sizeDelta = _config.ResultPanelSize;

            _titleLabel = NewRow("Title", panel, _config.ResultTitleFontSize);
            _daysLabel = NewRow("Days", panel, _config.ResultBodyFontSize);
            _causeLabel = NewRow("Cause", panel, _config.ResultBodyFontSize);
            _augmentsLabel = NewRow("Augments", panel, _config.ResultBodyFontSize);

            _restartButton = HudBuilder.NewButton(
                "Restart", panel, _texts, _texts.Get(RestartKey), _config.ResultBodyFontSize, _config.ResultButtonTextColor,
                _config.ResultButtonColor, _config.ResultButtonHighlightColor, _config.ResultButtonDisabledColor);
            var buttonRect = (RectTransform)_restartButton.transform;
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.anchoredPosition = new Vector2(0f, _config.ResultPadding);
            buttonRect.sizeDelta = _config.ResultButtonSize;
            _restartButton.onClick.AddListener(OnRestartClicked);

            _root.SetActive(false);
        }

        private TMP_Text NewRow(string name, RectTransform panel, float fontSize)
        {
            var label = HudBuilder.NewLabel(name, panel, _texts, fontSize, _config.TextColor, TextAlignmentOptions.Center);
            var rect = label.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, fontSize * LineHeightRatio);
            return label;
        }

        private void Show()
        {
            var titleSuffix = _result.Rescued ? RescuedTitleSuffix : _result.Cause.ToString();
            _titleLabel.text = _texts.Get(TitleKeyPrefix + titleSuffix);
            _daysLabel.text = _texts.Format(DaysKey, _result.Day);
            _causeLabel.gameObject.SetActive(!_result.Rescued);
            if (!_result.Rescued)
            {
                _causeLabel.text = _texts.Format(CauseKey, _texts.Get(CauseKeyPrefix + _result.Cause));
            }

            _augmentsLabel.text = _texts.Format(AugmentsKey, _texts.Get(NoAugmentsKey));

            var y = -_config.ResultPadding;
            y = PlaceRow(_titleLabel, y) - _config.ResultRowSpacing;
            y = PlaceRow(_daysLabel, y);
            if (!_result.Rescued)
            {
                y = PlaceRow(_causeLabel, y);
            }

            PlaceRow(_augmentsLabel, y);

            _fadeAge = 0f;
            _group.alpha = 0f;
            _root.SetActive(true);
            _restartButton.Select();
            _shown = true;
        }

        private float PlaceRow(TMP_Text label, float y)
        {
            var rect = label.rectTransform;
            rect.anchoredPosition = new Vector2(0f, y);
            return y - rect.sizeDelta.y - _config.ResultRowSpacing;
        }

        private void OnRestartClicked()
        {
            if (_restartClicked)
            {
                return;
            }

            _restartClicked = true;
            _restartButton.interactable = false;
            _restarter.Restart();
        }
    }
}
