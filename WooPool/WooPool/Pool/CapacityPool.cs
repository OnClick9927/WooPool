using System.Collections.Generic;

namespace WooPool
{
    /// <summary>
    /// 有容量的对象池
    /// </summary>
    /// <typeparam name="T">对象池的类型</typeparam>
    public abstract class CapacityPool<T>: ObjectPool<T>
    {
        private int _capacity;
        /// <summary>
        /// 最大存储容量
        /// </summary>
        public int capacity { get { return _capacity; } set { _capacity = value; } }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capacity">最大容量</param>
        protected CapacityPool(int capacity) : base() { this._capacity = capacity; }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="t">回收的对象</param>
        /// <param name="arg">回收参数</param>
        /// <returns>对象是否回收成功，如果容量满了则回收失败</returns>
        protected override bool OnSet(T t, IPoolArgs arg)
        {
            return count <= capacity;
        }
    }

}
