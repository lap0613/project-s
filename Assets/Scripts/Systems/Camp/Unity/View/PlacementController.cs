using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// The placement mode (ui-plan section 9, core-rules section 4). B opens the structure list built from the
    /// <see cref="StructureCatalog"/>; number keys or the wheel select; a preview stands in front of the player,
    /// blue when it can be placed and red when it overlaps an obstacle; confirm places; cancel or B closes.
    /// Time keeps running and the player can move. A new structure definition just makes the list longer.
    /// Registered as a VContainer entry point; also the <see cref="IPlacementMenu"/> the HUD reads.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;PlacementController&gt;().As&lt;IPlacementMenu&gt;();
    /// </code>
    /// </example>
    public sealed class PlacementController : ITickable, IPlacementMenu, IDisposable
    {
        private readonly IObjectResolver _resolver;
        private readonly CampConfig _config;
        private readonly CampSystem _camp;
        private readonly IInteractionInput _input;
        private readonly IPlayerPosition _player;
        private readonly IBag _bag;
        private readonly IItemNames _names;
        private readonly UiTextTable _texts;
        private readonly IEventBus _bus;
        private readonly IDisposable _diedSubscription;
        private readonly StructureDefinition[] _definitions;
        private readonly StructureSpec[] _specs;
        private Renderer[] _previewRenderers = new Renderer[0];
        private GameObject _preview;
        private int _previewIndex = -1;
        private bool _previewBlocked;
        private bool _playerDead;

        /// <summary>Creates the controller.</summary>
        /// <param name="resolver">Creates placed structures with their dependencies injected.</param>
        /// <param name="config">Catalog and placement values.</param>
        /// <param name="camp">Pays for the structure.</param>
        /// <param name="input">Button input.</param>
        /// <param name="player">Player position and facing.</param>
        /// <param name="bag">The bag (for have/need amounts in notices).</param>
        /// <param name="names">Item names.</param>
        /// <param name="texts">UI text table (cost formatting).</param>
        /// <param name="bus">Event channel.</param>
        public PlacementController(
            IObjectResolver resolver,
            CampConfig config,
            CampSystem camp,
            IInteractionInput input,
            IPlayerPosition player,
            IBag bag,
            IItemNames names,
            UiTextTable texts,
            IEventBus bus)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _camp = camp ?? throw new ArgumentNullException(nameof(camp));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _names = names ?? throw new ArgumentNullException(nameof(names));
            _texts = texts != null ? texts : throw new ArgumentNullException(nameof(texts));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));

            if (config.Catalog == null || config.Catalog.Structures == null || config.Catalog.Structures.Length == 0)
            {
                throw new InvalidOperationException("PlacementController: Camp Config needs a Structure Catalog with at least one structure.");
            }

            _definitions = config.Catalog.Structures;
            _specs = new StructureSpec[_definitions.Length];
            for (var i = 0; i < _definitions.Length; i++)
            {
                _specs[i] = _definitions[i].ToSpec();
            }

            _diedSubscription = bus.Subscribe<PlayerDied>(_ =>
            {
                _playerDead = true;
                Close();
            });
        }

        /// <inheritdoc />
        public bool IsOpen { get; private set; }

        /// <inheritdoc />
        public int EntryCount => _specs.Length;

        /// <inheritdoc />
        public int SelectedIndex { get; private set; }

        /// <inheritdoc />
        public string GetEntryName(int index) => _specs[index].DisplayName;

        /// <inheritdoc />
        public IReadOnlyList<ItemAmount> GetEntryCost(int index) => _specs[index].Cost;

        /// <inheritdoc />
        public IReadOnlyList<ItemId> GetEntryRequiredItems(int index) => _specs[index].RequiredItems;

        /// <inheritdoc />
        public void Tick()
        {
            if (_playerDead)
            {
                return;
            }

            if (!IsOpen)
            {
                IsOpen = _input.PlacementPressed;
                return;
            }

            if (_input.PlacementPressed || _input.CancelPressed)
            {
                Close();
                return;
            }

            var number = _input.NumberPressed;
            if (number >= 0 && number < _specs.Length)
            {
                SelectedIndex = number;
            }

            SelectedIndex = (SelectedIndex + _input.SelectionScroll + _specs.Length) % _specs.Length;

            var definition = _definitions[SelectedIndex];
            var position = new Vector3(
                _player.X + _player.ForwardX * _config.PreviewDistance,
                _player.Y,
                _player.Z + _player.ForwardZ * _config.PreviewDistance);
            var blocked = Physics.CheckSphere(
                position + Vector3.up * definition.FootprintRadius,
                definition.FootprintRadius,
                _config.ObstacleMask,
                QueryTriggerInteraction.Ignore);
            UpdatePreview(definition, position, blocked);

            // A click on the HUD (a bag slot, a menu entry) is not a placement click.
            var overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (_input.ConfirmPressed && !overUi)
            {
                Place(definition, _specs[SelectedIndex], position, blocked);
            }
        }

        /// <summary>Unsubscribes and removes the preview.</summary>
        public void Dispose()
        {
            _diedSubscription.Dispose();
            Close();
        }

        private void Place(StructureDefinition definition, StructureSpec spec, Vector3 position, bool blocked)
        {
            if (blocked)
            {
                _bus.Publish(new NoticePosted(CampTextKeys.PlacementBlockedNotice, string.Empty));
                return;
            }

            switch (_camp.TryPlace(spec, position.x, position.z))
            {
                case PlaceResult.MissingRequiredItem:
                    for (var i = 0; i < spec.RequiredItems.Count; i++)
                    {
                        if (_bag.Count(spec.RequiredItems[i]) < 1)
                        {
                            _bus.Publish(new NoticePosted(
                                CampTextKeys.MissingRequiredNotice, _names.GetDisplayName(spec.RequiredItems[i])));
                            break;
                        }
                    }

                    return;
                case PlaceResult.MissingCost:
                    _bus.Publish(new NoticePosted(
                        CampTextKeys.MissingMaterialsNotice, _texts.FormatItems(spec.Cost, _names, _bag)));
                    return;
            }

            var instance = _resolver.Instantiate(definition.Prefab, position, Quaternion.identity);
            if (instance.TryGetComponent<StructureView>(out var view))
            {
                view.Initialize(definition);
            }

            Close();
        }

        private void UpdatePreview(StructureDefinition definition, Vector3 position, bool blocked)
        {
            if (_previewIndex != SelectedIndex)
            {
                DestroyPreview();
                _previewIndex = SelectedIndex;
                if (definition.PreviewPrefab != null)
                {
                    _preview = UnityEngine.Object.Instantiate(definition.PreviewPrefab, position, Quaternion.identity);
                    _previewRenderers = _preview.GetComponentsInChildren<Renderer>();
                    ApplyPreviewMaterial(blocked);
                }
            }

            if (_preview == null)
            {
                return;
            }

            _preview.transform.position = position;
            if (blocked != _previewBlocked)
            {
                ApplyPreviewMaterial(blocked);
            }
        }

        private void ApplyPreviewMaterial(bool blocked)
        {
            _previewBlocked = blocked;
            var material = blocked ? _config.PreviewBlockedMaterial : _config.PreviewValidMaterial;
            if (material == null)
            {
                return;
            }

            for (var i = 0; i < _previewRenderers.Length; i++)
            {
                _previewRenderers[i].sharedMaterial = material;
            }
        }

        private void Close()
        {
            IsOpen = false;
            DestroyPreview();
        }

        private void DestroyPreview()
        {
            if (_preview != null)
            {
                UnityEngine.Object.Destroy(_preview);
            }

            _preview = null;
            _previewIndex = -1;
        }
    }
}
