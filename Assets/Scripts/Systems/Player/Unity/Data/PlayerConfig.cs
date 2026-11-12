using UnityEngine;

namespace Whiteout.Player.Unity
{
    /// <summary>Tunable player values (data asset). Must not reference View code.</summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Player Config
    /// float speed = config.MoveSpeed;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Player Config", fileName = "PlayerConfig")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Tooltip("Base move speed in meters per second.")]
        [SerializeField] private float moveSpeed = 4f;

        [Tooltip("Constant downward speed in meters per second that keeps the CharacterController grounded.")]
        [SerializeField] private float downwardSpeed = 2f;

        [Tooltip("How fast the visual model turns toward the move direction, in degrees per second.")]
        [SerializeField] private float modelTurnSpeed = 720f;

        [Tooltip("Extra free space around the player's capsule required at the start point, in meters (provisional).")]
        [SerializeField] private float startClearance = 0.5f;

        [Tooltip("How many random start points to try before falling back to the map center (provisional).")]
        [SerializeField] private int startPickAttempts = 30;

        /// <summary>Base move speed (m/s).</summary>
        public float MoveSpeed => moveSpeed;

        /// <summary>Constant downward speed (m/s) applied every frame.</summary>
        public float DownwardSpeed => downwardSpeed;

        /// <summary>Model turn speed (degrees per second).</summary>
        public float ModelTurnSpeed => modelTurnSpeed;

        /// <summary>Extra free space (m) around the player's capsule required at the start point.</summary>
        public float StartClearance => startClearance;

        /// <summary>Random start points tried before falling back to the map center.</summary>
        public int StartPickAttempts => startPickAttempts;
    }
}
