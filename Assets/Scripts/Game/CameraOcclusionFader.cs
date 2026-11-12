using System.Collections.Generic;
using UnityEngine;

namespace Whiteout.Game
{
    /// <summary>
    /// Put on the main camera. Every frame it sphere-casts from the camera to the target (the player) and fades the
    /// <see cref="CameraOccluder"/> objects in between; objects that no longer block the view are restored
    /// (tech-decisions 17). Values are provisional and tuned in the Inspector.
    /// </summary>
    public sealed class CameraOcclusionFader : MonoBehaviour
    {
        [Tooltip("Object the camera must keep visible (the player).")]
        [SerializeField] private Transform target;

        [Tooltip("Radius of the sphere cast from the camera to the target, in metres (provisional).")]
        [SerializeField] private float castRadius = 0.5f;

        [Tooltip("Layers the cast can hit. Only hits that carry a CameraOccluder are faded.")]
        [SerializeField] private LayerMask castLayers = ~0;

        [Tooltip("Maximum number of hits read per frame.")]
        [SerializeField] private int maxHits = 16;

        private readonly HashSet<CameraOccluder> faded = new HashSet<CameraOccluder>();
        private readonly HashSet<CameraOccluder> blockingThisFrame = new HashSet<CameraOccluder>();
        private readonly List<CameraOccluder> toRestore = new List<CameraOccluder>();
        private RaycastHit[] hits;

        /// <summary>Sets the object the camera must keep visible.</summary>
        /// <param name="newTarget">The player transform, or null to stop fading.</param>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void Awake()
        {
            hits = new RaycastHit[Mathf.Max(1, maxHits)];
        }

        private void LateUpdate()
        {
            blockingThisFrame.Clear();

            if (target != null)
            {
                var origin = transform.position;
                var toTarget = target.position - origin;
                var distance = toTarget.magnitude;
                if (distance > castRadius)
                {
                    var count = Physics.SphereCastNonAlloc(
                        origin, castRadius, toTarget / distance, hits, distance - castRadius,
                        castLayers, QueryTriggerInteraction.Ignore);

                    for (var i = 0; i < count; i++)
                    {
                        var occluder = hits[i].collider.GetComponentInParent<CameraOccluder>();
                        if (occluder != null)
                        {
                            blockingThisFrame.Add(occluder);
                        }
                    }
                }
            }

            foreach (var occluder in blockingThisFrame)
            {
                if (faded.Add(occluder))
                {
                    occluder.SetFaded(true);
                }
            }

            toRestore.Clear();
            foreach (var occluder in faded)
            {
                if (!blockingThisFrame.Contains(occluder))
                {
                    toRestore.Add(occluder);
                }
            }

            foreach (var occluder in toRestore)
            {
                faded.Remove(occluder);
                if (occluder != null)
                {
                    occluder.SetFaded(false);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var occluder in faded)
            {
                if (occluder != null)
                {
                    occluder.SetFaded(false);
                }
            }

            faded.Clear();
        }
    }
}
