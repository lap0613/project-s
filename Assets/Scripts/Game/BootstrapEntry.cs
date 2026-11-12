using System.Collections;
using UnityEngine;

namespace Whiteout.Game
{
    /// <summary>
    /// Bootstrap scene entry point: on Start, loads the Game scene additively as a child of the
    /// Bootstrap scope.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a GameObject in the Bootstrap scene; assign the BootstrapLifetimeScope.
    /// </code>
    /// </example>
    public sealed class BootstrapEntry : MonoBehaviour
    {
        [SerializeField] private BootstrapLifetimeScope bootstrapScope;
        [SerializeField] private string gameSceneName = "Game";

        private IEnumerator Start()
        {
            var loader = new GameSceneLoader(gameSceneName, bootstrapScope);
            yield return loader.Load();
        }
    }
}
