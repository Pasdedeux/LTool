﻿using UnityEngine;
using System;
using System.Collections.Generic;
using LitFramework;
/// <summary>
/// Author : Derek Liu
/// 创建时间:2022/3/2 15:34:28
/// </summary>
public class SpawnConfig
{
	/// <summary>
	/// 预制件ID
	/// </summary>
	public int ID { get; private set; }
	/// <summary>
	/// 预制件所属分类
	/// </summary>
	public string SpawnType { get; private set; }
	/// <summary>
	/// Resource资源路径
	/// </summary>
	public string resPath { get; private set; }
	/// <summary>
	/// 初始数量
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
			item.ID = int.Parse(reader.GetData(0, i));
			item.SpawnType = reader.GetData(1, i);
			item.resPath = reader.GetData(2, i);
			item.PreloadAmount = int.Parse(reader.GetData(3, i));
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
