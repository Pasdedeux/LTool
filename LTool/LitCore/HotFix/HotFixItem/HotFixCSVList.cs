/*======================================
* 项目名称 ：Assets.Scripts.Module.HotFix
* 项目描述 ：
* 类 名 称 ：HotFixCSVList
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Module.HotFix
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/8/2 15:21:27
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets.Scripts.Module.HotFix
{
    /// <summary>
    /// 配置档热更
    /// </summary>
    public class HotFixCSVList : IHotFix
    {
        //游戏配置档
        private const string CONFIG_NAME = "csvList.txt";

        public void DoFilesMovement()
        {
            //配置档总表
            if ( !DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME, false ) ) )
            {
                DocumentAccessor.LoadAsset( AssetPathManager.Instance.GetStreamAssetDataPath( CONFIG_NAME ), ( UnityWebRequest e ) => DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME, false ), e.downloadHandler.data ) );

                //顺次加载各类配置表
                string[] csvKeys = null;
                string localPath = AssetPathManager.Instance.GetPersistentDataPath( CONFIG_NAME );
                DocumentAccessor.LoadAsset( localPath, ( string e ) =>
                csvKeys = e.Split( new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries ) );

                for ( int i = 1; i < csvKeys.Length; i++ )
                {
                    string item = csvKeys[i].Split( ',' )[ 0 ];
                    if ( !DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( item, false ) ) )
                    {
                        DocumentAccessor.LoadAsset( AssetPathManager.Instance.GetStreamAssetDataPath( item ), ( UnityWebRequest e ) =>
                        {
                            DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( item, false ), e.downloadHandler.data );
                        } );
                    }
                }
            }
        }

        public IEnumerator DoHotFix()
        {
            LDebug.Log( "开始检测更新：" + CONFIG_NAME );
            //1、下载最新的资源配置信息
            bool canGoFurther = true;
            string localContent = null;
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


                ////本地配置表默认全更新。
                //string[] str = remoteContent.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                //string[] localFileContent = localContent.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                //var toDelete = localFileContent.Where( a => !str.Contains( a ) );

                //foreach ( var item in toDelete )
                //{
                //    FileInfo fileInfo = new FileInfo( AssetPathManager.Instance.GetPersistentDataPath( item, false ) );
                //    if ( fileInfo.Exists ) fileInfo.Delete();
                //    LDebug.Log( ">>>Delete " + item, LogColor.red );
                //    LDebug.Log( ">>>Delete Result " + DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( item, false ) ), LogColor.red );
                //}

                //for ( int w = 0; w < str.Length; w++ )
                //{
                //    LDebug.Log( "Remote update..." + str[ w ] + "开始读取" );
                //    yield return DocumentAccessor.ILoadAsset( FrameworkConfig.Instance.RemoteUrlConfig + str[ w ], ( UnityWebRequest e ) =>
                //    {
                //        LDebug.Log( "Remote update..." + str[ w ] + "读取完成", LogColor.yellow );
                //        DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( str[ w ], false ), e.downloadHandler.data );
                //    } );
                //}

                ////更新文档
                //DocumentAccessor.SaveAsset2LocalFile( localFilePath, contentByteArr );
                //LDebug.Log( "检测更新完成：" + CONFIG_NAME );

                //本地配置表默认增量更新。修改为增量更新后，后续的逻辑跟HOTFIXAB是一样的
                Dictionary<string, ABVersion> remoteABVersionsDic = ResolveABContent( remoteContent );
                Dictionary<string, ABVersion> localABVersionsDic = ResolveABContent( localContent );

                //需要删除的对象
                var toDelete = localABVersionsDic.Where( a => !remoteABVersionsDic.ContainsKey( a.Key ) );
                foreach ( var item in toDelete )
                {
                    FileInfo fileInfo = new FileInfo( AssetPathManager.Instance.GetPersistentDataPath( item.Key, false ) );
                    if ( fileInfo.Exists ) fileInfo.Delete();
                    LDebug.Log( ">>>Delete " + item.Key, LogColor.red );
                    LDebug.Log( ">>>Delete Result " + DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( item.Key, false ) ), LogColor.red );
                }

                //需要更新的对象：可以根据需求拓展对version的使用规则。
                //默认是更新版本号更高或者新增加的对象。
                var toUpdate = remoteABVersionsDic.Where( a => !localABVersionsDic.ContainsKey( a.Key ) || a.Value.Version > localABVersionsDic[ a.Key ].Version );
                foreach ( var item in toUpdate )
                {
                    LDebug.Log( "Remote update..." + FrameworkConfig.Instance.RemoteUrlConfig + "/" + item.Key + " 开始读取" );
                    yield return DocumentAccessor.ILoadAsset( FrameworkConfig.Instance.RemoteUrlConfig + "/" + item.Key, ( UnityWebRequest e ) =>
                    {
                        LDebug.Log( "Remote update..." + item.Key + "读取完成", LogColor.yellow );
                        DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( item.Key, false ), e.downloadHandler.data );
                    } );
                }

                //更新文档
                DocumentAccessor.SaveAsset2LocalFile( localFilePath, contentByteArr );
                LDebug.Log( "检测更新完成：" + CONFIG_NAME );
            }
        }

        //解析ABVersion配置表
        private Dictionary<string, ABVersion> ResolveABContent( string contentResolve )
        {
            Dictionary<string, ABVersion> resultDict = new Dictionary<string, ABVersion>();

            string[] str = contentResolve.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
            int toLoadNum = str.Length;

            for ( int k = 1; k < str.Length; k++ )
            {
                string line = str[ k ];
                if ( line != "" )
                {
                    string[] content = line.Split( ',' );
                    ABVersion ab = new ABVersion
                    {
                        AbName = content[ 0 ],
                        Version = int.Parse( content[ 1 ] ),
                        MD5 = content[ 2 ].Trim()
                    };
                    resultDict.Add( content[ 0 ], ab );
                }
            }

            return resultDict;
        }
    }
}
