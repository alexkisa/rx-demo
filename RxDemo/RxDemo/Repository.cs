using System;
using System.Collections.Generic;
using System.Linq;

namespace RxDemo
{
    internal class Repository
    {
        private readonly Dictionary<Type, Dictionary<string, object>> _typeEntityLookup = new Dictionary<Type, Dictionary<string, object>>();
        private readonly List<Subscription> _subscriptions = new List<Subscription>();

        public T Read<T>(string id)
        {
            var entityLookup = ValueOrDefault(_typeEntityLookup, typeof(T));
            if (entityLookup == null) return default(T);

            return (T) ValueOrDefault(entityLookup, id);
        }

        public IEnumerable<T> ReadAll<T>()
        {
            var entityLookup = ValueOrDefault(_typeEntityLookup, typeof(T));
            return entityLookup?.Select(kv => kv.Value).OfType<T>() ?? Enumerable.Empty<T>();
        }

        public void Update<T>(string id, Action<T> update) where T : class, new()
        {
            var entity = Read<T>(id) ?? new T();
            update(entity);

            var type = typeof(T);
            if (!_typeEntityLookup.ContainsKey(type)) _typeEntityLookup[type] = new Dictionary<string, object>();
            _typeEntityLookup[type][id] = entity;

            _subscriptions.ForEach(s => s.TryNotify(id, entity));
        }

        public Subscription Subscribe()
        {
            var subscription = new Subscription();
            _subscriptions.Add(subscription);
            return subscription;
        }

        public void Unsubscribe(Subscription subscription)
        {
            _subscriptions.Remove(subscription);
        }

        private static TValue ValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        public class Subscription
        {
            private Func<string, object, bool> _predicate;
            private Action<string, object> _action;

            public Subscription When<T>(Func<string, T, bool> predicate) where T : class
            {
                _predicate = (id, obj) =>
                {
                    var entity = obj as T;
                    return entity != null && predicate(id, entity);
                };

                return this;
            }

            public Subscription Do<T>(Action<string, T> action) where T : class
            {
                _action = (id, obj) =>
                {
                    var entity = obj as T;
                    if (entity != null) action(id, entity);
                };

                return this;
            }

            public void TryNotify<T>(string id, T entity)
            {
                if (_predicate == null || _predicate(id, entity)) _action?.Invoke(id, entity);
            }
        }
    }
}
