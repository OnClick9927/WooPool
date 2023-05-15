using System;

namespace WooPool
{
    internal partial class RecyclableObjectCollection : PoolUnit, IRecyclableObjectCollection
    {
        public RecyclableObjectCollection()
        {
            _createPool = new RecyclableObjectPool();
            _memory = new RecyclableObjectMemory();
        }
       

        protected override void OnDispose()
        {
            RecyleAll();
            _memory.Dispose();
            _createPool.Dispose();
        }

        private RecyclableObjectPool _createPool;
        private RecyclableObjectMemory _memory;

        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public RecyclableObject Get(Type type, IPoolArgs arg = null)
        {
            var obj = _createPool.Get(type, arg);
            _memory.Set(obj);
            return obj;
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Object Get<Object>(IPoolArgs arg = null) where Object : RecyclableObject
        {
            Object t = _createPool.Get<Object>(arg);
            _memory.Set(t);
            return t;
        }

        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set(Type type, RecyclableObject t, IPoolArgs arg = null)
        {
            _memory.Remove(t.guid);
            _createPool.Set(type, t, arg);
        }
        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t, IPoolArgs arg = null) where Object : RecyclableObject
        {
            _memory.Remove(t.guid);
            _createPool.Set<Object>(t, arg);
        }

        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool GetFromMemory(Guid id, out RecyclableObject obj)
        {
            return _memory.Exist(id, out obj);
        }

        /// <summary>
        /// 回收一个运行中的实例
        /// </summary>
        public void Recyle(Guid id)
        {
            RecyclableObject obj;
            bool bo = _memory.Exist(id, out obj);
            if (bo)
            {
                obj.Recyle();
            }
        }
        /// <summary>
        /// 回收所有运行中的实例
        /// </summary>
        public void RecyleAll()
        {
            var ids = _memory.GetGuids();
            for (int i = 0; i < ids.Length; i++)
            {
                Recyle(ids[i]);
            }
        }
    }
}
