#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2021/8/3 17:32:15
// 备注：由模板工具自动生成
///----------------------------------------------------------------*/
#endregion

//*******************************************************************
//**                  该类由工具自动生成，请勿手动修改                   **
//*******************************************************************

using LitFramework;
using System.Linq;
using System.Collections.Generic;
public static partial class Configs
{
	public static Dictionary<string, Maps_Demo> Maps_DemoDict;
	public static List<Maps_Demo> Maps_DemoList;
	public static Dictionary<string, SpawnConfig> SpawnConfigDict;
	public static List<SpawnConfig> SpawnConfigList;
	public static void Install()
	{
		Maps_DemoList = Maps_DemoDict.Values.ToList();
		SpawnConfigList = SpawnConfigDict.Values.ToList();
	}
}
