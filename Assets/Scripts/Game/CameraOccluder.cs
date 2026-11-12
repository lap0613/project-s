using UnityEngine;

namespace Whiteout.Game
{
    /// <summary>
    /// Marks an obstacle (tree, rock) that may turn see-through when it stands between the camera and the player.
    /// Only objects with this component are faded; ground, map-edge walls and player-built structures are not
    /// (tech-decisions 17).
    /// </summary>
    public sealed class CameraOccluder : MonoBehaviour
    {
        [Tooltip("See-through material swapped in while this object hides the player.")]
        [SerializeField] private Material fadedMaterial;

        private Renderer[] renderers;
        private Material[][] originalMaterials;
        private bool isFaded;

        /// <summary>Whether the see-through material is currently applied.</summary>
        public bool IsFaded => isFaded;

        /// <summary>Switches between the see-through material and the original materials.</summary>
        /// <param name="faded">True to apply the see-through material, false to restore the originals.</param>
        public void SetFaded(bool faded)
        {
            if (faded == isFaded || fadedMaterial == null)
            {
                return;
            }

            CacheRenderers();
            for (var i = 0; i < renderers.Length; i++)
            {
                if (faded)
                {
                    var swapped = new Material[originalMaterials[i].Length];
                    for (var m = 0; m < swapped.Length; m++)
                    {
                        swapped[m] = fadedMaterial;
                    }

                    renderers[i].sharedMaterials = swapped;
                }
                else
                {
                    renderers[i].sharedMaterials = originalMaterials[i];
                }
            }

            isFaded = faded;
        }

        private void CacheRenderers()
        {
            if (renderers != null)
            {
                return;
            }

            renderers = GetComponentsInChildren<Renderer>();
            originalMaterials = new Material[renderers.Length][];
            for (var i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].sharedMaterials;
            }
        }

        private void OnDisable()
        {
            SetFaded(false);
        }
    }
}
