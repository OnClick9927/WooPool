using System;

namespace WooPool
{
    public static class PoolEx
    {
        private class GPool : BaseTypePool<object>
        {
            protected override IObjectPool CreatePool(Type type)
            {
                if (type.IsArray)
                {
                    var poolType = typeof(ArrayPool<>).MakeGenericType(type.GetElementType());
                    return Activator.CreateInstance(poolType) as IObjectPool;
                }
                return null;
            }

            protected override void OnDispose()
            {

            }
        }
        static private GPool gPool = new GPool();

        /// <summary>
        /// 获取对应类型对象池中的对象数量
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>对应类型的对象池中对象的数量</returns>
        public static int GetGlbalPoolCount<T>()
        {
            return gPool.GetPoolCount<T>();
        }
        /// <summary>
        /// 设置全局对象池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="pool">对象池</param>
        public static void SetGlbalPool<T>(ObjectPool<T> pool)
        {
            gPool.SetPool(pool);
        }
        /// <summary>
        /// 全局分配
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="arg">分配参数</param>
        /// <returns>对应的类型的对象</returns>
        public static T GlobalAllocate<T>(IPoolArgs arg = null) where T : class
        {
            return gPool.Get<T>(arg);
        }
        public static Object GlobalAllocate(Type type, IPoolArgs arg = null)
        {
            return gPool.Get(type, arg);
        }
        /// <summary>
        /// 全局回收
        /// </summary>
        /// <typeparam name="T">回收的对象类型</typeparam>
        /// <param name="t">回收的对象</param>
        /// <param name="arg">回收参数</param>
        public static void GlobalRecyle<T>(T t, IPoolArgs arg = null) where T : class
        {
            gPool.Set(t, arg);
        }
        /// <summary>
        /// 分配数组
        /// </summary>
        /// <typeparam name="T">数组的元素类型</typeparam>
        /// <param name="length">数组长度</param>
        /// <returns>分配的数组对象</returns>
        public static T[] GlobalAllocateArray<T>(int length)
        {
            var result = GlobalAllocate<T[]>(new ArrayPoolArg(length));
            return result;
        }   
    }
}
