using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace EFChangeNotify
{
    public static class SafeCountDictionary
    {
        private class SafeCount : MarshalByRefObject
        {
            private int _counter;
            public int Counter { get { return _counter; } }

            public void Increment()
            {
                Interlocked.Increment(ref _counter);
            }

            public void Decrement()
            {
                Interlocked.Decrement(ref _counter);
            }
        }

        private static ConcurrentDictionary<string, SafeCount> _registeredConnectionStrings = new ConcurrentDictionary<string, SafeCount>();

        private static ConcurrentDictionary<string, SafeCount> _registeredSqlStrings = new ConcurrentDictionary<string, SafeCount>();

        /// <summary>
        /// Increments the count for the given key by one. If the key does not exist, it creates it and sets the count to 1.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="onAdd">Executes when the key is first created. Does not run on updates.</param>
        public static void IncrementConnectionString(string key, Action<string> onAdd)
        {
            var safeCount = _registeredConnectionStrings.GetOrAdd(key, x =>
            {
                onAdd(x);
                return new SafeCount();
            });

            safeCount.Increment();
        }

        /// <summary>
        /// Increments the count for the given key by one. If the key does not exist, it creates it and sets the count to 1.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="onAdd">Executes when the key is first created. Does not run on updates.</param>
        public static void IncrementSqlString(string key, Action<string> onAdd)
        {
            var safeCount = _registeredSqlStrings.GetOrAdd(key, x =>
            {
                onAdd(x);
                return new SafeCount();
            });

            safeCount.Increment();
        }

        internal static ICollection<String> GetAllDependencieis()
        {
            return _registeredConnectionStrings.Keys;
        }

        /// <summary>
        /// Decrements the count for the given key by one. If the count reaches 0, it runs <paramref name="onZero"/>.
        /// </summary>
        /// <param name="key">The key to decrement.</param>
        /// <param name="onZero">Executes when the count equals zero.</param>
        public static void DecrementConnectionString(string key, Action<string> onZero)
        {
            SafeCount val;

            if (_registeredConnectionStrings.TryGetValue(key, out val))
            {
                val.Decrement();

                if (val.Counter == 0)
                {
                    onZero(key);
                }
            }
        }

        /// <summary>
        /// Decrements the count for the given key by one. If the count reaches 0, it runs <paramref name="onZero"/>.
        /// </summary>
        /// <param name="key">The key to decrement.</param>
        /// <param name="onZero">Executes when the count equals zero.</param>
        public static void DecrementSqlString(string key, Action<string> onZero)
        {
            SafeCount val;

            if (_registeredSqlStrings.TryGetValue(key, out val))
            {
                val.Decrement();

                if (val.Counter == 0)
                {
                    onZero(key);
                }
            }
        }
    }
}
