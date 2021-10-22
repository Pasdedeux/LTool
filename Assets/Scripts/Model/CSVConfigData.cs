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
        //TODO 这里根据需要，可能修改为读写目录获取，并在此之前执行文档拷贝、远程地址解析、远程文件下载并覆盖操作，再通过读写地址获取指定文档
        //顺次加载各类配置表
        List<string> csvKeys = null;
        string localPath = FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath( "csvList.txt" ) : AssetPathManager.Instance.GetStreamAssetDataPath( "csvList.txt" );
        
        DocumentAccessor.LoadAsset( localPath, ( string e ) =>
        csvKeys = e.Split( new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries ).ToList() );

        foreach ( var item in csvKeys )
        {
            if ( !item.EndsWith( ".csv" ) ) continue;

            localPath = FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath( item ) : AssetPathManager.Instance.GetStreamAssetDataPath( item );
            DocumentAccessor.LoadAsset( localPath, ( string e ) =>
              {
                  var contens = item.Split( '/' );
                  string className = ( contens.Length > 1 ? contens[ 1 ] : contens[ 0 ] ).Split( '.' )[ 0 ];
                  string strMethod = "ReturnDictionary";
                  Type t = Type.GetType( className );
                  MethodInfo method = t.GetMethod( strMethod );//通过string类型的strMethod获得同名的方法“method”

                  var mainClass = Type.GetType( "Configs", true );
                  var props = mainClass.GetField( className + "Dict" );
                  var finalType = Convert.ChangeType( method.Invoke( null, new object[ 1 ] { e } ), props.FieldType );
                  props.SetValue( props, finalType );

                  LDebug.Log( string.Format( "配置档解析完成-> {0}", item ) );
              } );
        }
    }
}
