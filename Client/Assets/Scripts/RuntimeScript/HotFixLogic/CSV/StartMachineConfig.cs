using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class StartMachineConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 内网地址
	/// </summary>
	public string InnerIP { get; private set; }
	/// <summary>
	/// 外网地址
	/// </summary>
	public string OuterIP { get; private set; }
	/// <summary>
	/// 守护进程端口
	/// </summary>
	public string WatcherPort { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, StartMachineConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, StartMachineConfig> vec = new Dictionary<int, StartMachineConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			StartMachineConfig item = new StartMachineConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			item.InnerIP = reader.GetData(1, i);
			item.OuterIP = reader.GetData(2, i);
			item.WatcherPort = reader.GetData(3, i);
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				LDebug.LogError($"{e.Message} 表: StartMachineConfig 行: {i}列: Id", LogColor.red); 
			}
		}
		return vec;
	}
	
	/// <summary>
	/// 解析Vector3
	/// </summary>
	/// <param name="string">配置文件数据</param>
	/// <returns>Vector3</returns>
	static Vector3 ParseVector3(string str)
	{
		str = str.Substring(1, str.Length - 2);
		str.Replace(" ", "");
		string[] splits = str.Split(',');
		float x = float.Parse(splits[0]);
		float y = float.Parse(splits[1]);
		float z = float.Parse(splits[2]);
		return new Vector3(x, y, z);
	}
	
}
