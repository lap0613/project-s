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
    /// World UI drawn on the HUD canvas at the screen position of its target (ui-plan section 8):
    /// the prompt of the nearest E-action ("[E] 줍기 — 나무"; greyed when it cannot be done), a progress bar while E is
    /// held (gather, cook, fuel), and above the nearest campfire its fuel pips (one per wood) with the seconds left.
    /// What counts as "near" is decided by the systems (interaction ranges, the campfire's status range of 6 m).
    /// Needs a Screen Space - Overlay canvas. Texts are rebuilt only when they change.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on the HUD Canvas; assign Game Camera, Prompt Root + Label, Progress Root + Fill (anchors (0,0)-(1,1)),
    /// // Fuel Root + Pip Row (Horizontal Layout Group) + Pip Prefab (Image) + Seconds Label.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;WorldUiHud&gt;().
    /// </code>
    /// </example>
    public sealed class WorldUiHud : MonoBehaviour
    {
        private const string FuelSecondsKey = "world.fuel_seconds";

        [SerializeField] private Camera gameCamera;

        [Header("Prompt")]
        [SerializeField] private RectTransform promptRoot;
        [SerializeField] private TMP_Text promptLabel;

        [Header("Hold progress")]
        [SerializeField] private RectTransform progressRoot;
        [SerializeField] private RectTransform progressFill;

        [Header("Campfire fuel")]
        [SerializeField] private RectTransform fuelRoot;
        [SerializeField] private RectTransform fuelPipRow;
        [SerializeField] private Image fuelPipPrefab;
        [SerializeField] private TMP_Text fuelSecondsLabel;

        private readonly List<Image> _pips = new List<Image>();
        private IInteractionStatus _interaction;
        private IFireStatus _fire;
        private UiTextTable _texts;
        private HudConfig _config;
        private string _shownPromptKey;
        private string _shownPromptArgument;
        private bool _shownPromptAvailable;
        private int _shownSeconds = -1;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="interaction">Current E-action prompt and progress.</param>
        /// <param name="fire">Nearest campfire status.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Offsets and colors.</param>
        [Inject]
        public void Construct(IInteractionStatus interaction, IFireStatus fire, UiTextTable texts, HudConfig config)
        {
            _interaction = interaction;
            _fire = fire;
            _texts = texts;
            _config = config;
        }

        private void Start()
        {
            _texts.ApplyFont(promptLabel);
            _texts.ApplyFont(fuelSecondsLabel);
        }

        private void LateUpdate()
        {
            UpdatePrompt();
            UpdateFuel();
        }

        private void UpdatePrompt()
        {
            var prompt = _interaction.Prompt;
            var visible = _interaction.HasPrompt
                && Place(promptRoot, prompt.X, prompt.Y + _config.PromptHeight, prompt.Z);
            SetActive(promptRoot, visible);
            SetActive(progressRoot, visible && _interaction.IsHolding);
            if (!visible)
            {
                return;
            }

            if (prompt.TextKey != _shownPromptKey || prompt.Argument != _shownPromptArgument
                || prompt.IsAvailable != _shownPromptAvailable)
            {
                _shownPromptKey = prompt.TextKey;
                _shownPromptArgument = prompt.Argument;
                _shownPromptAvailable = prompt.IsAvailable;
                promptLabel.text = _texts.Format(prompt.TextKey, prompt.Argument);
                promptLabel.color = prompt.IsAvailable ? _config.TextColor : _config.UnavailableTextColor;
            }

            if (_interaction.IsHolding)
            {
                progressFill.anchorMax = new Vector2(_interaction.Progress, 1f);
            }
        }

        private void UpdateFuel()
        {
            var visible = _fire.HasNearbyFire
                && Place(fuelRoot, _fire.NearbyFireX, _fire.NearbyFireY + _config.FuelDisplayHeight, _fire.NearbyFireZ);
            SetActive(fuelRoot, visible);
            if (!visible)
            {
                return;
            }

            while (_pips.Count < _fire.NearbyMaxFuel)
            {
                _pips.Add(Instantiate(fuelPipPrefab, fuelPipRow));
            }

            var filled = _fire.NearbySecondsPerFuel > 0f
                ? Mathf.CeilToInt(_fire.NearbyFuelSeconds / _fire.NearbySecondsPerFuel)
                : 0;
            for (var i = 0; i < _pips.Count; i++)
            {
                SetActive(_pips[i].rectTransform, i < _fire.NearbyMaxFuel);
                _pips[i].color = i < filled ? _config.FuelPipFilledColor : _config.FuelPipEmptyColor;
            }

            var seconds = Mathf.CeilToInt(_fire.NearbyFuelSeconds);
            if (seconds != _shownSeconds)
            {
                _shownSeconds = seconds;
                fuelSecondsLabel.text = _texts.Format(FuelSecondsKey, seconds);
            }
        }

        /// <summary>Moves a HUD element to a world point's screen position; false when the point is behind the camera.</summary>
        private bool Place(RectTransform element, float x, float y, float z)
        {
            var screen = gameCamera.WorldToScreenPoint(new Vector3(x, y, z));
            if (screen.z <= 0f)
            {
                return false;
            }

            element.position = new Vector3(screen.x, screen.y, 0f);
            return true;
        }

        private static void SetActive(RectTransform element, bool active)
        {
            if (element.gameObject.activeSelf != active)
            {
                element.gameObject.SetActive(active);
            }
        }
    }
}
