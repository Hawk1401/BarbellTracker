using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Services
{
    public class ServiceCache<T>
    {
        private Dictionary<TrackedInformation, T> _cache;
        public int Max_Cache_Size { get; init; }
        private object _lock;
        public ServiceCache()
        {
            Max_Cache_Size = 30;
            _cache = new Dictionary<TrackedInformation, T>();
            _lock = new object();
        }

        public bool TryGetCachedItem(TrackedInformation key, out T item)
        {
            lock (_lock)
            {
                return _cache.TryGetValue(key, out item);
            }
        }

        public T AddItemToCache(TrackedInformation key, T Item)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    if (value.Equals(Item))
                    {
                        return Item;
                    };

                    var hash1 = _cache.Keys.First().GetHashCode();
                    var hash2 = key.GetHashCode();

                    throw new KeyAlreadyExist($"The cache has Already an item with the same Key: {key} but a Different Value");
                }

                _cache.Add(key, Item);
                RemoveOldItems();

                return Item;
            }
        }

        private void RemoveOldItems()
        {
            if(_cache.Count < Max_Cache_Size)
            {
                return;
            }

            var firstKey = _cache.Keys.First();
            _cache.Remove(firstKey);
        }
    }
}
