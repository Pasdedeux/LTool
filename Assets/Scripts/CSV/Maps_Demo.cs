using UnityEngine;
using System;
using System.Collections.Generic;
using LitFramework;
/// <summary>
/// Author : Derek Liu
/// 创建时间:2021/8/4 10:33:45
/// </summary>
public class Maps_Demo
{
	/// <summary>
	/// ID
	/// </summary>
	public string ID { get; set; }
	/// <summary>
	/// 地图名称
	/// </summary>
	public string SuperName { get; set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<string, Maps_Demo> ReturnDictionary(string csv)
	{
		Dictionary<string, Maps_Demo> vec = new Dictionary<string, Maps_Demo>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			Maps_Demo item = new Maps_Demo();
			item.ID = reader.GetData(0, i);
			item.SuperName = reader.GetData(1, i);
			vec.Add(item.ID, item);
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
