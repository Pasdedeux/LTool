/*======================================
* 项目名称 ：Assets.Scripts.ILRuntime
* 项目描述 ：
* 类 名 称 ：DotNetScriptCall
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.ILRuntime
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/16 17:07:46
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.HotFix;
using LitFramework.LitTool;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Assets.Scripts
{
    public class DotNetScriptCall : IScriptCall
    {
        private bool _isPreInit = false;
        public void PreStartRun()
        {
            Type t = Type.GetType(string.Format("{0}.{1}", ILRScriptCall.HOTFIX_NAMESPACE, ILRScriptCall.HOTFIX_CLASS));
            MethodInfo method = t.GetMethod(ILRScriptCall.HOTFIX_PRE_FUNCNAME);
            method.Invoke(null, null);

            _isPreInit = true;
        }

        public void StartRun()
        {
            if (!_isPreInit) PreStartRun();

            Type t = Type.GetType(string.Format("{0}.{1}", ILRScriptCall.HOTFIX_NAMESPACE, ILRScriptCall.HOTFIX_CLASS));
            MethodInfo method = t.GetMethod(ILRScriptCall.HOTFIX_FUNCNAME);
            method.Invoke(null, null);
        }


        #region 静态外调方法

        //================静态外调方法===============//
        public static void SetConfigInstall(string className, string methodName, string e)
        {
            Type t = Type.GetType(className);
            MethodInfo method = t.GetMethod(methodName);

            var mainClass = Type.GetType("Configs", true);
            var props = mainClass.GetField(className + "Dict");
            var finalType = Convert.ChangeType(method.Invoke(null, new object[1] { e }), props.FieldType);
            props.SetValue(props, finalType);
        }

        public static void SetConfigInstall2()
        {
            var mainClass = Type.GetType("Configs", true);
            var method = mainClass.GetMethod("Install");
            method.Invoke(null, null);
        }


        public static void SetSpawnPool(SpawnPool sp)
        {
            var mainClass = Type.GetType("SpawnPoolReflection", true);
            var method = mainClass.GetMethod("SpawnReflection");
            method.Invoke(null, new object[] { sp });
        }

        #endregion


        #region 业务逻辑

        /// <summary>
        /// 用于将Resource下的ScriptableObject
        /// </summary>
        public static void SetCommonData(UnityEngine.Object content)
        {
            //例如
            ////CommonData Scriptable
            //var mainClass = content.GetType();// Type.GetType( "CommonDataInfo", true );
            //var props = mainClass.GetFields();

            ////Local Container
            //var mainClassContainer = Type.GetType("CommonDataInfo", true);
            //var propsContainer = mainClassContainer.GetFields();

            ////查找匹配
            //bool isMatch = false;
            //for (int i = 0; i < props.Length; i++)
            //{
            //    isMatch = false;
            //    FieldInfo fieldInfo = props[i];
            //    for (int x = 0; x < propsContainer.Length; x++)
            //    {
            //        if (propsContainer[x].Name == fieldInfo.Name)
            //        {
            //            propsContainer[x].SetValue(null, fieldInfo.GetValue(content));
            //            isMatch = true;
            //            break;
            //        }
            //    }
            //    if (!isMatch)
            //        throw new Exception(string.Format("CommonData字段  {0}  在承接类CommonDataInfo中未找到对应字段", fieldInfo.Name));
            //}
        }


        #endregion
    }
}
