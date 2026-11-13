using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Environment.Unity
{
    /// <summary>
    /// Feeds the player's ring to <see cref="EnvironmentSystem"/> every frame and applies sight limits.
    /// Night: sun and ambient light are dimmed and a light on the player lights only
    /// <see cref="EnvironmentConfig.NightVisionRadius"/> around them (fire lights add their own light).
    /// Blizzard (any weather with fog): linear RenderSettings fog, clear up to
    /// <see cref="EnvironmentConfig.BlizzardFogDistance"/> from the player.
    /// When both apply, the narrower one wins: light radius 12 m is inside the 15 m fog distance.
    /// The look is a placeholder; all of its numbers live in <see cref="EnvironmentConfig"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a Game scene object; assign Player, Game Camera, Sun Light, Player Light.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;EnvironmentView&gt;().
    /// </code>
    /// </example>
    public sealed class EnvironmentView : MonoBehaviour
    {
        [Tooltip("The player whose position decides the ring and the lit area.")]
        [SerializeField] private Transform player;

        [Tooltip("Camera used to convert 'meters from the player' into fog distance.")]
        [SerializeField] private Camera gameCamera;

        [Tooltip("The scene's main directional light. Dimmed at night.")]
        [SerializeField] private Light sunLight;

        [Tooltip("A point light that follows the player. Enabled at night, range set to the night vision radius. Its intensity here is the full night value.")]
        [SerializeField] private Light playerLight;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private IMapLayout _layout;
        private EnvironmentSystem _environment;
        private EnvironmentConfig _config;
        private bool _targetNight;
        private bool _targetFog;
        private float _nightBlend;
        private float _fogBlend;
        private bool _fogActive;
        private bool _captured;

        private float _daySunIntensity;
        private float _dayAmbientIntensity;
        private float _playerLightIntensity;
        private bool _originalFog;
        private FogMode _originalFogMode;
        private Color _originalFogColor;
        private float _originalFogStart;
        private float _originalFogEnd;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="layout">Map description (ring from distance).</param>
        /// <param name="bus">Event channel.</param>
        /// <param name="environment">Environment rules receiving the player's ring.</param>
        /// <param name="config">Environment values.</param>
        [Inject]
        public void Construct(IMapLayout layout, IEventBus bus, EnvironmentSystem environment, EnvironmentConfig config)
        {
            _layout = layout;
            _environment = environment;
            _config = config;
            _subscriptions.Add(bus.Subscribe<NightStarted>(_ => _targetNight = true));
            _subscriptions.Add(bus.Subscribe<DayStarted>(_ => _targetNight = false));
            _subscriptions.Add(bus.Subscribe<WeatherChanged>(e => _targetFog = e.Weather.HasFog));
        }

        private void Start()
        {
            if (player == null || gameCamera == null || sunLight == null || playerLight == null)
            {
                throw new InvalidOperationException("EnvironmentView: Player, Game Camera, Sun Light and Player Light must be assigned.");
            }

            _daySunIntensity = sunLight.intensity;
            _dayAmbientIntensity = RenderSettings.ambientIntensity;
            _playerLightIntensity = playerLight.intensity;
            _originalFog = RenderSettings.fog;
            _originalFogMode = RenderSettings.fogMode;
            _originalFogColor = RenderSettings.fogColor;
            _originalFogStart = RenderSettings.fogStartDistance;
            _originalFogEnd = RenderSettings.fogEndDistance;
            playerLight.range = _config.NightVisionRadius;
            playerLight.enabled = false;
            _captured = true;
        }

        private void Update()
        {
            var position = player.position;
            var distance = _layout.DistanceFromCenter(position.x, position.z);
            _environment.SetPlayerRing(_layout.GetRing(distance));

            var fade = _config.VisionFadeSeconds;
            var step = fade > 0f ? Time.deltaTime / fade : 1f;
            _nightBlend = Mathf.MoveTowards(_nightBlend, _targetNight ? 1f : 0f, step);
            _fogBlend = Mathf.MoveTowards(_fogBlend, _targetFog ? 1f : 0f, step);
        }

        private void LateUpdate()
        {
            ApplyNight();
            ApplyFog();
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
            {
                _subscriptions[i].Dispose();
            }

            _subscriptions.Clear();
            if (_captured)
            {
                sunLight.intensity = _daySunIntensity;
                RenderSettings.ambientIntensity = _dayAmbientIntensity;
                RestoreFog();
            }
        }

        private void ApplyNight()
        {
            sunLight.intensity = Mathf.Lerp(_daySunIntensity, _daySunIntensity * _config.NightSunIntensityScale, _nightBlend);
            RenderSettings.ambientIntensity = Mathf.Lerp(
                _dayAmbientIntensity, _dayAmbientIntensity * _config.NightAmbientIntensityScale, _nightBlend);
            playerLight.enabled = _nightBlend > 0f;
            playerLight.intensity = _playerLightIntensity * _nightBlend;
        }

        private void ApplyFog()
        {
            if (_fogBlend <= 0f)
            {
                if (_fogActive)
                {
                    RestoreFog();
                }

                return;
            }

            if (!_fogActive)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = _config.BlizzardFogColor;
                _fogActive = true;
            }

            // Fog distance is measured from the camera, so add the camera-to-player distance.
            var cameraToPlayer = Vector3.Distance(gameCamera.transform.position, player.position);
            var fullStart = cameraToPlayer + _config.BlizzardFogDistance;
            var fullEnd = fullStart + _config.BlizzardFogFalloff;
            var clearStart = gameCamera.farClipPlane;
            var clearEnd = clearStart + _config.BlizzardFogFalloff;
            RenderSettings.fogStartDistance = Mathf.Lerp(clearStart, fullStart, _fogBlend);
            RenderSettings.fogEndDistance = Mathf.Lerp(clearEnd, fullEnd, _fogBlend);
        }

        private void RestoreFog()
        {
            RenderSettings.fog = _originalFog;
            RenderSettings.fogMode = _originalFogMode;
            RenderSettings.fogColor = _originalFogColor;
            RenderSettings.fogStartDistance = _originalFogStart;
            RenderSettings.fogEndDistance = _originalFogEnd;
            _fogActive = false;
        }
    }
}
