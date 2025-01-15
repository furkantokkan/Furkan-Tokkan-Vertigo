using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Utilities
{
    public class ObjectPoolManager<T> : IPoolManager<T> where T : Component
    {
        private readonly IObjectPool<T> pool;
        private readonly T prefab;

        public ObjectPoolManager(T prefab, int defaultCapacity = 10)
        {
            this.prefab = prefab;
            pool = new ObjectPool<T>(
                createFunc: CreateItem,
                actionOnGet: OnGetItem,
                actionOnRelease: OnReleaseItem,
                actionOnDestroy: OnDestroyItem,
                defaultCapacity: defaultCapacity
            );
        }

        protected virtual T CreateItem()
        {
            var item = Object.Instantiate(prefab);
            item.gameObject.SetActive(false);
            return item;
        }

        protected virtual void OnGetItem(T item)
        {
            item.gameObject.SetActive(true);
        }

        protected virtual void OnReleaseItem(T item)
        {
            item.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyItem(T item)
        {
            Object.Destroy(item.gameObject);
        }

        public virtual T Get() => pool.Get();
        public virtual void Release(T item) => pool.Release(item);
        public virtual void Clear() => pool.Clear();
    }

}
