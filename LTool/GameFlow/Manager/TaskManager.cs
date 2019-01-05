#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.GameFlow.Manager
* 项目描述 ：
* 类 名 称 ：TaskManager
* 类 描 述 ：
* 所在的域 ：DEREK-SURFACE
* 命名空间 ：LitFramework.GameFlow.Manager
* 机器名称 ：DEREK-SURFACE 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：lc1027
* 创建时间 ：2018/12/11 21:10:45
* 更新时间 ：2018/12/11 21:10:45
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ lc1027 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion
using LitFramework.UI.Base;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace LitFramework.GameFlow.Manager
{
    /// <summary>
    /// 延时调用方法与协程池
    /// </summary>
    public class TaskManager : Singleton<TaskManager>, IManager
    {
        private static UnityEngine.GameObject _coroutingGameObject = null;
        private static UnityEngine.MonoBehaviour _coroutingMono = null;
        public void Install()
        {
            if ( !_coroutingGameObject )
            {
                _coroutingGameObject = new UnityEngine.GameObject( "TaskManager" );
                _coroutingMono = _coroutingGameObject.AddComponent<CoroutingClass>();
            }


        }

        public void Uninstall()
        {




            if ( _coroutingMono )
            {
                _coroutingMono.StopAllCoroutines();
                _coroutingMono = null;
            }

            if ( _coroutingGameObject )
            {
                UnityEngine.GameObject.Destroy( _coroutingGameObject );
                _coroutingGameObject = null;
            }
        }


        #region 延迟调用方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">时间-秒</param>
        /// <param name="func">回调函数</param>
        public static void DelayPlayFunction( float time, System.Action func )
        {
            //24B
            _coroutingMono.StartCoroutine( DelayFunction( time, func ) );
        }
        private static IEnumerator DelayFunction( float time, System.Action func )
        {
            yield return new UnityEngine.WaitForSeconds( time );
            func?.Invoke();
        }
        public static void StopDelayPlayFunction()
        {
            _coroutingMono.StopCoroutine( "DelayFunction" );
        }
        #endregion

    }

    /// <summary>
    /// 协程器
    /// </summary>
    class CoroutingClass : UnityEngine.MonoBehaviour { }
}
