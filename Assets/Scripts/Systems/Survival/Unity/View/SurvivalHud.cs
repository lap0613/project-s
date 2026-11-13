using UnityEngine;
using VContainer;

namespace Whiteout.Survival.Unity
{
    /// <summary>
    /// Three gauge bars (체온 / 허기 / 체력). Each bar is a RectTransform stretched inside its background;
    /// its right anchor is set to the gauge ratio, so no sprite is needed.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign the three fill RectTransforms (anchors min (0,0), max (1,1), zero offsets).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;SurvivalHud&gt;().
    /// </code>
    /// </example>
    public sealed class SurvivalHud : MonoBehaviour
    {
        [SerializeField] private RectTransform temperatureFill;
        [SerializeField] private RectTransform hungerFill;
        [SerializeField] private RectTransform healthFill;

        private SurvivalSystem _survival;
        private SurvivalSettings _settings;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="survival">The hurdles to show.</param>
        /// <param name="settings">Supplies the gauge maximum.</param>
        [Inject]
        public void Construct(SurvivalSystem survival, SurvivalSettings settings)
        {
            _survival = survival;
            _settings = settings;
        }

        private void Update()
        {
            SetFill(temperatureFill, _survival.Temperature.Value);
            SetFill(hungerFill, _survival.Hunger.Value);
            SetFill(healthFill, _survival.Health.Value);
        }

        private void SetFill(RectTransform fill, float value)
        {
            fill.anchorMax = new Vector2(Mathf.Clamp01(value / _settings.MaxValue), 1f);
        }
    }
}
