using System.Collections;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// Loads the Game scene additively so its <see cref="LifetimeScope"/> becomes a child of the
    /// Bootstrap scope. Shared by <see cref="BootstrapEntry"/> and <see cref="GameRestarter"/>.
    /// Run the returned coroutines on a MonoBehaviour that is NOT inside the Game scene.
    /// </summary>
    /// <example>
    /// <code>
    /// var loader = new GameSceneLoader("Game", bootstrapScope);
    /// StartCoroutine(loader.Load());
    /// </code>
    /// </example>
    public sealed class GameSceneLoader
    {
        private readonly string _sceneName;
        private readonly LifetimeScope _parent;

        /// <summary>Creates a loader.</summary>
        /// <param name="sceneName">Scene to load additively (must be in Build Settings).</param>
        /// <param name="parent">Scope that becomes the parent of the loaded scene's scope.</param>
        public GameSceneLoader(string sceneName, LifetimeScope parent)
        {
            _sceneName = sceneName;
            _parent = parent;
        }

        /// <summary>Loads the scene additively with the parent scope, then makes it the active scene.</summary>
        public IEnumerator Load()
        {
            // The parent override must stay enqueued until the scene's scope has run Awake,
            // which happens before the async operation reports done.
            using (LifetimeScope.EnqueueParent(_parent))
            {
                var load = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
                while (!load.isDone)
                {
                    yield return null;
                }
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
        }

        /// <summary>Unloads the scene if it is loaded, then loads it again (same parent).</summary>
        public IEnumerator Reload()
        {
            var loaded = SceneManager.GetSceneByName(_sceneName);
            if (loaded.isLoaded)
            {
                var unload = SceneManager.UnloadSceneAsync(loaded);
                while (!unload.isDone)
                {
                    yield return null;
                }
            }

            yield return Load();
        }
    }
}
