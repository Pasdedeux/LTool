/*======================================
* 项目名称 ：Assets.Scripts.Model.HotFix
* 项目描述 ：
* 类 名 称 ：HotfixLogic
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Model.HotFix
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/12 16:33:00
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using Assets.Scripts.Module.HotFix;
using LitFramework;
using LitFramework.LitTool;
using LitFramework.MsgSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets.Scripts.Module.HotFix
{
    public class HotfixLogic : IHotFix
    {
        //Dll热更工程库
        private const string CONFIG_NAME = "LHotfixProject.dll";
        //Dll热更工程调试库
        private const string CONFIG_NAME_PDB = "LHotfixProject.pdb";

        public void DoFilesMovement()
        {
            //Dll热更工程库
            if ( !DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME, false ) ) )
            {
                DocumentAccessor.LoadAsset( AssetPathManager.Instance.GetStreamAssetDataPath( CONFIG_NAME ), ( UnityWebRequest e ) => DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME, false ), e.downloadHandler.data ) );
            }

            //Dll热更工程调试库
            if ( !DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME_PDB, false ) ) )
            {
                if( DocumentAccessor.IsExists( AssetPathManager.Instance.GetStreamAssetDataPath( CONFIG_NAME_PDB, false ) ) )
                    DocumentAccessor.LoadAsset( AssetPathManager.Instance.GetStreamAssetDataPath( CONFIG_NAME_PDB ), ( UnityWebRequest e ) => DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME_PDB, false ), e.downloadHandler.data ) );
            }
        }

        public IEnumerator DoHotFix()
        {
            //1、下载最新的资源配置信息
            bool canGoFurther = true;
            string localContent = null;

            //================DLL 运行库================//
            string remoteFilePath = CONFIG_NAME;
            //发送下载XX文件事件
            MsgManager.Instance.Broadcast( InternalEvent.HANDLING_REMOTE_RES, new MsgArgs( remoteFilePath, InternalEvent.RemoteStatus.Download ) );

            string localFilePath = AssetPathManager.Instance.GetPersistentDataPath( remoteFilePath, false );
            //2、根据本地是否存在资源配置信息，如果不存在，则视为远程更新流程不执行
            if ( DocumentAccessor.IsExists( localFilePath ) )
            {
                string remoteContent = null;
                byte[] contentByteArr = null;

                //远程主配置文件获取
                yield return DocumentAccessor.ILoadAsset( FrameworkConfig.Instance.RemoteUrlConfig + remoteFilePath, callBack: ( UnityWebRequest e ) =>
                {
                    LDebug.Log( "Remote update..." + remoteFilePath + "读取完成" );
                    remoteContent = e.downloadHandler.text;
                    contentByteArr = e.downloadHandler.data;
                },
                errorCallBack: ( UnityWebRequest e ) =>
                {
                    LDebug.LogError( "Remote Error..." + e + ": " + remoteFilePath );
                    if ( !string.IsNullOrEmpty( e.error ) ) { canGoFurther = false; return; }
                } );

                //因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                if ( !canGoFurther )
                {
                    MsgManager.Instance.Broadcast( InternalEvent.REMOTE_UPDATE_ERROR, new MsgArgs( remoteContent, remoteFilePath ) );
                    yield break;
                }

                //本地主配置文件获取
                DocumentAccessor.LoadAsset( localFilePath, ( string e ) => { localContent = e; } );

                //更新文档
                DocumentAccessor.SaveAsset2LocalFile( localFilePath, contentByteArr );
            }


            //================DLL 调试库================//
            if ( !FrameworkConfig.Instance.showLog ) yield break;

            remoteFilePath = CONFIG_NAME_PDB;
            localFilePath = AssetPathManager.Instance.GetPersistentDataPath( remoteFilePath, false );
            //2、根据本地是否存在资源配置信息，如果不存在，则视为远程更新流程不执行
            if ( DocumentAccessor.IsExists( localFilePath ) )
            {
                string remoteContent = null;
                byte[] contentByteArr = null;

                //远程主配置文件获取
                yield return DocumentAccessor.ILoadAsset( FrameworkConfig.Instance.RemoteUrlConfig + remoteFilePath, callBack: ( UnityWebRequest e ) =>
                {
                    LDebug.Log( "Remote update..." + remoteFilePath + "读取完成" );
                    remoteContent = e.downloadHandler.text;
                    contentByteArr = e.downloadHandler.data;
                },
                errorCallBack: ( UnityWebRequest e ) =>
                {
                    LDebug.LogError( "Remote Error..." + e + ": "+ remoteFilePath );
                    if ( !string.IsNullOrEmpty( e.error ) ) { canGoFurther = false; return; }
                } );

                //因为加载出问题导致无法继续时，目前先使用中断后续步骤，并弹窗提醒的方式搞
                if ( !canGoFurther )
                {
                    MsgManager.Instance.Broadcast( InternalEvent.REMOTE_UPDATE_ERROR, new MsgArgs( remoteContent, remoteFilePath ) );
                    yield break;
                }

                //本地主配置文件获取
                DocumentAccessor.LoadAsset( localFilePath, ( string e ) => { localContent = e; } );

                //更新文档
                DocumentAccessor.SaveAsset2LocalFile( localFilePath, contentByteArr );
            }
        }
    }
}
