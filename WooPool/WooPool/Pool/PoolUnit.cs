using System;

namespace WooPool
{
    public abstract class PoolUnit : IDisposable
    {
        /// <summary>
        /// 对象池是否已经销毁
        /// </summary>
        protected bool disposed { get; private set; }
        /// <summary>
        /// 销毁时调用
        /// </summary>
        protected abstract void OnDispose();
        /// <summary>
        /// 销毁方法
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            OnDispose();
            disposed = true;
        }
    }
}
