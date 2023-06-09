﻿using System;

namespace WooPool
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    public interface IObjectPool:IDisposable {
        /// <summary>
        /// 数量
        /// </summary>
        int count { get; }
        /// <summary>
        /// 类型
        /// </summary>
        Type type { get; }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj">回收的对象</param>
        /// <param name="args">参数</param>
        bool Set(object obj,IPoolArgs args);
    }
}
