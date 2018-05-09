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
* Copyright @ ShengYanTech 2018. All rights reserved.
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

namespace LitFramework.EditorTool
{
    class AssetPathManager: Singleton<AssetPathManager>
    {
        private StringBuilder _sBuilder = new StringBuilder();

        /// <summary>
        ///  获取外部persistant路径+"//"
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <param name="useWWW">是否采用www加载</param>
        /// <param name="useFile">是否采用FileInfo类加载</param>
        /// <returns></returns>
        public string GetPersistentDataPath( string filePath , bool useWWW = false, bool useFile = false )
        {
            _sBuilder.Remove( 0 , _sBuilder.Length );

            if( useWWW )
            {
                if( Application.platform == RuntimePlatform.Android )
                    _sBuilder.Append( "file:" );
                else if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor )
                    _sBuilder.Append( "file:///" );
                else
                    _sBuilder.Append( "file:/" );
                _sBuilder.Append( Application.persistentDataPath );
                _sBuilder.Append( "/" );
            }
            else
            {
                _sBuilder.Append( Application.persistentDataPath );
                if( useFile )
                    _sBuilder.Append( "//" );
                else
                    _sBuilder.Append( "/" );
            }
            _sBuilder.Append( filePath );
            return _sBuilder.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <param name="useFile">是否用于FileInfo FileStream</param>
        /// <returns></returns>
        public string GetStreamAssetDataPath( string filePath, bool useFile = true )
        {
            _sBuilder.Remove( 0 , _sBuilder.Length );
            _sBuilder.Append( Application.streamingAssetsPath );
            if( useFile )
                _sBuilder.Append( "//" );
            else
                _sBuilder.Append( "/" );
            _sBuilder.Append( filePath );
            return _sBuilder.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">要加载的文件名</param>
        /// <returns></returns>
        public string GetTemporaryCachePath( string filePath )
        {
            _sBuilder.Remove( 0 , _sBuilder.Length );
            _sBuilder.Append( Application.temporaryCachePath );
            _sBuilder.Append( "/" );
            _sBuilder.Append( filePath );
            return _sBuilder.ToString();
        }
    }
}
