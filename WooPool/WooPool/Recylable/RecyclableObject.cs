using System;

namespace WooPool
{
    /// <summary>
    /// 可回收类
    /// </summary>
    public abstract class RecyclableObject : PoolUnit
    {
        private static IRecyclableObjectCollection GetCollection()
        {
            return PoolEx.GetCollection();
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type)
        {
            RecyclableObject t = PoolEx.GlobalAllocate(type) as RecyclableObject;
            t.OnAllocate();
            return t;
        }




        /// <summary>
        /// 通过唯一id回收对象
        /// </summary>
        /// <param name="env"></param>
        /// <param name="guid"></param>
        public static void RecyleByGuid(Guid guid)
        {
            GetCollection().Recyle(guid);
        }
        /// <summary>
        /// 回收所有实例
        /// </summary>
        /// <param name="env"></param>
        public static void RecyleAll()
        {
            GetCollection().RecyleAll();
        }
        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="env"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetFromMemory(Guid id, out RecyclableObject obj)
        {
            return GetCollection().GetFromMemory(id, out obj);
        }



        private bool _recyled;
        private bool _datadirty;
        private Guid _guid = Guid.NewGuid();

        /// <summary>
        /// 是否被回收
        /// </summary>
        public bool recyled { get { return _recyled; } }
        /// <summary>
        /// 数据是否发生改变
        /// </summary>
        protected bool dataDirty { get { return _datadirty; } }

        /// <summary>
        /// 唯一 id
        /// </summary>
        public Guid guid { get { return _guid; } }

        /// <summary>
        /// 回收
        /// </summary>
        public void Recyle()
        {
            if (_recyled) return;
            OnRecyle();
            _recyled = true;
            GetCollection().Set(this);
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        protected void ResetData()
        {
            if (!_datadirty) return;
            OnDataReset();
            _datadirty = false;
        }
        /// <summary>
        /// 设置数据发生改动
        /// </summary>
        protected void SetDataDirty()
        {
            _datadirty = true;
        }

        /// <summary>
        /// 被分配时
        /// </summary>
        protected virtual void OnAllocate()
        {
            _recyled = false;
            ResetData();
        }
        /// <summary>
        /// 被回收时
        /// </summary>
        protected virtual void OnRecyle() { ResetData(); }
        /// <summary>
        /// 数据重置时
        /// </summary>
        protected abstract void OnDataReset();
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            _guid = Guid.Empty;
        }
    }
}
