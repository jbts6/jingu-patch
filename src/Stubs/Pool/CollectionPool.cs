using System.Collections.Generic;

namespace UnityEngine.Pool
{
    public class ObjectPool<T>
    {
        public ObjectPool(System.Func<T> createFunc, System.Action<T> actionOnGet, System.Action<T> actionOnRelease, System.Action<T> actionOnDestroy, bool collectionCheck, int defaultCapacity, int maxSize) { }
        public T Get() { return default(T); }
        public PooledObject<T> Get(out T value) { value = default(T); return new PooledObject<T>(value); }
        public void Release(T toRelease) { }
    }

    public struct PooledObject<TCollection>
    {
        public PooledObject(TCollection value) { }
    }

    public class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
    {
        internal static readonly ObjectPool<TCollection> s_Pool = new ObjectPool<TCollection>(() => new TCollection(), null, delegate (TCollection l) { l.Clear(); }, null, true, 10, 10000);

        public static TCollection Get()
        {
            return CollectionPool<TCollection, TItem>.s_Pool.Get();
        }

        public static PooledObject<TCollection> Get(out TCollection value)
        {
            return CollectionPool<TCollection, TItem>.s_Pool.Get(out value);
        }

        public static void Release(TCollection toRelease)
        {
            CollectionPool<TCollection, TItem>.s_Pool.Release(toRelease);
        }
    }
}
