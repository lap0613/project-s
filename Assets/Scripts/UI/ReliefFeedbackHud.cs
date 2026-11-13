using System;
using System.Collections.Generic;
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
    /// The relief feedback (design/ux/ui-plan.md 7.6 N3, N4, N5): an orange edge glow while the player stands in
    /// the warmth radius of a lit campfire (with an enter sound and a fire sound that grows with closeness), and
    /// a morning effect when a new day begins from day 2 on (sun icon with the day text, a golden edge glow and a
    /// chime). Effects stay on the screen edge; the centre is clear. A morning that coincides with the player's
    /// death is skipped. Sound is played with plain AudioSources (tech-decisions 33). Reads fire state through
    /// <see cref="IFireStatus"/> and the day through <see cref="DayStarted"/>; it never touches Camp or RunFlow.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a full-canvas RectTransform (together with GoalGuidanceHud).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;ReliefFeedbackHud&gt;().
    /// </code>
    /// </example>
    public sealed class ReliefFeedbackHud : MonoBehaviour
    {
        private const string MorningKey = "hud.morning.day";
        private const int FirstMorningDay = 2;
        private const int VignetteTextureSize = 128;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly Dictionary<long, float> _lastEnterSound = new Dictionary<long, float>();
        private IFireStatus _fire;
        private IPlayerPosition _player;
        private UiTextTable _texts;
        private HudConfig _config;
        private Sprite _vignetteSprite;
        private Sprite _sunSprite;
        private Image _warmthImage;
        private Image _glowImage;
        private CanvasGroup _bannerGroup;
        private TMP_Text _bannerLabel;
        private AudioSource _enterSource;
        private AudioSource _loopSource;
        private AudioSource _morningSource;
        private float _warmth;
        private bool _wasInRange;
        private bool _dead;
        private int _pendingMorningDay;
        private float _bannerAge = -1f;
        private float _glowAge = -1f;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="fire">Campfire status near the player.</param>
        /// <param name="player">Player position, for the fire sound distance.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Times, opacities, colors and sounds.</param>
        [Inject]
        public void Construct(IEventBus bus, IFireStatus fire, IPlayerPosition player, UiTextTable texts, HudConfig config)
        {
            _fire = fire;
            _player = player;
            _texts = texts;
            _config = config;
            _subscriptions.Add(bus.Subscribe<DayStarted>(e =>
            {
                if (e.Day >= FirstMorningDay)
                {
                    _pendingMorningDay = e.Day;
                }
            }));
            _subscriptions.Add(bus.Subscribe<PlayerDied>(_ =>
            {
                _dead = true;
                _pendingMorningDay = 0;
            }));
        }

        private void Start()
        {
            // Behind the other HUD parts: a screen effect must never cover a number.
            transform.SetAsFirstSibling();
            _vignetteSprite = CreateVignetteSprite();
            _sunSprite = WeatherIconSprites.Create(WeatherIconShape.Sun);

            _warmthImage = NewFullScreen("WarmthVignette", _config.WarmthVignetteColor);
            _glowImage = NewFullScreen("MorningGlow", _config.MorningGlowColor);
            BuildBanner();

            _enterSource = NewSource("EnterSound", false);
            _loopSource = NewSource("FireLoop", true);
            _morningSource = NewSource("MorningSound", false);
            _loopSource.clip = _config.FireLoopClip;
            _loopSource.volume = 0f;
        }

        private void Update()
        {
            UpdateWarmth();
            UpdateFireLoop();
            UpdateMorning();
        }

        private void UpdateWarmth()
        {
            var inRange = _fire.IsInLitFireRange;
            if (inRange && !_wasInRange)
            {
                PlayEnterSound();
            }

            _wasInRange = inRange;
            var seconds = inRange ? _config.WarmthFadeInSeconds : _config.WarmthFadeOutSeconds;
            _warmth = Mathf.MoveTowards(_warmth, inRange ? 1f : 0f, Time.deltaTime / seconds);
            SetAlpha(_warmthImage, _warmth * _config.WarmthVignetteMaxOpacity);
        }

        private void PlayEnterSound()
        {
            // The fire status has no fire id; the nearest fire's position tells fires apart.
            var key = ((long)Mathf.RoundToInt(_fire.NearbyFireX * 10f) << 32) ^ (uint)Mathf.RoundToInt(_fire.NearbyFireZ * 10f);
            if (_lastEnterSound.TryGetValue(key, out var last) && Time.time - last < _config.WarmthEnterCooldownSeconds)
            {
                return;
            }

            _lastEnterSound[key] = Time.time;
            if (_config.WarmthEnterClip != null)
            {
                _enterSource.PlayOneShot(_config.WarmthEnterClip, _config.WarmthEnterVolume);
            }
        }

        private void UpdateFireLoop()
        {
            var volume = 0f;
            if (_loopSource.clip != null && _fire.HasNearbyFire && _fire.NearbyFireLit)
            {
                var dx = _fire.NearbyFireX - _player.X;
                var dz = _fire.NearbyFireZ - _player.Z;
                var distance = Mathf.Sqrt(dx * dx + dz * dz);
                volume = _config.FireLoopMaxVolume * Mathf.Clamp01(1f - distance / _config.FireLoopHearDistance);
            }

            _loopSource.volume = volume;
            if (volume > 0f && !_loopSource.isPlaying)
            {
                _loopSource.Play();
            }
            else if (volume <= 0f && _loopSource.isPlaying)
            {
                _loopSource.Pause();
            }
        }

        private void UpdateMorning()
        {
            // One frame late on purpose: a death in the same frame as the new day cancels the effect.
            if (_pendingMorningDay > 0 && !_dead)
            {
                StartMorning(_pendingMorningDay);
            }

            _pendingMorningDay = 0;

            if (_glowAge >= 0f)
            {
                _glowAge += Time.deltaTime;
                var u = _glowAge / _config.MorningGlowSeconds;
                SetAlpha(_glowImage, u >= 1f ? 0f : Mathf.Sin(u * Mathf.PI) * _config.MorningGlowMaxOpacity);
                if (u >= 1f)
                {
                    _glowAge = -1f;
                }
            }

            if (_bannerAge >= 0f)
            {
                _bannerAge += Time.deltaTime;
                var total = _config.MorningBannerSeconds;
                var fade = Mathf.Max(_config.MorningBannerFadeSeconds, 0.0001f);
                _bannerGroup.alpha = Mathf.Clamp01(Mathf.Min(_bannerAge / fade, (total - _bannerAge) / fade));
                if (_bannerAge >= total)
                {
                    _bannerAge = -1f;
                    _bannerGroup.alpha = 0f;
                }
            }
        }

        private void StartMorning(int day)
        {
            _bannerLabel.text = _texts.Format(MorningKey, day);
            _bannerAge = 0f;
            _glowAge = 0f;
            if (_config.MorningClip != null)
            {
                _morningSource.PlayOneShot(_config.MorningClip, _config.MorningVolume);
            }
        }

        private void BuildBanner()
        {
            var iconSize = _config.MorningBannerIconSize;
            var fontSize = _config.MorningBannerFontSize;
            var textWidth = fontSize * 4f;
            var gap = iconSize * 0.2f;
            var width = iconSize + gap + textWidth;

            var root = HudBuilder.NewRect("MorningBanner", transform);
            root.anchorMin = new Vector2(0.5f, _config.MorningBannerTopFraction);
            root.anchorMax = new Vector2(0.5f, _config.MorningBannerTopFraction);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = Vector2.zero;
            root.sizeDelta = new Vector2(width, Mathf.Max(iconSize, fontSize * 1.3f));
            _bannerGroup = root.gameObject.AddComponent<CanvasGroup>();
            _bannerGroup.alpha = 0f;
            _bannerGroup.blocksRaycasts = false;
            _bannerGroup.interactable = false;

            var sun = HudBuilder.NewImage("Sun", root, _sunSprite, _config.MorningSunColor).rectTransform;
            sun.anchorMin = new Vector2(0f, 0.5f);
            sun.anchorMax = new Vector2(0f, 0.5f);
            sun.pivot = new Vector2(0f, 0.5f);
            sun.anchoredPosition = Vector2.zero;
            sun.sizeDelta = new Vector2(iconSize, iconSize);

            _bannerLabel = HudBuilder.NewLabel("Day", root, _texts, fontSize, _config.MorningBannerColor, TextAlignmentOptions.MidlineLeft);
            var labelRect = _bannerLabel.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0f, 1f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(iconSize + gap, 0f);
            labelRect.sizeDelta = new Vector2(textWidth, 0f);
        }

        private Image NewFullScreen(string name, Color color)
        {
            var image = HudBuilder.NewImage(name, transform, _vignetteSprite, new Color(color.r, color.g, color.b, 0f));
            HudBuilder.Stretch(image.rectTransform);
            return image;
        }

        private AudioSource NewSource(string name, bool loop)
        {
            var source = new GameObject(name).AddComponent<AudioSource>();
            source.transform.SetParent(transform, false);
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }

        private static void SetAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        private Sprite CreateVignetteSprite()
        {
            var texture = new Texture2D(VignetteTextureSize, VignetteTextureSize, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[VignetteTextureSize * VignetteTextureSize];
            var inner = _config.VignetteInnerRadius;
            var outer = _config.VignetteOuterRadius;
            for (var y = 0; y < VignetteTextureSize; y++)
            {
                for (var x = 0; x < VignetteTextureSize; x++)
                {
                    var u = (x + 0.5f) / VignetteTextureSize * 2f - 1f;
                    var v = (y + 0.5f) / VignetteTextureSize * 2f - 1f;
                    var t = Mathf.Clamp01((Mathf.Sqrt(u * u + v * v) - inner) / (outer - inner));
                    pixels[y * VignetteTextureSize + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(t * t * (3f - 2f * t) * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, VignetteTextureSize, VignetteTextureSize), new Vector2(0.5f, 0.5f), VignetteTextureSize, 0, SpriteMeshType.FullRect);
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Dispose();
            }

            HudBuilder.DestroySprite(_vignetteSprite);
            HudBuilder.DestroySprite(_sunSprite);
        }
    }
}
