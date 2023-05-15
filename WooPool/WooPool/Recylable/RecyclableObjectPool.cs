namespace WooPool
{
    internal partial class RecyclableObjectCollection
    {
        private class RecyclableObjectPool : BaseTypePool<RecyclableObject>
        {
            protected override void OnDispose()
            {
               
            }
        }
    }
}
