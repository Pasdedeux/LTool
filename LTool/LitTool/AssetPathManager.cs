/*======================================
* 项目名称 ：LitFramework.EditorTool
* 项目描述 ：
* 类 名 称 ：AssetPathManager
* 类 描 述 ：
* 命名空间 ：LitFramework.EditorTool
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 14:22:01
* 更新时间 ：2018/5/9 14:22:01
* 版 本 号 ：v1.0.0.0
*******************************************************************
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/9 14:22:01
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LitFramework.LitTool
{
    public class AssetPathManager: Singleton<AssetPathManager>
    {
        public AssetPathManager() { }

        private string _tmpPath;
        /// <summary>
        ///  获取外部persistant路径+"//"
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <param name="useUri">true-用于www/unitywebrequest加载路径,  false-用于FileInfo FileStream</param>
        /// <returns></returns>
        public string GetPersistentDataPath( string filePath, bool useUri = true )
        {
            Uri uri = new Uri( Application.persistentDataPath +"/"+ filePath );
            _tmpPath = useUri ? uri.AbsoluteUri : uri.AbsolutePath;
            return _tmpPath.Replace( "%20", " " );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <param name="useUri">true-用于www/unitywebrequest加载路径,  false-用于FileInfo FileStream</param>
        /// <returns></returns>
        public string GetStreamAssetDataPath(string filePath, bool useUri = true )
        {
            Uri uri = new Uri( Application.streamingAssetsPath +"/"+ filePath );
            _tmpPath = useUri ? uri.AbsoluteUri : uri.AbsolutePath;
            return _tmpPath.Replace( "%20", " " );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <param name="useUri">true-用于www/unitywebrequest加载路径,  false-用于FileInfo FileStream</param>
        /// <returns></returns>
        public string GetTemporaryCachePath( string filePath, bool useUri = true )
        {
            Uri uri = new Uri( Application.temporaryCachePath + "/" + filePath );
            _tmpPath = useUri ? uri.AbsoluteUri : uri.AbsolutePath;
            return _tmpPath.Replace( "%20", " " );
        }
    }
}
