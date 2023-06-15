using System;
using System.Collections.Generic;
using System.Reflection;

namespace WooPool
{
    /// <summary>
    /// 基类对象池
    /// </summary>
    /// <remarks>可以用于从对象池中取出对应基类的子类对象</remarks>
    /// <typeparam name="T">对象的基类类型</typeparam>
    public abstract class BaseTypePool<T> : PoolUnit
    {

        private Dictionary<Type, IObjectPool> _poolMap;
        private MethodInfo _getMethodInfo;
        private MethodInfo _setMethodInfo;

        private Dictionary<Type, MethodInfo> _getpools = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> _setpools = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// 自旋锁
        /// </summary>
        //protected LockParam para = new LockParam();
        private object para = new object();

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseTypePool()
        {
            Type[] types = new Type[] { typeof(IPoolArgs) };
            Type type = GetType();
            _getMethodInfo = type.GetMethod(nameof(Get), types);
            _setMethodInfo = type.GetMethod(nameof(Set), types);
            _poolMap = new Dictionary<Type, IObjectPool>();
        }
        /// <summary>
        /// 设置内部对象池
        /// </summary>
        /// <typeparam name="Object">泛型类型</typeparam>
        /// <param name="pool">对应类型的对象池</param>
        public void SetPool<Object>(ObjectPool<Object> pool) where Object : T
        {
            SetPool(typeof(Object), pool);
        }
        /// <summary>
        /// 设置内部对象池
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="pool">对象池</param>
        public void SetPool(Type type, IObjectPool pool)
        {
            lock (para)
            {
                if (!_poolMap.ContainsKey(type))
                    _poolMap.Add(type, pool);
                else
                    _poolMap[type] = pool;
            }
        }
        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <typeparam name="Object">类型</typeparam>
        /// <returns>对应类型的对象池</returns>
        public ObjectPool<Object> GetPool<Object>() where Object : T
        {
            Type type = typeof(Object);
            var pool = GetPool(type);
            return pool as ObjectPool<Object>;
        }
        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对应类型的对象池</returns>
        public IObjectPool GetPool(Type type)
        {
            lock (para)
            {
                IObjectPool pool;
                if (!_poolMap.TryGetValue(type, out pool))
                {
                    pool = CreatePool(type);
                    if (pool == null)
                    {
                        var pooType = typeof(ActivatorCreatePool<>).MakeGenericType(type);
                        pool = Activator.CreateInstance(pooType, null) as IObjectPool;
                    }
                    _poolMap.Add(type, pool);
                }
                return pool;
            }
        }
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对应类型的对象池</returns>
        protected virtual IObjectPool CreatePool(Type type)
        {
            return null;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="Object">类型</typeparam>
        /// <param name="arg">参数</param>
        /// <returns>对应类型的对象</returns>
        public Object Get<Object>(IPoolArgs arg = null) where Object : T
        {
            var pool = GetPool<Object>();
            Object t = pool.Get(arg);
            return t;
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="arg">参数</param>
        /// <returns>对应类型的对象</returns>
        public T Get(Type type, IPoolArgs arg = null)
        {
            MethodInfo m2;
            if (!_getpools.TryGetValue(type, out m2))
            {
                m2 = _getMethodInfo.MakeGenericMethod(type);
                _getpools.Add(type, m2);
            }
            return (T)m2.Invoke(this, new object[] { arg });
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <typeparam name="Object">类型</typeparam>
        /// <param name="t">需要回收的对象</param>
        /// <param name="arg">参数</param>
        public void Set<Object>(Object t, IPoolArgs arg = null) where Object : T
        {
            Type type = t.GetType();
            var pool = GetPool(type);
            pool.Set(t, arg);
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="t">需要回收的对象</param>
        /// <param name="arg">参数</param>
        public void Set(Type type, T t, IPoolArgs arg = null)
        {
            MethodInfo m2;
            if (!_setpools.TryGetValue(type, out m2))
            {
                m2 = _setMethodInfo.MakeGenericMethod(type);
                _setpools.Add(type, m2);
            }
            m2.Invoke(this, new object[] { t, arg });
        }


        /// <summary>
        /// 获取现有数量
        /// </summary>
        /// <typeparam name="Object">类型</typeparam>
        /// <returns>对应类型对象池中的数量</returns>
        public int GetPoolCount<Object>() where Object : T
        {
            return GetPoolCount(typeof(Object));
        }
        /// <summary>
        /// 获取现有数量
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>对应类型对象池中的数量</returns>
        public int GetPoolCount(Type type)
        {
            var pool = GetPool(type);
            return pool.count;
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        public new void Dispose()
        {
            lock (para)
            {
                if (disposed) return;
                base.Dispose();
                foreach (var item in _poolMap.Values) item.Dispose();
                _poolMap.Clear();
                _setpools.Clear();
                _getpools.Clear();
            }
        }

    }

}
