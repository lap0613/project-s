using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Whiteout.Core.Contracts;

namespace Whiteout.Game
{
    /// <summary>
    /// <see cref="IPointerWorld"/> from the main camera and the mouse position that <see cref="IInteractionInput"/> reports.
    /// The cursor ray is intersected with horizontal planes, so no collider is needed on the targets.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;PointerWorld&gt;(Lifetime.Singleton).As&lt;IPointerWorld&gt;();
    /// </code>
    /// </example>
    public sealed class PointerWorld : IPointerWorld
    {
        private readonly IInteractionInput _input;
        private Camera _camera;

        /// <summary>Creates the pointer.</summary>
        /// <param name="input">Cursor position.</param>
        public PointerWorld(IInteractionInput input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <inheritdoc />
        public bool IsOverUi => _input.HasPointer && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        /// <inheritdoc />
        public bool IsOverWorld => _input.HasPointer && !IsOverUi;

        /// <inheritdoc />
        public bool TryGetPointOnPlane(float planeY, out float x, out float z)
        {
            x = 0f;
            z = 0f;
            if (!_input.HasPointer)
            {
                return false;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    return false;
                }
            }

            var ray = _camera.ScreenPointToRay(new Vector3(_input.PointerX, _input.PointerY, 0f));
            if (Mathf.Abs(ray.direction.y) < 1e-4f)
            {
                return false;
            }

            var t = (planeY - ray.origin.y) / ray.direction.y;
            if (t < 0f)
            {
                return false;
            }

            x = ray.origin.x + ray.direction.x * t;
            z = ray.origin.z + ray.direction.z * t;
            return true;
        }
    }
}
