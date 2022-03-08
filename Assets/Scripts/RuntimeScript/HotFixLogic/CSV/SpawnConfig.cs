using UnityEngine;
using System;
using System.Collections.Generic;
using LitFramework;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
public partial class SpawnConfig
{
	/// <summary>
	/// 预制件ID
	/// </summary>
	public int ID { get; private set; }
	/// <summary>
	/// C-预制件所属分类
	/// </summary>
	public string SpawnType { get; private set; }
	/// <summary>
	/// S-Resource资源路径
	/// </summary>
	public string resPath { get; private set; }
	/// <summary>
	/// #-初始数量
	/// </summary>
	public int PreloadAmount { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, SpawnConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, SpawnConfig> vec = new Dictionary<int, SpawnConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			SpawnConfig item = new SpawnConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.ID = paras0;
			item.SpawnType = reader.GetData(1, i);
			item.resPath = reader.GetData(2, i);
			int.TryParse(reader.GetData(3, i), out int paras1);
			item.PreloadAmount = paras1;
			try
			{
				vec.Add(item.ID, item);
			}
			catch (Exception e)
			{
				LDebug.LogError($"{e.Message} 表: SpawnConfig 行: {i}列: ID", LogColor.red); 
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
