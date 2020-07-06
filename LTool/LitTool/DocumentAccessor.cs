#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：LitFramework.LitTool
* 项目描述 ：
* 类 名 称 ：FileSerilizer
* 类 描 述 ：
* 所在的域 ：DEREK-HOMEPC
* 命名空间 ：LitFramework.LitTool
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/9 21:24:56
* 更新时间 ：2018/5/9 21:24:56
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LitFramework.LitTool
{
    public class DocumentAccessor : Singleton<DocumentAccessor>
    {
        private static object _lock = new object();

        /// <summary>
        /// 参数1-解析出的一条完整信息
        /// 参数2-以解析出的信息第一个元素为键，整条消息作为值构建的字典   
        /// </summary>
        public static Func<List<string> , Dictionary<string , List<string>> , bool> ReadTextAdditionalCondition =
            ( e , d ) => { return true; };

        /// <summary>
        /// 解析txt 或者 远端txt内容
        /// </summary>
        /// <param name="path">地址或者内容文本</param>
        /// <param name="isContent">path字符串如果是地址，为false，反之为true</param>
        /// <param name="identifier">默认分隔符为=</param>
        /// <returns></returns>
        public static Dictionary<string , List<string>> OpenText( string path , bool isContent = false , string identifier = "=" )
        {
            Dictionary<string , List<string>> content = new Dictionary<string , List<string>>();
            Char chars = identifier.ToCharArray()[ 0 ];

            if( !isContent )
            {
                StreamReader sr = new StreamReader( path , Encoding.UTF8 );
                string line;
                while( ( line = sr.ReadLine() ) != null )
                {
                    if( string.IsNullOrEmpty( line ) )
                        continue;
                    List<string> list = line.Split( chars ).ToList();

                    if( !content.ContainsKey( list[ 0 ] ) )
                        content.Add( list[ 0 ] , list );
                    else
                    {
                        if( ReadTextAdditionalCondition( list , content ) )
                            content[ list[ 0 ] ] = list;
                    }
                }
                sr.Dispose();
                sr = null;
            }
            else
            {
                path.Replace( "\r\n" , "\n" );
                string[] res = path.Split( new string[] { "\r\n" } , StringSplitOptions.None );
                for( int i = 0; i < res.Length; i++ )
                {
                    if( !string.IsNullOrEmpty( res[ i ] ) )
                    {
                        List<string> list = res[ i ].Split( chars ).ToList();
                        content[ list[ 0 ] ] = list;
                    }
                }
            }
            return content;

        }

        /// <summary>
        /// 将本地版本字典保存到文件，如txt
        /// </summary>
        /// <param name="targetList">目标字典</param>
        /// <param name="dataPath">存入的目标文件</param>
        /// <param name="append">追加的信息，会被添加到第一行</param>
        /// <param name="identifier">目标字典值的连接符</param>
        public static void SaveText( Dictionary<string , List<string>> targetList , string dataPath , string append = null , string identifier = "=" )
        {
            FileInfo fi = new FileInfo( dataPath );
            if( fi.Exists )
                fi.Delete();

            StringBuilder sb = new StringBuilder();
            if( append != null )
                sb.AppendLine( append );
            foreach( var item in targetList )
            {
                sb.AppendLine( string.Join( identifier , item.Value.ToArray() ) );
            }

            WriteFile( sb , dataPath );
        }

        /// <summary>
        /// 写入文件到目标地址
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="targetPath"></param>
        public static void WriteFile( StringBuilder sb , string targetPath )
        {
            lock( _lock )
            {
                using( FileStream fs = new FileStream( targetPath , FileMode.Create ) )
                {
                    StreamWriter sw = new StreamWriter( fs );
                    sw.WriteLine( sb );
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 读取目标地址文件
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="targetPath">完整地址+名称</param>
        public static string ReadFile( string targetPath )
        {
            FileInfo t = new FileInfo( targetPath );
            if( !t.Exists )
            {
                Debug.Log( "文件不存在:" + targetPath );
                return string.Empty;
            }
            //使用流的形式读取
            string str = null;
            if( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor )
            {
                try
                {
                    str = File.ReadAllText( targetPath );
                }
                catch( Exception e )
                {
                    Debug.LogError( "File.ReadAllText error " + targetPath + "  " + e.Message );
                    //路径与名称未找到文件则直接返回空
                    return null;
                }
            }
            else
            {
                //使用流的形式读取
                StreamReader sr = null;
                StringBuilder sb = new StringBuilder();
                try
                {
                    // 由于在iOS平台下, 读取StreamingAssets 路径的文本文件，必须使用不含 file:// 前缀的路径
                    // 所以在该函数中, 只能在内部使用条件编译选项, 并且直接使用 Application.streamingAssetsPath 来组合路径
                    //var fn = string.Format( "{0}/{1}" , Application.persistentDataPath , name );

                    var fn = targetPath;
                    sr = File.OpenText( fn );

                    string line;
                    while( ( line = sr.ReadLine() ) != null )
                    {
                        //一行一行的读取
                        //将每一行的内容存入数组链表容器中
                        sb.AppendLine( line );
                    }
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    if( sr != null )
                    {
                        //关闭流
                        sr.Close();
                        //销毁流
                        sr.Dispose();
                    }
                }
                //将数组链表容器返回
                return sb.ToString();
            }
            //将数组链表容器返回
            return str;
        }

        /// <summary>
        /// 保存www资源到本地
        /// </summary>
        /// <param name="dataPath">完整目标地址</param>
        /// 
        /// 例如  SaveAsset2LocalFile( path , w.bytes , w.bytes.Length );
        /// <param name="info"></param>
        /// <param name="length"></param>
        public static void SaveAsset2LocalFile( string dataPath, byte[] info, int length )
        {
            Stream sw = null;

            FileInfo fileInfo = new FileInfo( dataPath );
            if ( !fileInfo.Directory.Exists ) fileInfo.Directory.Create();
            if ( fileInfo.Exists ) fileInfo.Delete();
            
            //如果此文件不存在则创建  
            sw = fileInfo.Create();
            //写入  
            sw.Write( info, 0, length );
            //写入并清除字节流
            sw.Flush();
            //关闭流  
            sw.Close();
            //销毁流  
            sw.Dispose();
            //关闭文件
            fileInfo = null;
        }

        public static bool IsExists( string fileFullPath )
        {
            FileInfo fileInfo = new FileInfo( fileFullPath );
            return fileInfo.Exists;
        }

        #region UnityWebRequest
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">例如：Application.streamingAssetsPath+ "Csv/CutTool.csv"</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IEnumerator ILoadAsset( string path , Action<UnityWebRequest> callBack )
        {
            Uri uri = new Uri( path );
            LDebug.LogWarning( " >路径: \n AbsoluteUri : " + uri.AbsoluteUri + " \n AbsolutePath: " + uri.AbsolutePath + " \n LocalPath: " + uri.LocalPath );
            using ( UnityWebRequest uwr = UnityWebRequest.Get( uri ) )
            {
                uwr.timeout = 5;
                uwr.disposeUploadHandlerOnDispose = true;
                uwr.disposeDownloadHandlerOnDispose = true;
                uwr.disposeCertificateHandlerOnDispose = true;

                yield return uwr.SendWebRequest();

                if ( uwr.isNetworkError || uwr.isHttpError )
                {
                    LDebug.LogError( "  >Error: " + uwr.error );
                }
                else
                {
                    callBack?.Invoke( uwr );
                    LDebug.Log( " >Received: \n" + uwr.downloadHandler.text );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">例如：Application.streamingAssetsPath+ "Csv/CutTool.csv"</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static IEnumerator ILoadAsset( string path, Action<string> callBack )
        {
            Uri uri = new Uri( path );
            LDebug.LogWarning( " >路径: \n AbsoluteUri : " + uri.AbsoluteUri + " \n AbsolutePath: " + uri.AbsolutePath + " \n LocalPath: " + uri.LocalPath );
            using ( UnityWebRequest uwr = UnityWebRequest.Get( uri ) )
            {
                uwr.timeout = 5;
                uwr.disposeUploadHandlerOnDispose = true;
                uwr.disposeDownloadHandlerOnDispose = true;
                uwr.disposeCertificateHandlerOnDispose = true;

                yield return uwr.SendWebRequest();

                if ( uwr.isNetworkError || uwr.isHttpError )
                {
                    LDebug.LogError( "  >Error: " + uwr.error );
                }
                else
                {
                    callBack?.Invoke( uwr.downloadHandler.text );
                    LDebug.Log( " >Received: \n" + uwr.downloadHandler.text );
                }
            }
        }
        
        #endregion

        #region WWW加载

        /// <summary>
        /// www加载，Unity版本
        /// </summary>
        /// <param name="filePath">包含IP地址在内（网络请求时）的完整地址</param>
        /// <param name="callBack">加载完成后的回调</param>
        /// <returns></returns>
        public IEnumerator WWWLoading( string filePath , Action<WWW> callBack = null )
        {
            WWW www = new WWW( filePath );

            //while ( !www.isDone ) { }
            yield return www;

            if ( www.error != null )
                throw new Exception( string.Format( "WWW Error: {0}  filePath: {1}", www.error, filePath ) );

            if( www.isDone )
            {
                if( callBack != null )
                    callBack.Invoke( www );
            }

            yield return null;

            www.Dispose();
            www = null;
        }


        /// <summary>
        /// www加载
        /// </summary>
        /// <param name="filePath">包含IP地址在内（网络请求时）的完整地址</param>
        /// <param name="callBack">加载完成后的回调</param>
        /// <returns></returns>
        public static string WWWLoadingWithWaiting( string wwwFilePath, Action<WWW> callBack = null )
        {
            LDebug.Log( wwwFilePath );
            string resutl = null;
            WWW www = new WWW( wwwFilePath );
            while ( !www.isDone ) { }

            if ( www.error != null )
            {
                if ( Application.platform == RuntimePlatform.WindowsEditor )
                    throw new Exception( string.Format( "WWW Error: {0}  filePath: {1}  ", www.error, wwwFilePath ) );
                else
                    Debug.LogError( "WWW Error: " + www.error );
            }

            if ( www.isDone )
            {
                if ( callBack != null )
                    callBack.Invoke( www );
            }

            resutl = www.text;
            www.Dispose();
            www = null;

            return resutl;
        }

        #endregion


    }
}
