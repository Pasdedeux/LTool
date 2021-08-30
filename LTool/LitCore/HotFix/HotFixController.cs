/*======================================
* 项目名称 ：Assets.Scripts.Module.HotFix
* 项目描述 ：
* 类 名 称 ：HotFixRes
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Module.HotFix
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/2 15:12:43
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.LitTool;
using LitFramework.MsgSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Module.HotFix
{
    /// <summary>
    /// 热更模块
    /// 
    /// 完成美术资源、配置档及代码迁移+热更流程
    /// 
    /// 热更流程说明：<see href="https://www.processon.com/view/link/61013cd4637689719d2d8166">流程图</see>
    /// </summary>
    public class HotFixController : Singleton<HotFixController>
    {
        /// <summary>
        /// 执行文件迁移+文件热更
        /// </summary>
        /// <param name="hotFixesQueue"></param>
        public void Excecute( Queue<IHotFix> hotFixesQueue )
        {
            //是否使用可读写目录
            if ( FrameworkConfig.Instance.UsePersistantPath )
            {
                foreach ( var item in hotFixesQueue )
                    item.DoFilesMovement();

                StartHotFix( hotFixesQueue );
            }
            else MsgManager.Instance.Broadcast(InternalEvent.END_LOAD_REMOTE_CONFIG );
        }

        private void StartHotFix( Queue<IHotFix> hotFixesQueue )
        {
            //是否需要热更
            if ( FrameworkConfig.Instance.UseRemotePersistantPath )
            {
                if ( string.IsNullOrEmpty( FrameworkConfig.Instance.RemoteUrlConfig ) )
                {
                    LDebug.LogErrorFormat( ">>REMOTE_IP： {0} 无效!", FrameworkConfig.Instance.RemoteUrlConfig );
                    return;
                }
                LitTool.MonoBehaviour.StartCoroutine( IStartHotFix( hotFixesQueue ) );
            }
            else MsgManager.Instance.Broadcast( InternalEvent.END_LOAD_REMOTE_CONFIG );
        }

        private IEnumerator IStartHotFix( Queue<IHotFix> hotFixList )
        {
            MsgManager.Instance.Broadcast( InternalEvent.START_LOAD_REMOTE_CONFIG );

            do
            {
                IHotFix toHotFix = hotFixList.Dequeue();
                yield return toHotFix.DoHotFix();
            } while ( hotFixList.Count > 0 );

            MsgManager.Instance.Broadcast( InternalEvent.END_LOAD_REMOTE_CONFIG );
        }

    }

    /// <summary>
    /// csv资源文件内容
    /// </summary>
    public class ABVersion
    {
        public string AbName;
        public int Version;
        public string MD5;
    }
}
