/*======================================
* 项目名称 ：LitFrameworkEditor
* 项目描述 ：
* 类 名 称 ：GlobalEditorSetting
* 类 描 述 ：
* 命名空间 ：LitFrameworkEditor
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/2/26 13:50:25
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 编辑器用全局配置
/// </summary>
public class GlobalEditorSetting
{
    //% - CTRL on Windows / CMD on OSX
    //# - Shift
    //& -Alt
    //LEFT/RIGHT/UP/DOWN - Arrow keys
    //F1 … F2 - F keys
    //HOME,END,PGUP,PGDN
    //字母键 - _ + 字母（如:_g代表按键）
    //[MenuItem("Tools/New Option %#a"]//CTRL-SHIFT-A


    //音频存放路径
    public const string AUDIO_PATH = "Sounds";//Resources

    //UI创建路径配置
    public const string UI_PREFAB_PATH = "Prefabs/UI/";
    public const string JSON_FILE_NAME = "configs.dat";
    public const string OUTPUT_RESPATH = "Essential/ResPath.cs";
}


#region UI/Sound文本模板

#region 模板类
/// <summary>
/// 路径存放模板
/// </summary>
class ResPathTemplate
{
    public Dictionary<string, string> UI = new Dictionary<string, string>() { };
    public Dictionary<string, string> Sound = new Dictionary<string, string>() { };
}


/// <summary>
/// 配置表路径注册类
/// </summary>
class ConfigsParse
{
    List<string> CSString = new List<string>();

    public string CreateCS( ConfigsNamesTemplate rpt )
    {
        AddHead();
        AddBody( rpt );
        AddTail();
        string result = GetFomatedCS();

        return result;
    }

    private void AddHead()
    {
        CSString.Add( "#region << 版 本 注 释 >>" );
        CSString.Add( "///*----------------------------------------------------------------" );
        CSString.Add( "// Author : Derek Liu" );
        CSString.Add( "// 创建时间:" + DateTime.Now.ToString() );
        CSString.Add( "// 备注：由模板工具自动生成" );
        CSString.Add( "///----------------------------------------------------------------*/" );
        CSString.Add( "#endregion" );
        CSString.Add( "" );
        CSString.Add( "//*******************************************************************" );
        CSString.Add( "//**                  该类由工具自动生成，请勿手动修改                   **" );
        CSString.Add( "//*******************************************************************" );
        CSString.Add( "" );
        CSString.Add( "using LitFramework;" );
        CSString.Add( "using System.Linq;" );
        CSString.Add( "using System.Collections.Generic;" );
        CSString.Add( "public static partial class Configs" );
        CSString.Add( "{" );
    }
    private void AddTail()
    {
        CSString.Add( "}" );
    }
    private void AddBody( ConfigsNamesTemplate rpt )
    {
        foreach ( var item in rpt.configsNameList )
        {
            CSString.Add( string.Format("public static Dictionary<{2}, {1}> {0}Dict;", item.Key, item.Key, item.Value ) );
            CSString.Add( string.Format("public static List<{1}> {0}List;", item.Key , item.Key ) );
        }

        CSString.Add( string.Format( "public static void Install()" ) );
        CSString.Add( "{" );
        foreach ( var item in rpt.configsNameList )
        {
            CSString.Add(string.Format("{0}List = {1}Dict.Values.ToList();", item.Key, item.Key));
        }
        CSString.Add( "}" );
    }
    string GetFomatedCS()
    {
        StringBuilder result = new StringBuilder();
        int tablevel = 0;
        for ( int i = 0; i < CSString.Count; i++ )
        {
            string tab = "";

            for ( int j = 0; j < tablevel; ++j )
                tab += "\t";

            if ( CSString[ i ].Contains( "{" ) )
                tablevel++;
            if ( CSString[ i ].Contains( "}" ) )
            {
                tablevel--;
                tab = "";
                for ( int j = 0; j < tablevel; ++j )
                    tab += "\t";
            }

            result.Append( tab + CSString[ i ] + "\n" );
        }
        return result.ToString();
    }
}
/// <summary>
/// 配置表访问文件配置文件
/// </summary>
class ConfigsNamesTemplate
{
    public Dictionary<string, string> configsNameList = new Dictionary<string, string>();
}
#endregion

#endregion
