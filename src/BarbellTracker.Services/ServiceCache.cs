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
                return TryGetCachedItemWithoutLock(key, out item);
            }
        }

        public T AddItemToCache(TrackedInformation key, T Item)
        {
            lock (_lock)
            {

                if (HasItemNotChached(key, Item))
                {
                    AddItem(key, Item);
                }
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

        private void AddItem(TrackedInformation key, T Item)
        {
            _chache.Add(key, Item);
            RemoveOldItems();
        }


        private bool TryGetCachedItemWithoutLock(TrackedInformation key, out T item)
        {
            return _chache.TryGetValue(key, out item);

        }

        private bool HasItemChached(TrackedInformation key, T Item)
        {
            if (_chache.TryGetValue(key, out var value))
            {
                if (value.Equals(Item))
                {
                    return true;
                };

                throw new KeyAlreadyExist($"The cache has Already an item with the same Key: {key} but a Different Value");
            }
            return false;
        }

        private bool HasItemNotChached(TrackedInformation key, T Item)
        {
            return !HasItemChached(key, Item);
        }
    }
}
