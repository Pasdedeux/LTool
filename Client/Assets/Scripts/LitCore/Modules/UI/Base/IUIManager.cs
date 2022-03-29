#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.UI.Base
* 项目描述 ：
* 类 名 称 ：IUIManager
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.UI.Base
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/8/26 16:24:38
* 更新时间 ：2018/8/26 16:24:38
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.UI.Base
{
    public interface IUIManager
    {
        void ShowFade( Action callBack = null, float time = 0.4f );
        void HideFade( Action callBack = null , float time = 0.4f );
        void Close( string uiName, bool isDestroy = false, bool useAnim = true , bool force = false , Action exitCallBack  = null );
        IBaseUI Show( string uiName , params object[] args);
    }

    //UI基础接口，暴露一系列访问方式
    public interface IBaseUI
    {
        #region 状态

        /// <summary>
        /// 该窗口是否开启中
        /// </summary>
        bool IsShowing { get; set; }

        /// <summary>
        /// 当前窗口类型
        /// </summary>
        UIType CurrentUIType { get; set; }

        /// <summary>
        /// 是否执行过Start
        /// </summary>
        bool IsStarted { get; set; }

        /// <summary>
        /// 附加特性
        /// </summary>
        UIFlag Flag { get; set; }

        /// <summary>
        /// 是否使用低帧率
        /// </summary>
        bool UseLowFrame { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        string AssetsName { get; set; }

        /// <summary>
        /// 创建完毕标记，用于控制UI预制件在第一次创建出来时，不要自动触发OnEnable
        /// </summary>
        bool IsInitOver { get; set; }

        #endregion

        #region 节点

        /// <summary>
        /// 关联的UI实例
        /// </summary>
        GameObject GameObjectInstance { get; set; }

        /// <summary>
        /// 动画列表
        /// </summary>
        DOTweenAnimation[] DotAnims { get; set; }

        /// <summary>
        /// 根节点
        /// </summary>
        Transform RootTrans { get; set; }

        /// <summary>
        /// 动画节点
        /// </summary>
        Transform RootAniTrans { get; set; }

        /// <summary>
        /// 根 Canvas
        /// </summary>
        Canvas RootCanvas { get; set; }

        #endregion

        #region 调用方法

        /// <summary>
        /// 启动方法
        /// </summary>
        void CallCtor();

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="replay">会传bool到 OnEnable/OnDisable</param>
        /// <param name="args">通过Show的传参</param>
        void Show(bool replay = false, params object[] args);

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="isDestroy">是否摧毁并彻底释放</param>
        /// <param name="freeze">是否暂时冻结，会传bool到 OnEnable/OnDisable</param>
        void Close(bool isDestroy = false, bool freeze = false);

        /// <summary>
        /// 检测并显示模态窗体背景
        /// </summary>
        void CheckMask();

        void OnClose();

        ///<inheritdoc/>
        /// <remarks>
        /// 刷新窗体
        /// </remarks>
        //virtual void OnShow();

        /// <summary>
        /// 刷新窗体，带参数
        /// </summary>
        /// <param name="args"></param>
        void OnShow(params object[] args);

        void Dispose();

        void OnAdapter();

        /// <summary>
        /// 点击返回事件
        /// </summary>
        void OnBackPushed();

        #region Alternative Function

        void FindMember();

        void OnAwake();

        void OnEnabled(bool replay);

        void OnDisabled(bool freeze);

        void OnStart();

        void OnUpdate();

        void Initialize();

        #endregion

        #endregion

    }
}
