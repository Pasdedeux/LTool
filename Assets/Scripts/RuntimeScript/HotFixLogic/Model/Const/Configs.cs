#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 创建时间:2022/2/22 16:03:39
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
	public static Dictionary<int, SpawnConfig> SpawnConfigDict;
	public static List<SpawnConfig> SpawnConfigList;
	public static void Install()
	{
		SpawnConfigList = SpawnConfigDict.Values.ToList();
	}
}
