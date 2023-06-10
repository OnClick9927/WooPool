using System;
using System.Collections.Generic;

namespace WooPool
{

    /// <summary>
    /// 基础对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : PoolUnit, IDisposable, IObjectPool
    {
        /// <summary>
        /// 数据容器
        /// </summary>
        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy = new Lazy<Queue<T>>(() => { return new Queue<T>(); }, true);
        /// <summary>
        /// 自旋锁
        /// </summary>
        protected object para = new object();
        /// <summary>
        /// 对象池的对象的类型
        /// </summary>
        public virtual Type type { get { return typeof(T); } }
        /// <summary>
        /// 对象池中当前对象的数量
        /// </summary>
        public int count { get { return pool.Count; } }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="arg">获取参数</param>
        /// <returns>对应对象池类型的对象</returns>
        public virtual T Get(IPoolArgs arg = null)
        {
            lock (para)
            {
                T t;
                if (pool.Count > 0)
                {
                    t = pool.Dequeue();
                }
                else
                {
                    t = CreateNew(arg);
                    OnCreate(t, arg);
                    (t as IPoolObject)?.OnAllocate();
                }
                OnGet(t, arg);
                (t as IPoolObject)?.OnGet();
                return t;
            }
        }
        /// <summary>
        /// 将对象回收
        /// </summary>
        /// <param name="obj">被回收的对象</param>
        /// <param name="args">回收参数</param>
        /// <returns>对象是否回收成功</returns>
        public bool Set(object obj, IPoolArgs args)
        {
            if (obj is T)
            {
                return Set((T)obj, args);
            }
            return false;
        }
        protected void RealSet(T t, IPoolArgs arg = null)
        {
            pool.Enqueue(t);
            (t as IPoolObject)?.OnSet();
        }
        /// <summary>
        /// 将对象回收
        /// </summary>
        /// <param name="t">对应类型的对象</param>
        /// <param name="arg">回收参数</param>
        /// <returns>对象是否回收成功</returns>
        public virtual bool Set(T t, IPoolArgs arg = null)
        {
            lock (para)
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg))
                    {
                        RealSet(t, arg);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 清空对象池里的所有对象
        /// </summary>
        /// <param name="arg">清空参数</param>
        public void Clear(IPoolArgs arg = null)
        {
            lock (para)
            {
                while (pool.Count > 0)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                    (t as IDisposable)?.Dispose();
                }
            }
        }
        /// <summary>
        /// 清除对象池里一定数量的对象
        /// </summary>
        /// <param name="clearCount">需要清除的数量</param>
        /// <param name="arg">清除参数</param>
        public void Clear(int clearCount, IPoolArgs arg = null)
        {
            lock (para)
            {
                int restCount = clearCount > pool.Count ? 0 : pool.Count - clearCount;
                while (pool.Count > restCount)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                }
            }
        }
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <param name="arg">创建参数</param>
        /// <returns></returns>
        protected abstract T CreateNew(IPoolArgs arg);
        /// <summary>
        /// 对象被清除时调用
        /// </summary>
        /// <param name="t">对应类型的对象</param>
        /// <param name="arg">清除参数</param>
        protected virtual void OnClear(T t, IPoolArgs arg) { }
        /// <summary>
        /// 对象被回收时调用
        /// </summary>
        /// <param name="t">回收的对象</param>
        /// <param name="arg">回收参数</param>
        /// <returns>对象是否回收成功</returns>
        protected virtual bool OnSet(T t, IPoolArgs arg)
        {
            return true;
        }
        /// <summary>
        /// 对象被获取时调用
        /// </summary>
        /// <param name="t">获取的对象</param>
        /// <param name="arg">获取参数</param>
        protected virtual void OnGet(T t, IPoolArgs arg) { }
        /// <summary>
        /// 对象被创建时调用
        /// </summary>
        /// <param name="t">创建的对象</param>
        /// <param name="arg">创建参数</param>
        protected virtual void OnCreate(T t, IPoolArgs arg) { }
        /// <summary>
        /// 对象池被销毁时调用
        /// </summary>
        protected override void OnDispose()
        {
            Clear(null);
        }
    }
}
