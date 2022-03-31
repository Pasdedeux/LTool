using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LitFramework.LitPool
{
    internal static class ListPool<T>
    {
        // �����ڴ����
        private static readonly ObjectPool<List<T>> _listPool = new ObjectPool<List<T>>(null, Clear);
        static void Clear(List<T> l) { l.Clear(); }

        public static List<T> Get()
        {
            return _listPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            _listPool.Release(toRelease);
        }


        private class ObjectPool<T> where T : new()
        {
            private readonly Stack<T> _stack = new Stack<T>();
            private readonly UnityAction<T> _actionOnGet;
            private readonly UnityAction<T> _actionOnRelease;

            public int CountAll { get; private set; }
            public int CountActive { get { return CountAll - CountInactive; } }
            public int CountInactive { get { return _stack.Count; } }

            public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
            {
                _actionOnGet = actionOnGet;
                _actionOnRelease = actionOnRelease;
            }

            public T Get()
            {
                T element;
                if (_stack.Count == 0)
                {
                    element = new T();
                    CountAll++;
                }
                else
                {
                    element = _stack.Pop();
                }
                if (_actionOnGet != null)
                    _actionOnGet(element);
                return element;
            }

            public void Release(T element)
            {
                if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
                    Log.Info("Internal error. Trying to destroy object that is already released to pool.");
                if (_actionOnRelease != null)
                    _actionOnRelease(element);
                _stack.Push(element);
            }
        }
    }
}
