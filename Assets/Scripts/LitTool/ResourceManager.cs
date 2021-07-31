/*======================================
* 项目名称 ：Assets.Scripts.Manager
* 项目描述 ：
* 类 名 称 ：ResourceManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Manager
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/7/5 11:33:15
* 更新时间 ：2019/7/5 11:33:15
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note: 直接使用Unity2018 以后的版本，暂不支持旧版本
*修改时间：2019/7/5 11:33:15
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using LitFramework.Base;
using LitFramework.MsgSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;


namespace LitFramework.LitTool
{
    /// <summary>
    /// 待完善-基于resource.load的图集加载、assetbundle、文件加载等
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>, IManager
    {
        public event Action<string> LoadErrorEventHandler;
        public event Action<string> LoadFileWithStringEventHandler;
        public event Action<byte[]> LoadFileWithRawDataEventHandler;
        public event Action<Texture2D> LoadTextureEventHandler;
        public event Action<AssetBundle> LoadAssetBundleEventHandler;

        private Dictionary<string, Sprite[]> _atlasDict = new Dictionary<string, Sprite[]>();

        //游戏配置档
        private const string CSV_CONFIG_LIST = "csvList.txt";
        //AB配置档
        private const string AB_CONFIG_LIST = "ABVersion.csv";
        //资源服地址
        private string REMOTE_IP = "http://192.168.1.102/";
        //需要下载更新的配置档文件
        private List<string> _toBeDownloadCsvList = new List<string>()
        {
            CSV_CONFIG_LIST,
            AB_CONFIG_LIST,
        };

        #region AB Load

        public bool IsLoadABEnd;
        private bool _isLoadAB;
        private int _loadAbNum = -1;

        public void DownLoadCSVAB( string ip, string username = null, string password = null )
        {
            _isLoadAB = false;
            //执行远程更新
            if ( FrameworkConfig.Instance.UseRemotePersistantPath )
            {
                LDebug.Log( "热更连接地址：" + ip );

                REMOTE_IP = ip;
                LitTool.MonoBehaviour.StartCoroutine( IDownLoadVersion() );
            }
            //选择不远程更新
            else
                MsgManager.Instance.Broadcast( InternalEvent.END_LOAD_REMOTE_CONFIG );
        }


        /// <summary>
        /// 更新流程说明：<see href="https://www.processon.com/view/link/61013cd4637689719d2d8166">流程图</see>
        /// </summary>
        public IEnumerator IDownLoadVersion()
        {
            MsgManager.Instance.Broadcast( InternalEvent.START_LOAD_REMOTE_CONFIG );

            yield return null;

            for ( int i = 0; i < _toBeDownloadCsvList.Count; i++ )
            {
                //1、下载最新的资源配置信息
                bool canGoFurther = true;
                string localContent = null;
                string remoteFilePath = _toBeDownloadCsvList[ i ];

                //发送下载XX文件事件
                MsgManager.Instance.Broadcast( InternalEvent.HANDLING_REMOTE_RES, new MsgArgs( remoteFilePath, InternalEvent.RemoteStatus.Download ) );

                string localFilePath = AssetPathManager.Instance.GetPersistentDataPath( remoteFilePath, false );
                //2、根据本地是否存在资源配置信息，如果不存在，则视为远程更新流程不执行
                if ( DocumentAccessor.IsExists( localFilePath ) )
                {
                    string remoteContent = null;
                    byte[] contentByteArr = null;

                    //远程主配置文件获取
                    yield return DocumentAccessor.ILoadAsset( REMOTE_IP + remoteFilePath, callBack: ( UnityWebRequest e ) =>
                    {
                        LDebug.Log( "Remote update..." + remoteFilePath + "读取完成" );
                        remoteContent = e.downloadHandler.text;
                        contentByteArr = e.downloadHandler.data;
                    },
                    errorCallBack: ( UnityWebRequest e ) =>
                    {
                        LDebug.LogError( "Remote Error..." + e );
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

                    //本地配置表默认全更新。
                    if ( remoteFilePath == CSV_CONFIG_LIST )
                    {
                        string[] str = remoteContent.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                        int toLoadNum = str.Length;

                        string[] localFileContent = localContent.Split( "\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                        var toDelete = localFileContent.Where( a => !str.Contains( a ) );

                        foreach ( var item in toDelete )
                        {
                            FileInfo fileInfo = new FileInfo( AssetPathManager.Instance.GetPersistentDataPath( item, false ) );
                            if ( fileInfo.Exists ) fileInfo.Delete();
                            LDebug.Log( ">>>Delete " + item, LogColor.red );
                            LDebug.Log( ">>>Delete Result " + DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( item, false ) ), LogColor.red );
                        }

                        for ( int w = 0; w < str.Length; w++ )
                        {
                            LDebug.Log( "Remote update..." + str[ w ] + "开始读取" );
                            yield return DocumentAccessor.ILoadAsset( REMOTE_IP + str[ w ], ( UnityWebRequest e ) =>
                            {
                                LDebug.Log( "Remote update..." + str[ w ] + "读取完成", LogColor.yellow );
                                DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( str[ w ], false ), e.downloadHandler.data );
                            } );
                        }
                    }
                    //AB资源默认增量更新
                    else if ( remoteFilePath == AB_CONFIG_LIST )
                    {
                        Dictionary<string, ABVersion> remoteABVersionsDic = ResolveABContent( remoteContent );
                        Dictionary<string, ABVersion> localABVersionsDic = ResolveABContent( localContent );

                        //需要删除的对象
                        var toDelete = localABVersionsDic.Where( a => !remoteABVersionsDic.ContainsKey( a.Key ) );
                        foreach ( var item in toDelete )
                        {
                            FileInfo fileInfo = new FileInfo( AssetPathManager.Instance.GetPersistentDataPath( FrameworkConfig.Instance.ABFolderName + "/" + item.Key, false ) );
                            if ( fileInfo.Exists ) fileInfo.Delete();
                            LDebug.Log( ">>>Delete " + item.Key, LogColor.red );
                            LDebug.Log( ">>>Delete Result " + DocumentAccessor.IsExists( AssetPathManager.Instance.GetPersistentDataPath( FrameworkConfig.Instance.ABFolderName + "/" + item.Key, false ) ), LogColor.red );
                        }

                        //需要更新的对象：可以根据需求拓展对version的使用规则。
                        //默认是更新版本号更高或者新增加的对象。
                        var toUpdate = remoteABVersionsDic.Where( a => !localABVersionsDic.ContainsKey( a.Key ) || a.Value.Version > localABVersionsDic[ a.Key ].Version );
                        foreach ( var item in toUpdate )
                        {
                            LDebug.Log( "Remote update..." + REMOTE_IP + FrameworkConfig.Instance.ABFolderName + "/" + item.Key + " 开始读取" );
                            yield return DocumentAccessor.ILoadAsset( REMOTE_IP + FrameworkConfig.Instance.ABFolderName + "/" + item.Key, ( UnityWebRequest e ) =>
                            {
                                LDebug.Log( "Remote update..." + item.Key + "读取完成", LogColor.yellow );
                                DocumentAccessor.SaveAsset2LocalFile( AssetPathManager.Instance.GetPersistentDataPath( FrameworkConfig.Instance.ABFolderName + "/" + item.Key, false ), e.downloadHandler.data );
                            } );
                        }
                    }

                    //更新文档
                    DocumentAccessor.SaveAsset2LocalFile( localFilePath, contentByteArr );
                }
                else
                {
                    LDebug.LogErrorFormat( "开启了远端更新，但是本地不存在文件 {0}", remoteFilePath );
                    continue;
                }
            }
            MsgManager.Instance.Broadcast( InternalEvent.END_LOAD_REMOTE_CONFIG );
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

        #region AB包加载资源
        private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();

        private AssetBundle mainAB = null;
        private AssetBundleManifest mainManifest = null;
        /// <summary>
        /// AB包路径
        /// </summary>
        private string ABFilePath
        {
            get
            {
                return FrameworkConfig.Instance.UsePersistantPath ? Application.persistentDataPath + string.Format( "/{0}/", FrameworkConfig.Instance.ABFolderName ) : Application.streamingAssetsPath + string.Format( "/{0}/", FrameworkConfig.Instance.ABFolderName );
            }
        }

        /// <summary>
        /// 主包名字
        /// </summary>
        private string MainABName
        {
            get
            {
#if UNITY_IOS
            
#elif UNITY_ANDROID

#else

#endif
                return FrameworkConfig.Instance.ABTotalName;
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AssetBundle LoadAB( string name )
        {
            if ( mainAB == null )
            {
                Debug.Log( ABFilePath + MainABName );
                mainAB = AssetBundle.LoadFromFile( ABFilePath + MainABName );
                mainManifest = mainAB.LoadAsset<AssetBundleManifest>( "AssetBundleManifest" );
            }
            string[] str = mainManifest.GetAllDependencies( name );

            for ( int i = 0; i < str.Length; i++ )
            {
                if ( !bundleDic.ContainsKey( str[ i ] ) )
                {
                    AssetBundle ab = AssetBundle.LoadFromFile( ABFilePath + str[ i ] );
                    bundleDic.Add( str[ i ], ab );
                }
            }
            if ( !bundleDic.ContainsKey( name ) )
            {
                AssetBundle ab = AssetBundle.LoadFromFile( ABFilePath + name );
                bundleDic.Add( name, ab );
            }
            return bundleDic[ name ];
        }

        /// <summary>
        /// 从AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadFromAB<T>( string rule ) where T : UnityEngine.Object
        {
            string[] names = rule.Split( '#' );
            string abName = names[ 0 ];
            string resName = names[ 1 ];
            AssetBundle ab = LoadAB( abName );
            return ab.LoadAsset<T>( resName );
        }

        /// <summary>
        /// 卸载没有被使用的资源
        /// </summary>
        public void RemoveMenory()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 卸载单个AB包
        /// </summary>
        /// <param name="name"></param>
        public void ClearOneAB( string name )
        {
            if ( bundleDic.ContainsKey( name ) )
            {
                bundleDic[ name ].Unload( false );
                bundleDic.Remove( name );
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        public void ClearAll()
        {
            AssetBundle.UnloadAllAssetBundles( false );
            bundleDic.Clear();
            mainAB = null;
            mainManifest = null;
        }
        #endregion

        #endregion

        #region FTP方式（废弃）

        /// <summary>
        /// 当所有资源下载完成或者更新完成,开始游戏
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        IEnumerator DownLoadABIE( string abName )
        {
            yield return null;
            DownLoadAB( abName );
            _loadAbNum--;
        }


        IEnumerator ILoadAndDownAbPackageFile( string abPath, string downLoadPath, string filePath )
        {
            UnityWebRequest request = UnityWebRequest.Get( filePath );
            yield return request.SendWebRequest();
            string str = request.downloadHandler.text;
            string[] abSources = str.Split( ',' );
            int allNum = abSources.Length;
            int indexNum = 0;
            string downLoadTruePath;
            for ( int i = 0; i < allNum; i++ )
            {
                string loadPath = string.Format( "{0}/{1}", abPath, abSources[ i ] );
                downLoadTruePath = string.Format( "{0}/{1}", downLoadPath, abSources[ i ] );
                request = UnityWebRequest.Get( loadPath );
                yield return request.SendWebRequest();
                downloadAndSave( request, downLoadTruePath );
                if ( ++indexNum == allNum )
                {
                    _isLoadAB = true;
                }
            }

            yield return new WaitUntil( () => _isLoadAB );
            LDebug.Log( "ab包下载完成" );
        }

        public void DownLoadAB( string AbSources )
        {
            //本地有没有,有的话更新,没有就读取streamingAssetsPath
            //有就根据版本更新,没有就去streamingAssets下载完整ab资源
            //更新完成或者下载完成,才开始进入游戏
            string loadPath = Application.persistentDataPath + "/" + FrameworkConfig.Instance.ABFolderName;
            if ( !Directory.Exists( loadPath ) )
                Directory.CreateDirectory( loadPath );

            string userName = "";
            string password = "";
            downloadWithFTP( "ftp://192.168.1.227/" + AbSources, loadPath + "/" + AbSources, userName: userName, password: password );
        }

        public void DownLoadSources( string sources, string loadPath )
        {
            string userName = "";
            string password = "";

            downloadWithFTP( "ftp://192.168.1.227/" + sources, loadPath, userName: userName, password: password );
        }

        private byte[] downloadWithFTP( string ftpUrl, string savePath = "", string userName = "", string password = "" )
        {
            FtpWebRequest request = ( FtpWebRequest )WebRequest.Create( new Uri( ftpUrl ) );
            //request.Proxy = null; 
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;

            //If username or password is NOT null then use Credential
            if ( !string.IsNullOrEmpty( userName ) && !string.IsNullOrEmpty( password ) )
            {
                request.Credentials = new NetworkCredential( userName, password );
            }

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            //If savePath is NOT null, we want to save the file to path
            //If path is null, we just want to return the file as array
            if ( !string.IsNullOrEmpty( savePath ) )
            {
                downloadAndSave( request.GetResponse(), savePath );
                return null;
            }
            else
            {
                return downloadAsbyteArray( request.GetResponse() );
            }
        }

        byte[] downloadAsbyteArray( WebResponse request )
        {
            using ( Stream input = request.GetResponseStream() )
            {
                byte[] buffer = new byte[ 16 * 1024 ];
                using ( MemoryStream ms = new MemoryStream() )
                {
                    int read;
                    while ( input.CanRead && ( read = input.Read( buffer, 0, buffer.Length ) ) > 0 )
                    {
                        ms.Write( buffer, 0, read );
                    }
                    return ms.ToArray();
                }
            }
        }

        void downloadAndSave( WebResponse request, string savePath )
        {
            Stream reader = request.GetResponseStream();
            //Create Directory if it does not exist
            if ( !Directory.Exists( Path.GetDirectoryName( savePath ) ) )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( savePath ) );
            }

            FileStream fileStream = new FileStream( savePath, FileMode.Create );

            int bytesRead = 0;
            byte[] buffer = new byte[ 2048 ];

            while ( true )
            {
                bytesRead = reader.Read( buffer, 0, buffer.Length );

                if ( bytesRead == 0 )
                    break;

                fileStream.Write( buffer, 0, bytesRead );
            }
            fileStream.Close();
            //fileStream = null;
        }

        void downloadAndSave( UnityWebRequest request, string savePath )
        {
            LDebug.Log( "创建文件" + savePath );
            savePath = savePath.Trim();
            //Create Directory if it does not exist
            if ( !Directory.Exists( Path.GetDirectoryName( savePath ) ) )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( savePath ) );
            }
            FileStream fileStream = new FileStream( savePath, FileMode.Create );
            fileStream.Write( request.downloadHandler.data, 0, request.downloadHandler.data.Length );
            fileStream.Close();
            //fileStream = null;
        }

        #endregion

        #region 远程加载文件、AB包、纹理等 采用UnityWebRequest 方式。BAN
        /// <summary>
        /// 加载 .txt/ .dat/ .csv等文件
        /// </summary>
        /// <param name="searchPath">要加载文件的【带后缀】完整路径 Application.streamingAssetsPath+ "Csv/CutTool.csv"</param>
        /// <param name="useRawDataArray">true - 返回的是download data    false - 直接返回结果字符串</param>
        /// <returns></returns>
        private IEnumerator IDoLoadFileUWR( string searchPath, bool useRawDataArray = false )
        {
            using ( UnityWebRequest request = UnityWebRequest.Get( searchPath ) )
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    LDebug.LogError( request.error );
                }
                else
                {
                    if ( useRawDataArray )
                        LoadFileWithRawDataEventHandler?.Invoke( request.downloadHandler.data );
                    else
                        LoadFileWithStringEventHandler?.Invoke( request.downloadHandler.text );
                }
            }
        }

        /// <summary>
        /// 加载png 等图像格式
        /// </summary>
        /// <param name="searchPath">要加载文件的【带后缀】完整路径 Application.streamingAssetsPath+ "Csv/CutTool.csv"</param>
        /// <returns></returns>
        private IEnumerator IDoLoadTextureUWR( string searchPath )
        {
            using ( UnityWebRequest request = UnityWebRequestTexture.GetTexture( searchPath ) )
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    LDebug.LogError( request.error );
                }
                else
                {
                    Texture2D texture = ( request.downloadHandler as DownloadHandlerTexture ).texture;
                    LoadTextureEventHandler?.Invoke( texture );
                }
            }
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// /// <param name="searchPath">要加载文件的【带后缀】完整路径 Application.streamingAssetsPath+ "Csv/CutTool.csv"</param>
        /// <returns></returns>
        private IEnumerator IDoLoadAssetBundleUWR( string searchPath )
        {
            using ( UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle( searchPath ) )
            {
                yield return request.SendWebRequest();
                if ( request.isHttpError || request.isNetworkError )
                {
                    LoadErrorEventHandler?.Invoke( request.error );
                    LDebug.LogError( request.error );
                }
                else
                {
                    AssetBundle assetBundle = ( request.downloadHandler as DownloadHandlerAssetBundle ).assetBundle;
                    LoadAssetBundleEventHandler?.Invoke( assetBundle );
                }
            }
        }
        #endregion

        /// <summary>
        /// 加载图集中的子对象
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="atlasPath"></param>
        /// <returns></returns>
        public Sprite LoadSpriteAtlas( string spriteName, string atlasPath = null )
        {
            //常驻内存
            Sprite sprite = Resources.Load<Sprite>( spriteName );

            if ( sprite != null || string.IsNullOrEmpty( atlasPath ) )
            {
                return GameObject.Instantiate<Sprite>( sprite );
            }
            if ( !_atlasDict.ContainsKey( atlasPath ) )
            {
                Sprite[] atlasSprites = Resources.LoadAll<Sprite>( atlasPath );
                _atlasDict.Add( atlasPath, atlasSprites );
            }

            var sprites = _atlasDict[ atlasPath ];
            var length = _atlasDict[ atlasPath ].Length;
            for ( int i = 0; i < length; i++ )
            {
                if ( sprites[ i ].name.Equals( string.Concat( new string[] { atlasPath, "_", spriteName } ) ) )

                    return sprite = sprites[ i ];
            }
            return GameObject.Instantiate<Sprite>( sprite );
        }

        public void Install() { }

        public void Uninstall() { }
    }

    /// <summary>
    /// csv资源文件内容
    /// </summary>
    public class ABVersion
    {
        public string AbName;
        public int Version;
        public string MD5;
    }
}
