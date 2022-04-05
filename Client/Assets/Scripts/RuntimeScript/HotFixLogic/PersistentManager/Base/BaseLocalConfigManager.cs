using ILRBaseModel.Base;
using LitFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLocalConfigManager<T> : IManager where T : BaseLocalConfigManager<T>, new()
{
    /// <summary>
    /// 初始化
    /// </summary>
    public abstract void Install();
    /// <summary>
    /// 首次登录设置
    /// </summary>
    public virtual void FirstIniteData()
    {

    }

    /// <summary>
    /// 卸载
    /// </summary>
    public virtual void Uninstall()
    {
        if (_instance != null)
            _instance = null;
    }

    /// <summary>
    /// volatile多用于多线程的环境，当一个变量定义为volatile时，读取这个变量的值时候每次都是从momery里面读取而不是从cache读。这样做是为了保证读取该变量的信息都是最新的，而无论其他线程如何更新这个变量。
    /// </summary>
    private static volatile T _instance;
    //线程锁，通常不需要
    private static object _lock = new object();

    //主题单例
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new T();
                    DataManager.Instance.DestroyPayerData += _instance.Uninstall;
                }
            }
            return _instance;
        }
    }
}

