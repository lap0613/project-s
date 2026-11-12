using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Randomness;
using Whiteout.Core.Stats;

namespace Whiteout.Player.Unity
{
    /// <summary>
    /// Moves the player with a <see cref="CharacterController"/>. This root object is the logic object;
    /// the visual model is a child (<see cref="model"/>) that only rotates for display.
    /// Dependencies arrive through VContainer method injection.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on the player root with a CharacterController; assign the child model.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;PlayerController&gt;().
    /// </code>
    /// </example>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Tooltip("Child object holding the visual model. Rotated toward the move direction.")]
        [SerializeField] private Transform model;

        private CharacterController _controller;
        private IPlayerInput _input;
        private PlayerConfig _config;
        private IRandom _random;
        private IMapLayout _layout;
        private PlayerMoveLogic _moveLogic;
        private Stat _moveSpeed;

        /// <summary>Move speed stat, so modifiers (augments, gear) can attach to it.</summary>
        public Stat MoveSpeedStat => _moveSpeed;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="input">Move input.</param>
        /// <param name="config">Player values.</param>
        /// <param name="random">Random source for the start point.</param>
        /// <param name="layout">Map description.</param>
        [Inject]
        public void Construct(IPlayerInput input, PlayerConfig config, IRandom random, IMapLayout layout)
        {
            _input = input;
            _config = config;
            _random = random;
            _layout = layout;
            _moveLogic = new PlayerMoveLogic();
            _moveSpeed = new Stat(PlayerStats.MoveSpeed, config.MoveSpeed);
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            // CharacterController overwrites teleports made while it is enabled; disabling it also keeps
            // the player's own collider out of the free-spot checks below.
            _controller.enabled = false;

            var y = transform.position.y;
            var start = new PlayerStartPicker(_random, _layout)
                .PickFree(point => IsFreeSpot(new Vector3(point.X, y, point.Z)), _config.StartPickAttempts);

            transform.position = new Vector3(start.X, y, start.Z);
            _controller.enabled = true;
        }

        /// <summary>
        /// True when no non-trigger collider overlaps the player's capsule (plus <see cref="PlayerConfig.StartClearance"/>)
        /// at <paramref name="position"/>. The capsule is lifted by the clearance plus the controller's skin width,
        /// so its lowest point stays above the feet and the ground the player stands on does not count.
        /// </summary>
        private bool IsFreeSpot(Vector3 position)
        {
            var clearance = _config.StartClearance;
            var radius = _controller.radius + clearance;
            var center = position + _controller.center;
            var halfSegment = Mathf.Max(0f, _controller.height * 0.5f - _controller.radius);
            var lift = Vector3.up * (clearance + _controller.skinWidth);
            var bottom = center + Vector3.down * halfSegment + lift;
            var top = center + Vector3.up * halfSegment + lift;
            return !Physics.CheckCapsule(bottom, top, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var velocity = _moveLogic.ComputeVelocity(_input.MoveX, _input.MoveZ, _moveSpeed.Value);

            var motion = new Vector3(velocity.X, -_config.DownwardSpeed, velocity.Z) * dt;
            _controller.Move(motion);

            RotateModel(velocity, dt);
        }

        private void RotateModel(PlanarVector velocity, float dt)
        {
            if (model == null || (velocity.X == 0f && velocity.Z == 0f))
            {
                return;
            }

            var target = Quaternion.LookRotation(new Vector3(velocity.X, 0f, velocity.Z), Vector3.up);
            model.rotation = Quaternion.RotateTowards(model.rotation, target, _config.ModelTurnSpeed * dt);
        }
    }
}
