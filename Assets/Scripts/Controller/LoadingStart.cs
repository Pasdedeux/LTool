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
    void Start()
    {
        //是否迁移到可读写目录
        DocumentAccessor.MoveStreamPath2PersistantPath();

        FrameworkController.Instance.InitLoadingLogo();
        FrameworkController.Instance.InitFramework();

        LDebug.Log( ">>" );
    }   
}
