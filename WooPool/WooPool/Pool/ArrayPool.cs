using System.Collections.Generic;

namespace WooPool
{
    /// <summary>
    /// ArrayPoolArg
    /// </summary>
    public struct ArrayPoolArg : IPoolArgs
    {
        /// <summary>
        /// 长度
        /// </summary>
        public int length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public ArrayPoolArg(int length)
        {
            this.length = length;
        }
    }
    /// <summary>
    /// 数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayPool<T> : ObjectPool<T[]>
    {
        private Queue<int> _lengthqueue = new Queue<int>();
        /// <summary>
        /// 创建数组
        /// </summary>
        /// <param name="arg">创建参数</param>
        /// <returns>创建出来的数组</returns>
        protected override T[] CreateNew(IPoolArgs arg)
        {
            ArrayPoolArg len = (ArrayPoolArg)arg;
            return new T[len.length];
        }
        Queue<T[]> queue = new Queue<T[]>();

        /// <summary>
        /// 从数组对象池中获取数组对象
        /// </summary>
        /// <param name="arg">参数</param>
        /// <returns>数组对象</returns>
        public override T[] Get(IPoolArgs arg = null)
        {
            ArrayPoolArg len = (ArrayPoolArg)arg;
            int length = len.length;
            lock (para)
            {
                T[] t;
                if (pool.Count > 0 && _lengthqueue.Contains(length))
                {
                    while (_lengthqueue.Peek() != length)
                    {
                        _lengthqueue.Dequeue();
                        queue.Enqueue(pool.Dequeue());
                    }
                    t = pool.Dequeue();
                    while (pool.Count != 0) queue.Enqueue(pool.Dequeue());
                    int _count = queue.Count;
                    for (int i = 0; i < _count; i++)
                    {
                        var tmp = queue.Dequeue();
                        int _len = tmp.Length;
                        _lengthqueue.Enqueue(_len);
                        pool.Enqueue(tmp);
                    }
                }
                else
                {
                    t = CreateNew(arg);
                    OnCreate(t, arg);
                }
                OnGet(t, arg);
                return t;
            }
        }

        /// <summary>
        /// 回收数组对象
        /// </summary>
        /// <param name="t">数组对象</param>
        /// <param name="arg">参数</param>
        /// <returns>回收是否成功</returns>
        public override bool Set(T[] t, IPoolArgs arg = null)
        {
            lock (para)
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg))
                    {
                        int _len = t.Length;
                        _lengthqueue.Enqueue(_len);
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
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
            if (_lengthqueue != null)
            {
                _lengthqueue.Clear();
            }
        }
    }
}
