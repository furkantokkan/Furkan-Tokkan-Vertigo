using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    public interface IPoolManager<T> where T : Component
    {
        T Get();
        void Release(T item);
        void Clear();
    }
}
