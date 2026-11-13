using UnityEditor;
using UnityEditor.SceneManagement;

namespace Whiteout.Editor
{
    /// <summary>
    /// Editor-only: whatever scene is open, Play starts from the Bootstrap scene (build index 0), so the
    /// Game scene always gets its parent scope (tech-decisions 21). Toggle with
    /// <c>Whiteout &gt; Play From Bootstrap</c>; the choice is stored per machine in EditorPrefs (default on).
    /// </summary>
    [InitializeOnLoad]
    public static class PlayFromBootstrap
    {
        private const string MenuPath = "Whiteout/Play From Bootstrap";
        private const string PrefKey = "Whiteout.PlayFromBootstrap";

        static PlayFromBootstrap()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Apply();
        }

        /// <summary>Whether Play starts from the Bootstrap scene.</summary>
        public static bool Enabled
        {
            get => EditorPrefs.GetBool(PrefKey, true);
            set
            {
                EditorPrefs.SetBool(PrefKey, value);
                Apply();
            }
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            Enabled = !Enabled;
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            // Re-read just before entering play so Build Settings changes are picked up.
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                Apply();
            }
        }

        private static void Apply()
        {
            EditorSceneManager.playModeStartScene = Enabled ? FindBootstrapScene() : null;
        }

        private static SceneAsset FindBootstrapScene()
        {
            var scenes = EditorBuildSettings.scenes;
            return scenes.Length > 0 ? AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[0].path) : null;
        }
    }
}
