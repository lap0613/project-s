using System;

namespace Whiteout.Core.Events
{
    /// <summary>
    /// Publish/subscribe channel for "something happened" notifications between systems.
    /// Not thread-safe: call it from the main thread only.
    /// </summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;DayStarted&gt;(e =&gt; OnDay(e));
    /// bus.Publish(new DayStarted(1));
    /// </code>
    /// </example>
    public interface IEventBus
    {
        /// <summary>
        /// Delivers <paramref name="evt"/> to every active subscriber of <typeparamref name="T"/>.
        /// A throwing handler does not stop the remaining handlers. Handlers may subscribe or
        /// unsubscribe during delivery; changes take effect from the next publish, except that a handler
        /// disposed during delivery is not called afterwards.
        /// </summary>
        /// <typeparam name="T">Event type (exact type match, no base-type delivery).</typeparam>
        /// <param name="evt">The event payload.</param>
        void Publish<T>(T evt);

        /// <summary>Registers <paramref name="handler"/> for events of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">Event type.</typeparam>
        /// <param name="handler">Callback invoked on publish.</param>
        /// <returns>A handle; dispose it to unsubscribe. Disposing twice is safe.</returns>
        IDisposable Subscribe<T>(Action<T> handler);
    }
}
