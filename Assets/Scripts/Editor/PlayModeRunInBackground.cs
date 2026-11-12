using UnityEditor;
using UnityEngine;

namespace Whiteout.Editor
{
    /// <summary>
    /// Editor-only: keeps play mode running while the Unity window has no focus, so play checks
    /// (including automated ones over MCP) do not stall on the first frame. Sets
    /// <see cref="Application.runInBackground"/> when play mode starts. Not included in builds,
    /// so the built game's focus behaviour is unchanged (tech-decisions 18).
    /// </summary>
    [InitializeOnLoad]
    public static class PlayModeRunInBackground
    {
        static PlayModeRunInBackground()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                Application.runInBackground = true;
            }
        }
    }
}
