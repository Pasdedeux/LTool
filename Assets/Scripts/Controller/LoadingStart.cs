using Assets.Scripts.Controller;
using LitFramework;
using LitFramework.GameFlow;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.InputSystem;
using LitFramework.LitPool;
using LitFramework.LitTool;
using LitFramework.Mono;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadingStart : MonoBehaviour
{
    /// <summary>
    /// 框架启动核心方法：FrameworkController.Instance.InitFramework()
    /// 如果需要有其它启动流程，至少需要调用这个方法完成初始化
    /// </summary>
    void Start()
    {
        //是否迁移到可读写目录
        DocumentAccessor.MoveStreamPath2PersistantPath();
        
        //TODO 指定地址下载指定文件，并规定解析及覆写规则
        //..

        //启动框架本身
        FrameworkController.Instance.InitFramework();

        LDebug.Log( ">>Loading Start Initialized !" );
    }   
}
