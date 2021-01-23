using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FolderCopy
{
    /// <summary>
    /// 拷贝文件夹
    /// </summary>
    /// <param name="oripath">源目录  Application.dataPath + "/Resources"</param>
    /// <param name="despath">目标目录  Application.dataPath + "/Resources1"</param>
    public static void CopyTo( string oripath, string despath )
    {
        DirectoryInfo folder = new DirectoryInfo( oripath );

        if ( !Directory.Exists( despath ) )
            Directory.CreateDirectory( despath );

        foreach ( var item in folder.GetFiles() )
        {
            if ( item.Extension != ".meta" )
            {
                FileInfo desfile = new FileInfo( despath + "/" + item.Name );
                if ( desfile.Exists )
                    desfile.Delete();
                item.CopyTo( despath + "/" + item.Name );
            }

        }

        foreach ( var item in folder.GetDirectories() )
        {
            string ori = oripath + "/" + item.Name;
            string des = despath + "/" + item.Name;
            CopyTo( ori, des );
        }
    }
    /// <summary>
    /// 移动文件夹
    /// </summary>
    /// <param name="oripath">源目录  Application.dataPath + "/Resources"</param>
    /// <param name="despath">目标目录  Application.dataPath + "/Resources1"</param>
    public static void MoveTo( string oripath, string despath )
    {
        DirectoryInfo folder = new DirectoryInfo( oripath );

        if ( !Directory.Exists( despath ) )
            Directory.CreateDirectory( despath );

        foreach ( var item in folder.GetFiles() )
        {
            if ( item.Extension != ".meta" )
            {
                FileInfo desfile = new FileInfo( despath + "/" + item.Name );
                if ( desfile.Exists )
                    desfile.Delete();
                item.MoveTo( despath + "/" + item.Name );
            }

        }

        foreach ( var item in folder.GetDirectories() )
        {
            string ori = oripath + "/" + item.Name;
            string des = despath + "/" + item.Name;
            MoveTo( ori, des );
        }
    }

}
