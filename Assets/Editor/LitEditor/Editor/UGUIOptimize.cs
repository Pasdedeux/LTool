/*======================================
* 项目名称 ：LitFrameworkEditor.Editor
* 项目描述 ：
* 类 名 称 ：UGUIOptimize
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor.Editor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/29 17:18:22
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LitFrameworkEditor.Editor
{
    /// <summary>
    /// UGUI 具体修改策略，可通过委托扩展和修改
    /// </summary>
    public static class UGUIOptimizeStrategy
    {
        /// <summary>
        /// <para>Image 对象创建时回调处理委托，可扩展或重写</para>
        /// <para>默认修改属性为：image.raycastTarget = false</para>
        /// <para>扩展方式为：CustomComponentOptimizing.imageModFunc += ( e ) => { ( e as Image ).raycastTarget = true; };</para>
        /// </summary>
        public static Action<Image> imageModFunc = ( e ) => { e.raycastTarget = false; };
        /// <summary>
        /// <para>Text 对象创建时回调处理委托，可扩展或重写</para>
        /// <para>默认修改属性为： 
        /// Text.supportRichText = false;
        /// Text.raycastTarget = false;</para>
        /// <para>扩展方式为：CustomComponentOptimizing.txtModFunc += ( e ) => { ( e as Text ).supportRichText = true; };</para>
        /// </summary>
        public static Action<Text> txtModFunc = ( e ) => { e.supportRichText = false; e.raycastTarget = false; };

        /// <summary>
        /// 创建相机时回调
        /// </summary>
        public static Action<Camera> camModeFunc = null;

        /// <summary>
        /// 修改UGUI Image组件设置
        /// </summary>
        /// <param name="image"></param>
        internal static void ModifyImage( Image image )
        {
            imageModFunc?.Invoke( image );
        }

        /// <summary>
        /// 修改UGUI Text组件设置
        /// </summary>
        /// <param name="text"></param>
        internal static void ModifyText( Text text )
        {
            txtModFunc?.Invoke( text );
        }

        /// <summary>
        /// 修改 Camera 组件
        /// </summary>
        /// <param name="cam"></param>
        internal static void ModifyCame( Camera cam )
        {
            camModeFunc?.Invoke( cam );
        }
    }

    /// <summary>
    /// UGUI 外部调用指定修改UGUI路径
    /// </summary>
    public static class UGUIOptimizeCommand
    {
        /// <summary>
        /// UGUI-Text
        /// </summary>
        /// <param name="menuCommand"></param>
        public static void CreatText( MenuCommand menuCommand )
        {
            var text = UGUIOptTool.CreatCustomUGUI<Text>();
            if ( LitFramework.FrameworkConfig.Instance.UGUIOpt )
                UGUIOptimizeStrategy.ModifyText( text );
        }
        /// <summary>
        /// UGUI-Image
        /// </summary>
        /// <param name="menuCommand"></param>
        public static void CreatImage( MenuCommand menuCommand )
        {
            var image = UGUIOptTool.CreatCustomUGUI<Image>();
            if ( LitFramework.FrameworkConfig.Instance.UGUIOpt )
                UGUIOptimizeStrategy.ModifyImage( image );
        }
        /// <summary>
        /// 创建相机
        /// </summary>
        /// <param name="munuCommand"></param>
        public static void CreateCamera( MenuCommand menuCommand )
        {
            var cam = UGUIOptTool.CreatCustomUGUI<Camera>();
            UGUIOptimizeStrategy.ModifyCame( cam );
        }
    }

    /// <summary>
    /// UGUI工具基本工具库，创建对象
    /// </summary>
    internal static class UGUIOptTool
    {
        /// <summary>
        /// 自定义生成对应组件
        /// </summary>
        /// <typeparam name="T">Image/Text等组件</typeparam>
        /// <returns></returns>
        internal static T CreatCustomUGUI<T>() where T : Component
        {
            string typeName = typeof( T ).Name;

            GameObject newType = new GameObject( typeName );
            GameObject go = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign( newType, go );

            var addComnent = newType.AddComponent<T>();
            return addComnent;
        }
    }

}
