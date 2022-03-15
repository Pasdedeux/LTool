#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 备注：由模板工具自动生成
///----------------------------------------------------------------*/
#endregion

//*******************************************************************
//**                  该类由工具自动生成，请勿手动修改                   **
//*******************************************************************

using System.Linq;
using System.Collections.Generic;
public static partial class Configs
{
	public static Dictionary<int, AIConfig> AIConfigDict;
	public static List<AIConfig> AIConfigList;
	public static Dictionary<int, RobotsConfig> RobotsConfigDict;
	public static List<RobotsConfig> RobotsConfigList;
	public static Dictionary<int, ServerInfoConfig> ServerInfoConfigDict;
	public static List<ServerInfoConfig> ServerInfoConfigList;
	public static Dictionary<int, SpawnConfig> SpawnConfigDict;
	public static List<SpawnConfig> SpawnConfigList;
	public static Dictionary<int, StartMachineConfig> StartMachineConfigDict;
	public static List<StartMachineConfig> StartMachineConfigList;
	public static Dictionary<int, StartProcessConfig> StartProcessConfigDict;
	public static List<StartProcessConfig> StartProcessConfigList;
	public static Dictionary<int, UnitConfig> UnitConfigDict;
	public static List<UnitConfig> UnitConfigList;
	public static Dictionary<int, StartSceneConfig> StartSceneConfigDict;
	public static List<StartSceneConfig> StartSceneConfigList;
	public static Dictionary<int, StartZoneConfig> StartZoneConfigDict;
	public static List<StartZoneConfig> StartZoneConfigList;
	public static void Install()
	{
		AIConfigList = AIConfigDict.Values.ToList();
		RobotsConfigList = RobotsConfigDict.Values.ToList();
		ServerInfoConfigList = ServerInfoConfigDict.Values.ToList();
		SpawnConfigList = SpawnConfigDict.Values.ToList();
		StartMachineConfigList = StartMachineConfigDict.Values.ToList();
		StartProcessConfigList = StartProcessConfigDict.Values.ToList();
		UnitConfigList = UnitConfigDict.Values.ToList();
		StartSceneConfigList = StartSceneConfigDict.Values.ToList();
		StartZoneConfigList = StartZoneConfigDict.Values.ToList();
	}
}
