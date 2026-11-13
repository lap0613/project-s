using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Whiteout.UI
{
    /// <summary>
    /// Reports when the mouse enters or leaves a weather icon of the forecast timeline, so the timeline can show
    /// its tooltip (ui-plan 7.3 [B]). Needs a raycast-target graphic on the same object.
    /// </summary>
    /// <example>
    /// <code>
    /// var hover = iconObject.AddComponent&lt;WeatherIconHover&gt;();
    /// hover.Initialize(slot, OnEnter, OnExit);
    /// </code>
    /// </example>
    public sealed class WeatherIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private int _slot;
        private Action<int> _entered;
        private Action<int> _exited;

        /// <summary>Sets the slot id and the callbacks.</summary>
        /// <param name="slot">Id passed back to the callbacks.</param>
        /// <param name="entered">Called when the mouse enters.</param>
        /// <param name="exited">Called when the mouse leaves.</param>
        public void Initialize(int slot, Action<int> entered, Action<int> exited)
        {
            _slot = slot;
            _entered = entered;
            _exited = exited;
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            _entered?.Invoke(_slot);
        }

        /// <inheritdoc />
        public void OnPointerExit(PointerEventData eventData)
        {
            _exited?.Invoke(_slot);
        }
    }
}
