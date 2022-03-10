/*======================================
* 项目名称 ：LitFrameworkEditor.Editor
* 项目描述 ：
* 类 名 称 ：SceneViewExtention
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor.Editor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/3/30 14:10:44
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

namespace LitFrameworkEditor.Editor
{
    /// <summary>
    /// 强化Scene窗口显示
    /// </summary>
    public class SceneViewExtention:MonoBehaviour
    {
        /// <summary>
        /// 显示选中物体名称
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="gizmoType"></param>
        [DrawGizmo( GizmoType.Selected )]
        static void DrawGameObjectName( Transform transform, GizmoType gizmoType )
        {
            Handles.Label( transform.position, transform.gameObject.name );
        }
    }
}
