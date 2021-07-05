using LitFramework;
using LitFramework.GameFlow;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.InputSystem;
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
        if ( FrameworkConfig.Instance.UsePersistantPath )
            DocumentAccessor.MoveStreamPath2PersistantPath();

        //框架启动：直接启动，或者使用LoadingTaskModel登记启动时机
        LitFrameworkFacade.Instance.StartUp( beforeExecuteFunc: ()=> 
        {
            LocalDataManager.Instance.InstallEventHandler += e =>
            {
                //顺次加载本地配置表、存储的JSON数据
                new CSVConfigData( e );
                new JsonConfigData( e );
            };
        },
        afterExecuteFunc: () =>
        {
            //TODO 项目当中需要切换场景时，可以使用这个方法，具体功能可以参考参数说明
            GameFlowController.Instance.ChangeScene( 1, loadingUIPath: ResPath.UI.UILOADING, needFading: true );
        }
        );
        LDebug.Log( ">>" );
    }   
}
