using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class UnitConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// Type
	/// </summary>
	public int Type { get; private set; }
	/// <summary>
	/// 名字
	/// </summary>
	public string Name { get; private set; }
	/// <summary>
	/// 描述
	/// </summary>
	public string Desc { get; private set; }
	/// <summary>
	/// 位置
	/// </summary>
	public int Position { get; private set; }
	/// <summary>
	/// 身高
	/// </summary>
	public int Height { get; private set; }
	/// <summary>
	/// 体重
	/// </summary>
	public int Weight { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, UnitConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, UnitConfig> vec = new Dictionary<int, UnitConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			UnitConfig item = new UnitConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			int.TryParse(reader.GetData(1, i), out int paras1);
			item.Type = paras1;
			item.Name = reader.GetData(2, i);
			item.Desc = reader.GetData(3, i);
			int.TryParse(reader.GetData(4, i), out int paras2);
			item.Position = paras2;
			int.TryParse(reader.GetData(5, i), out int paras3);
			item.Height = paras3;
			int.TryParse(reader.GetData(6, i), out int paras4);
			item.Weight = paras4;
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				LDebug.LogError($"{e.Message} 表: UnitConfig 行: {i}列: Id", LogColor.red); 
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
