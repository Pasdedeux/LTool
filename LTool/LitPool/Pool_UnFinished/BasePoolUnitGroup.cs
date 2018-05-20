#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitPool
* 项目描述 ：
* 类 名 称 ：PoolUnitGroup
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitPool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/6 21:34:42
* 更新时间 ：2018/5/6 21:34:42
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
    /// 一组特定对象的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePoolUnitGroup<T> where T : class, IPoolUnit
    {
        protected object template;
        protected Stack<T> idleList;
        protected List<T> workList;
        protected int createdNum = 0;

        public BasePoolUnitGroup()
        {
            idleList = new Stack<T>();
            workList = new List<T>();
        }

        /// <summary>
        /// 获取一个实例对象
        /// </summary>
        /// <typeparam name="UT"></typeparam>
        /// <returns></returns>
        public virtual T TakeUnit<UT>() where UT : T
        {
            T unit;
            if( idleList.Count > 0 )
                unit = idleList.Pop();
            else
            {
                unit = CreateUnit<UT>();
                unit.SetParentList( this );
                createdNum++;
            }
            workList.Add( unit );
            unit.State = PoolType.Work;

            OnUnitChangePool( unit );
            return unit;
        }

        /// <summary>
        /// 放回对象到对应池
        /// </summary>
        /// <param name="unit"></param>
        public virtual void RestoreUnit( T unit )
        {
            if( unit != null && unit.State == PoolType.Work )
            {
                workList.Remove( unit );
                idleList.Push( unit );
                unit.State = PoolType.Idle;

                OnUnitChangePool( unit );
            }
        }

        /// <summary>
        /// 设置模板
        /// </summary>
        /// <param name="template"></param>
        public void SetTemplate( object template )
        {
            this.template = template;
        }
        protected abstract void OnUnitChangePool( T unit );
        protected abstract T CreateUnit<UT>() where UT : T;
    }
}
