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
        private Dictionary<TrackedInformation, T> _chache;
        public int Max_Cache_Size { get; init; }
        private object _lock;
        public ServiceCache()
        {
            Max_Cache_Size = 30;
            _chache = new Dictionary<TrackedInformation, T>();
            _lock = new object();
        }

        public bool TryGetCachedItem(TrackedInformation key, out T item)
        {
            lock (_lock)
            {
                return _chache.TryGetValue(key, out item);
            }
        }

        public T AddItemToCache(TrackedInformation key, T Item)
        {
            lock (_lock)
            {
                if (_chache.TryGetValue(key, out var value))
                {
                    if (value.Equals(Item))
                    {
                        return Item;
                    };

                    var hash1 = _chache.Keys.First().GetHashCode();
                    var hash2 = key.GetHashCode();

                    throw new KeyAlreadyExist($"The cache has Already an item with the same Key: {key} but a Different Value");
                }

                _chache.Add(key, Item);
                RemoveOldItems();

                return Item;
            }
        }

        private void RemoveOldItems()
        {
            if(_chache.Count < Max_Cache_Size)
            {
                return;
            }

            var firstKey = _chache.Keys.First();
            _chache.Remove(firstKey);
        }
    }
}
