using System;
using System.Collections.Generic;

namespace WooPool
{
    internal partial class RecyclableObjectCollection
    {
        private class RecyclableObjectMemory : IDisposable
        {
            private object _lock = new object();
            private Dictionary<Guid, RecyclableObject> _map;
            public RecyclableObjectMemory()
            {
                _map = new Dictionary<Guid, RecyclableObject>();
            }
            public void Dispose()
            {
                _map = null;
            }

            public void Set(RecyclableObject obj)
            {
                lock (_lock)
                {
                    Guid id = obj.guid;
                    if (_map.ContainsKey(id))
                        throw new Exception("Same Key");
                    else
                        _map.Add(id, obj);
                }
            }

            public bool Exist(Guid id, out RecyclableObject obj)
            {
                lock (_lock)
                {
                    return _map.TryGetValue(id, out obj);
                }
            }
            public void Remove(Guid id)
            {
                lock (_lock)
                {
                    bool bo = _map.Remove(id);
                }
            }

            public Guid[] GetGuids()
            {
                lock (_lock)
                {
                    Guid[] ids = new Guid[_map.Count];
                    int index = 0;
                    foreach (var item in _map.Keys)
                    {
                        ids[index++] = item;
                    }
                    return ids;
                }

            }
        }
    }
}
