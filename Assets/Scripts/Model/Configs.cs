#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/7/5 13:57:30
// 备注：由模板工具自动生成
///----------------------------------------------------------------*/
#endregion

using LitFramework;
using System.Linq;
using System.Collections.Generic;
public static partial class Configs
{
    //TODO 这里可以作为扩展类，记录包括JSON变量等其它配置表变量。手动维护
    public static List<Maps_Demo> Maps_DemoList;

	public static void Install()
    {
        //TODO 测试演示
        Maps_DemoList = Maps_DemoDict.Values.ToList();
    }

}
