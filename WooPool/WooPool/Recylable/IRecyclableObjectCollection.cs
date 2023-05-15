using System;

namespace WooPool
{
    public interface IRecyclableObjectCollection
    {
        RecyclableObject Get(Type type, IPoolArgs arg = null);
        Object Get<Object>(IPoolArgs arg = null) where Object : RecyclableObject;
        bool GetFromMemory(Guid id, out RecyclableObject obj);
        void Recyle(Guid id);
        void RecyleAll();
        void Set(Type type, RecyclableObject t, IPoolArgs arg = null);
        void Set<Object>(Object t, IPoolArgs arg = null) where Object : RecyclableObject;
    }
}