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
        public ServiceCache()
        {
            Max_Cache_Size = 30;
            _chache = new Dictionary<TrackedInformation, T>();
        }

        public bool TryGetCachedItem(TrackedInformation key, out T item)
        {
            return _chache.TryGetValue(key, out item);
        }

        public T AddItemToCache(TrackedInformation key, T Item)
        {
            if (_chache.ContainsKey(key))
            {
                throw new KeyAlreadyExist($"The cache has Already an item with the same Key: {key}");
            }

            _chache.Add(key, Item);
            RemoveOldItems();

            return Item;
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
