using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// Draws the map outline and ring boundaries as circles: Gizmos in the Scene view and
    /// LineRenderers in the Game view. Toggle with <see cref="showRings"/>.
    /// In play mode the layout is injected (the object is listed in the Game scope's Auto Inject Game Objects);
    /// in edit mode (Scene view) it falls back to <see cref="editorLayout"/>.
    /// The Game view lines use the serialized <see cref="lineMaterial"/> asset; no material is created at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to any object in the Game scene; tick "Show Rings"; assign Line Material (a URP Particles/Unlit material).
    /// // Assign Editor Layout to see Gizmos outside play mode.
    /// </code>
    /// </example>
    public sealed class MapRingDebugView : MonoBehaviour
    {
        [SerializeField] private bool showRings = true;
        [Tooltip("Used when no layout was injected (edit mode Scene view).")]
        [SerializeField] private MapLayoutDefinition editorLayout;
        [Tooltip("Material for the Game view lines (vertex-coloured, e.g. URP Particles/Unlit). If empty, only Scene view Gizmos are drawn.")]
        [SerializeField] private Material lineMaterial;
        [SerializeField] private int segments = 128;
        [SerializeField] private float lineWidth = 0.6f;
        [SerializeField] private float lineHeight = 0.1f;
        [SerializeField] private Color innerColor = Color.green;
        [SerializeField] private Color middleColor = Color.yellow;
        [SerializeField] private Color outerColor = Color.red;

        private IMapLayout _injectedLayout;
        private GameObject _linesRoot;
        private bool _linesVisible;

        /// <summary>Receives the layout from the container.</summary>
        /// <param name="layout">Map description.</param>
        [Inject]
        public void Construct(IMapLayout layout)
        {
            _injectedLayout = layout;
        }

        private IMapLayout Layout => _injectedLayout ?? editorLayout;

        private void Update()
        {
            // Development aid only: drawn in the Editor and Development Builds, never in release builds (tech-decisions 20).
            if (!Debug.isDebugBuild)
            {
                return;
            }

            if (showRings == _linesVisible)
            {
                return;
            }

            if (showRings)
            {
                BuildLines();
            }
            else
            {
                DestroyLines();
            }
        }

        private void OnDisable()
        {
            DestroyLines();
        }

        private void OnDrawGizmos()
        {
            var layout = Layout;
            if (!showRings || layout == null)
            {
                return;
            }

            DrawGizmoCircle(layout, layout.InnerRingRadius, innerColor);
            DrawGizmoCircle(layout, layout.MiddleRingRadius, middleColor);
            DrawGizmoCircle(layout, layout.OuterRadius, outerColor);
        }

        private void DrawGizmoCircle(IMapLayout layout, float radius, Color color)
        {
            Gizmos.color = color;
            var previous = PointOnCircle(layout, radius, 0);
            for (var i = 1; i <= segments; i++)
            {
                var next = PointOnCircle(layout, radius, i);
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }

        private void BuildLines()
        {
            var layout = Layout;
            if (layout == null || lineMaterial == null || !Application.isPlaying)
            {
                return;
            }

            _linesRoot = new GameObject("MapRingLines");
            _linesRoot.transform.SetParent(transform, false);
            CreateLine(layout, layout.InnerRingRadius, innerColor);
            CreateLine(layout, layout.MiddleRingRadius, middleColor);
            CreateLine(layout, layout.OuterRadius, outerColor);
            _linesVisible = true;
        }

        private void CreateLine(IMapLayout layout, float radius, Color color)
        {
            var go = new GameObject("Ring " + radius);
            go.transform.SetParent(_linesRoot.transform, false);

            var line = go.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.loop = true;
            line.positionCount = segments;
            line.widthMultiplier = lineWidth;
            line.startColor = color;
            line.endColor = color;
            line.sharedMaterial = lineMaterial;
            for (var i = 0; i < segments; i++)
            {
                line.SetPosition(i, PointOnCircle(layout, radius, i));
            }
        }

        private Vector3 PointOnCircle(IMapLayout layout, float radius, int index)
        {
            var angle = index * Mathf.PI * 2f / segments;
            return new Vector3(
                layout.CenterX + radius * Mathf.Cos(angle),
                lineHeight,
                layout.CenterZ + radius * Mathf.Sin(angle));
        }

        private void DestroyLines()
        {
            if (_linesRoot != null)
            {
                Destroy(_linesRoot);
            }

            _linesRoot = null;
            _linesVisible = false;
        }
    }
}
