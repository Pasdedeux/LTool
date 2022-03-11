using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class StartZoneConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 数据库地址
	/// </summary>
	public string DBConnection { get; private set; }
	/// <summary>
	/// 数据库名
	/// </summary>
	public string DBName { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, StartZoneConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, StartZoneConfig> vec = new Dictionary<int, StartZoneConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			StartZoneConfig item = new StartZoneConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			item.DBConnection = reader.GetData(1, i);
			item.DBName = reader.GetData(2, i);
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				Log.Error($"{e.Message} 表: StartZoneConfig 行: {i}列: Id", LogColor.red); 
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
