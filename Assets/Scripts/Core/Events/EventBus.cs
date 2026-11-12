using System;
using System.Collections.Generic;

namespace Whiteout.Core.Events
{
    /// <summary>
    /// Default <see cref="IEventBus"/>. Subscriber lists are copy-on-write, so publishing allocates nothing
    /// and handlers can change subscriptions while a publish is running. Not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// var bus = new EventBus(ex =&gt; log(ex));
    /// </code>
    /// </example>
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Array> _subscriptions = new Dictionary<Type, Array>();
        private readonly Action<Exception> _onHandlerException;

        /// <summary>Creates a bus.</summary>
        /// <param name="onHandlerException">
        /// Called with every exception thrown by a handler. If null, handler exceptions are swallowed.
        /// </param>
        public EventBus(Action<Exception> onHandlerException = null)
        {
            _onHandlerException = onHandlerException;
        }

        /// <inheritdoc />
        public void Publish<T>(T evt)
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var array))
            {
                return;
            }

            var snapshot = (Subscription<T>[])array;
            for (var i = 0; i < snapshot.Length; i++)
            {
                var subscription = snapshot[i];
                if (!subscription.IsActive)
                {
                    continue;
                }

                try
                {
                    subscription.Handler(evt);
                }
                catch (Exception ex)
                {
                    _onHandlerException?.Invoke(ex);
                }
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var subscription = new Subscription<T>(this, handler);
            var current = _subscriptions.TryGetValue(typeof(T), out var array)
                ? (Subscription<T>[])array
                : Array.Empty<Subscription<T>>();

            var next = new Subscription<T>[current.Length + 1];
            Array.Copy(current, next, current.Length);
            next[current.Length] = subscription;
            _subscriptions[typeof(T)] = next;
            return subscription;
        }

        private void Remove<T>(Subscription<T> subscription)
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var array))
            {
                return;
            }

            var current = (Subscription<T>[])array;
            var index = Array.IndexOf(current, subscription);
            if (index < 0)
            {
                return;
            }

            if (current.Length == 1)
            {
                _subscriptions.Remove(typeof(T));
                return;
            }

            var next = new Subscription<T>[current.Length - 1];
            Array.Copy(current, 0, next, 0, index);
            Array.Copy(current, index + 1, next, index, current.Length - index - 1);
            _subscriptions[typeof(T)] = next;
        }

        private sealed class Subscription<T> : IDisposable
        {
            private readonly EventBus _owner;

            public Subscription(EventBus owner, Action<T> handler)
            {
                _owner = owner;
                Handler = handler;
                IsActive = true;
            }

            public Action<T> Handler { get; }

            public bool IsActive { get; private set; }

            public void Dispose()
            {
                if (!IsActive)
                {
                    return;
                }

                IsActive = false;
                _owner.Remove(this);
            }
        }
    }
}
