using System;

namespace WooPool
{
    /// <summary>
    /// 对象池生命周期
    /// </summary>
    public interface IPoolObject : IDisposable
    {
        /// <summary>
        /// 在对象被创建时调用
        /// </summary>
        void OnAllocate();
        /// <summary>
        /// 在对象从对象池中被取出时调用
        /// </summary>
        void OnGet();
        /// <summary>
        /// 在对象被回收回对象池时调用
        /// </summary>
        void OnSet();
    }
}
