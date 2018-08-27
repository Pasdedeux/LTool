#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameUtility
* 项目描述 ：
* 类 名 称 ：LitTool
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.GameUtility
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/26 17:04:10
* 更新时间 ：2018/8/26 17:04:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.GameUtility
{
    public class LitTool
    {
        public static MonoBehaviour monoBehaviour;
        private static WaitForSeconds _waitSeconds;
        private static WaitUntil _waitUntil;
        #region 延迟调用方法
        public static void DelayPlayFunction( float time, System.Action func )
        {
            if ( monoBehaviour == null )
            {
                GameObject go = new GameObject( "Monobehavior" );
                monoBehaviour = go.AddComponent<MonoForCorouting>();
            }
            _waitSeconds = new WaitForSeconds( time );
            monoBehaviour.StartCoroutine( DelayFunction( time, func ) );
        }
        static IEnumerator DelayFunction( float time, System.Action func )
        {
            yield return _waitSeconds;
            func?.Invoke();
        }

        public static void StopDelayPlayFunction()
        {
            monoBehaviour.StopCoroutine( "DelayFunction" );
        }


        public static void WaitUntilFunction( Func<bool> conditionFunc, System.Action func )
        {
            if ( monoBehaviour == null )
            {
                GameObject go = new GameObject( "Monobehavior" );
                monoBehaviour = go.AddComponent<MonoForCorouting>();
            }
            _waitUntil = new WaitUntil( conditionFunc );
            monoBehaviour.StartCoroutine( WaitUntilFunction( func ) );
        }

        static IEnumerator WaitUntilFunction( Action func )
        {
            yield return _waitUntil;
            func?.Invoke();
        }
        #endregion
    }


    public class MonoForCorouting : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad( this );
        }
    }
}
