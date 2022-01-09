using FairyGUI;
using LitFramework;
using LitFramework.LitTool;
using LitFramework.HotFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LitFramework.LitPool;

namespace LHotfixProject
{
    public class HotfixEntry
    {
        /// <summary>
        /// 热更预处理接口。
        /// 可以为空。
        /// 用于启动一些提前自定义任务，例如前期的初始化内容
        /// </summary>
        public static void Pre_RunGame_ILRuntime()
        {
            LDebug.Log( ">>>>执行预加载或初始化", LogColor.yellow );
            
        }

        /// <summary>
        /// 热更项目启动入口
        /// </summary>
        public static void RunGame_ILRuntime()
        {
            //记得完成相关CLR绑定以及适配器制作
            LDebug.Log( string.Format( "《《《 --- {0} version v1.0.0 --- 》》》", FrameworkConfig.Instance.scriptEnvironment == RunEnvironment.DotNet ? "DotNet" : "HotFix" ), LogColor.yellow );
        }
    }
}
