#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitPool
* 项目描述 ：
* 类 名 称 ：BasePool
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitPool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/6 22:27:02
* 更新时间 ：2018/5/6 22:27:02
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework.LitPool
{
    /// <summary>
    /// 对象池基类
    /// </summary>
    /// <typeparam name="UnitType"></typeparam>
    /// <typeparam name="PoolUnitGroup"></typeparam>
    public abstract class BasePool<UnitType, PoolUnitGroup> : UnityEngine.MonoBehaviour  where UnitType : class , IPoolUnit where PoolUnitGroup : BasePoolUnitGroup<UnitType>, new()
    {
        //缓存池，按类型存放各自分类列表
        private Dictionary<Type , PoolUnitGroup> _allUnitPool = new Dictionary<Type , PoolUnitGroup>();
        
        /// <summary>
        /// 获取一个指定类型空闲单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Take<T>() where T : class, UnitType
        {
            PoolUnitGroup group = GetGroup<T>();
            return group.TakeUnit<T>() as T;
        }

        /// <summary>
        /// 获取或者新创建指定类型的池
        /// </summary>
        /// <typeparam name="T">对象单元类型</typeparam>
        /// <returns></returns>
        private PoolUnitGroup GetGroup<T>() where T : UnitType
        {
            var t = typeof( T );
            PoolUnitGroup group = null;

            _allUnitPool.TryGetValue( t , out group );
            if(group == null)
            {
                group = CreateUnitGroup<T>();
                _allUnitPool.Add( t , group );
            }
            return group;
        }

        /// <summary>
        /// 将创建对象池的方法交由子类具体实现
        /// </summary>
        /// <typeparam name="UT"></typeparam>
        /// <returns></returns>
        protected abstract PoolUnitGroup CreateUnitGroup<UT>() where UT : UnitType;
    }
}
