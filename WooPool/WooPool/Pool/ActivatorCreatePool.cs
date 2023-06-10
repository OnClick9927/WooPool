using System;

namespace WooPool
{
    /// <summary>
    /// 自动管理对象池（使用Activator创建对象）
    /// </summary>
    /// <typeparam name="T">对象池对应对象的类型</typeparam>
    public class ActivatorCreatePool<T> : ObjectPool<T>
    {
        private object[] args;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="args">创建对象所需的构造参数</param>
        public ActivatorCreatePool(params object[] args)
        {
            this.args = args;
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="arg">创建参数</param>
        /// <returns>创建出来的对象</returns>
        protected override T CreateNew(IPoolArgs arg)
        {
            Type type = typeof(T);
            return (T)Activator.CreateInstance(type, args);
        }
    }
}
