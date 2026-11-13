using System;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer.Unity;
using Whiteout.Camp.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;
using Whiteout.Gathering.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// Outlines the target the mouse cursor is on (ui-plan section 8.1, M2): the target's meshes are drawn again a little
    /// larger with a front-face-culled unlit material, so only a rim shows around them. Bright color when the action can
    /// be done now, grey when it cannot. Draws every frame while there is a target; creates no objects.
    /// The target is found by the position in the director's prompt among the pickups, campfires and shelters.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;TargetHighlighter&gt;();
    /// </code>
    /// </example>
    public sealed class TargetHighlighter : ILateTickable
    {
        private const float PositionTolerance = 0.05f;

        private readonly InteractionDirector _director;
        private readonly PickupRegistry _pickups;
        private readonly CampRegistry _camp;
        private readonly PointerConfig _config;
        private readonly MaterialPropertyBlock _block = new MaterialPropertyBlock();
        private readonly int _colorId = Shader.PropertyToID("_BaseColor");

        /// <summary>Creates the highlighter.</summary>
        /// <param name="director">Tells which target is under the cursor.</param>
        /// <param name="pickups">Pickups on the map.</param>
        /// <param name="camp">Placed campfires and shelters.</param>
        /// <param name="config">Outline colors, scale and material.</param>
        public TargetHighlighter(InteractionDirector director, PickupRegistry pickups, CampRegistry camp, PointerConfig config)
        {
            _director = director ?? throw new ArgumentNullException(nameof(director));
            _pickups = pickups ?? throw new ArgumentNullException(nameof(pickups));
            _camp = camp ?? throw new ArgumentNullException(nameof(camp));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc />
        public void LateTick()
        {
            if (_config.OutlineMaterial == null || !_director.TryGetHighlight(out var prompt))
            {
                return;
            }

            var root = FindRoot(prompt.X, prompt.Z);
            if (root == null)
            {
                return;
            }

            _block.SetColor(_colorId, prompt.IsAvailable ? _config.HighlightAvailableColor : _config.HighlightUnavailableColor);
            var filters = root.GetComponentsInChildren<MeshFilter>();
            for (var i = 0; i < filters.Length; i++)
            {
                DrawOutline(filters[i]);
            }
        }

        private void DrawOutline(MeshFilter filter)
        {
            if (filter.sharedMesh == null || !filter.TryGetComponent<MeshRenderer>(out var meshRenderer) || !meshRenderer.enabled
                || !filter.gameObject.activeInHierarchy)
            {
                return;
            }

            var center = meshRenderer.bounds.center;
            var matrix = Matrix4x4.Translate(center)
                * Matrix4x4.Scale(Vector3.one * _config.OutlineScale)
                * Matrix4x4.Translate(-center)
                * filter.transform.localToWorldMatrix;
            var renderParams = new RenderParams(_config.OutlineMaterial)
            {
                matProps = _block,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                layer = filter.gameObject.layer,
            };
            for (var submesh = 0; submesh < filter.sharedMesh.subMeshCount; submesh++)
            {
                Graphics.RenderMesh(renderParams, filter.sharedMesh, submesh, matrix);
            }
        }

        private Transform FindRoot(float x, float z)
        {
            var pickups = _pickups.Pickups;
            for (var i = 0; i < pickups.Count; i++)
            {
                if (pickups[i] != null && Near(pickups[i].transform.position, x, z))
                {
                    return pickups[i].transform;
                }
            }

            var campfires = _camp.Campfires;
            for (var i = 0; i < campfires.Count; i++)
            {
                if (campfires[i] != null && Near(campfires[i].transform.position, x, z))
                {
                    return campfires[i].transform;
                }
            }

            var shelters = _camp.Shelters;
            for (var i = 0; i < shelters.Count; i++)
            {
                if (shelters[i] != null && Near(shelters[i].transform.position, x, z))
                {
                    return shelters[i].transform;
                }
            }

            return null;
        }

        private static bool Near(Vector3 position, float x, float z)
        {
            return Mathf.Abs(position.x - x) < PositionTolerance && Mathf.Abs(position.z - z) < PositionTolerance;
        }
    }
}
