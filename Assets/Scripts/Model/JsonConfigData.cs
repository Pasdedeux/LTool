/*======================================
* 项目名称 ：Assets.Scripts.Model
* 项目描述 ：
* 类 名 称 ：JsonConfigData
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Model
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/4 13:08:30
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using LitFramework;
using LitFramework.GameFlow.Model.DataLoadInterface;
using LitFramework.LitTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class JsonConfigData : BaseLocalData
{
    public JsonConfigData( LocalDataManager mng ) : base( mng ) { }

    public override void Load()
    {
        ////TODO 读取多个JSON文件并保存指定访问位置
        //string localPath = FrameworkConfig.Instance.UsePersistantPath ? AssetPathManager.Instance.GetPersistentDataPath( "XX.json" ) : AssetPathManager.Instance.GetStreamAssetDataPath( "XX.json" );
        //DocumentAccessor.LoadAsset( localPath, ( string e ) => 
        //{
        //    var loadedJson = LitJson.JsonMapper.ToObject<自定义的JSON类名>( e );
        //} );
    }
}
