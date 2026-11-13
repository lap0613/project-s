using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Game
{
    /// <summary>
    /// Restarts a run by reloading ONLY the Game scene. The Bootstrap scene and its services stay.
    /// Place it in the Bootstrap scene: a coroutine started from the Game scene would die when that scene unloads.
    /// The Bootstrap scope registers it as <see cref="IRunRestarter"/>, so the Game scene (the results screen) can
    /// call it without knowing this class (tech-decisions 34, R1).
    /// </summary>
    /// <example>
    /// <code>
    /// restarter.Restart();
    /// </code>
    /// </example>
    public sealed class GameRestarter : MonoBehaviour, IRunRestarter
    {
        [SerializeField] private BootstrapLifetimeScope bootstrapScope;
        [SerializeField] private string gameSceneName = "Game";

        private bool _restarting;

        /// <summary>Unloads and reloads the Game scene. Ignored while a restart is already running.</summary>
        [ContextMenu("Restart")]
        public void Restart()
        {
            if (_restarting)
            {
                return;
            }

            StartCoroutine(RestartRoutine());
        }

        private System.Collections.IEnumerator RestartRoutine()
        {
            _restarting = true;
            try
            {
                var loader = new GameSceneLoader(gameSceneName, bootstrapScope);
                yield return loader.Reload();
            }
            finally
            {
                _restarting = false;
            }
        }
    }
}
