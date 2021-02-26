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
        //音频存放路径
        public const string AUDIO_PATH = "Audios";//Resources

        //UI创建路径配置
        public const string JSON_FILE_NAME = "configs.dat";
        public const string OUTPUT_FILENAME = "ResPath.cs";
    }



#region 模板类
/// <summary>
/// 路径存放模板
/// </summary>
class ResPathTemplate
{
    public Dictionary<string, string> UI = new Dictionary<string, string>() { };
    public Dictionary<string, string> Sound = new Dictionary<string, string>() { };
}
#endregion
