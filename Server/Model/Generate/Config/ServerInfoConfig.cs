using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class ServerInfoConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 区服名称
	/// </summary>
	public string ServerName { get; private set; }
	/// <summary>
	/// 区服描述
	/// </summary>
	public string ServerDesc { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, ServerInfoConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, ServerInfoConfig> vec = new Dictionary<int, ServerInfoConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			ServerInfoConfig item = new ServerInfoConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			item.ServerName = reader.GetData(1, i);
			item.ServerDesc = reader.GetData(2, i);
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				Log.Error($"{e.Message} 表: ServerInfoConfig 行: {i}列: Id", LogColor.red); 
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
