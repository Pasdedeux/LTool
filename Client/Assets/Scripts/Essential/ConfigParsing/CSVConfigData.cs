/*======================================
* 项目名称 ：Assets.Scripts.Model
* 项目描述 ：
* 类 名 称 ：CSVConfigData
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Model
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/2 10:27:33
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// CSV配置表读取，默认加载txt表格内记录文件。
/// 配置表文档作为基础信息，务必优先加载
/// </summary>
public class CSVConfigData : BaseLocalData
{
    public CSVConfigData( LocalDataManager mng ) : base( mng ) { }

    /// 加载配置
    /// </summary>
    public override void Load()
    {
        //顺次加载各类配置表
        List<string> csvKeys = null;
        string localPath = FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath( "csvList.txt") : AssetPathManager.Instance.GetStreamAssetDataPath( "csvList.txt");

        DocumentAccessor.LoadAsset( localPath, ( string e ) =>
        csvKeys = e.Split( new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries ).ToList() );

        for ( int i = 1; i < csvKeys.Count; i++ )
        {
            string item = csvKeys[ i ].Split( ',' )[ 0 ];
            if ( !item.EndsWith( ".csv" ) ) continue;

            localPath = FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath( item ) : AssetPathManager.Instance.GetStreamAssetDataPath( item );
            DocumentAccessor.LoadAsset( localPath, ( string e ) =>
              {
                  var contens = item.Split( '/' );
                  string className = ( contens.Length > 1 ? contens[ 1 ] : contens[ 0 ] ).Split( '.' )[ 0 ];
                  string strMethod = "ReturnDictionary";

                  //正常情况下，通过域内反射获取到对应类及静态方法
                  if ( FrameworkConfig.Instance.scriptEnvironment != RunEnvironment.ILRuntime )
                      Assets.Scripts.DotNetScriptCall.SetConfigInstall( className, strMethod, e );
                  //热更情况下，通过热更接口获取到内部配置档类及方法
                  else
                      Assets.Scripts.ILRScriptCall.SetConfigInstall( className, strMethod, e );

                  LDebug.Log( string.Format( "配置档解析完成-> {0}", item ) );
              } );
        }

        if ( FrameworkConfig.Instance.scriptEnvironment != RunEnvironment.ILRuntime )
            Assets.Scripts.DotNetScriptCall.SetConfigInstall2();
        else
            Assets.Scripts.ILRScriptCall.SetConfigInstall2();
    }
}
