using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace LitFramework.LitPool
{
    /// <summary>
    /// 用于对象的类的回收和反复使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ObjectPool<T> where T : class, IPoolClass, new()
    {
        private static readonly Stack<T> _stack = new Stack<T>();

        public static int CountAll { get; private set; }
        public static int CountActive { get { return CountAll - CountInactive; } }
        public static int CountInactive { get { return _stack.Count; } }

        public static T Get()
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
            element.PrivateInit();
            return element;
        }

        public static void Release(T element)
        {
            if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
            {
                Log.Info("Internal error. Trying to destroy object that is already released to pool.");
                return;
            }
            element.PrivateRelease();
            _stack.Push(element);
        }
    }
}

/// <summary>
/// 使用ObjectPool的类需要手动实现该方法，维护自身的初始化和释放过程
/// </summary>
public interface IPoolClass
{
    void PrivateInit();

    void PrivateRelease();
}
