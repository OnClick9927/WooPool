using System;

namespace WooPool
{
    /// <summary>
    /// 委托对象池
    /// </summary>
    /// <typeparam name="T">对象池里对象的类型</typeparam>
    public abstract class DelegatePool<T> : ObjectPool<T>
    {
        /// <summary>
        /// 对象创建时调用的委托
        /// </summary>
        private Func<T> _create;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="create">对象创建时需要调用的委托</param>
        public DelegatePool(Func<T> create)
        {
            _create = create;
        }

        /// <summary>
        /// 对象池创建新对象
        /// </summary>
        /// <param name="arg">创建参数</param>
        /// <returns>创建的对象</returns>
        protected override T CreateNew(IPoolArgs arg)
        {
            if (_create == null)
                return default;
            return _create.Invoke();
        }
    }
}
