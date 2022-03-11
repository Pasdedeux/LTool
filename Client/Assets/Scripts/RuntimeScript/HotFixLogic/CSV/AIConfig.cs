using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class AIConfig
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 所属ai
	/// </summary>
	public int AIConfigId { get; private set; }
	/// <summary>
	/// 此ai中的顺序
	/// </summary>
	public int Order { get; private set; }
	/// <summary>
	/// 节点名字
	/// </summary>
	public string Name { get; private set; }
	/// <summary>
	/// 描述
	/// </summary>
	public string Desc { get; private set; }
	/// <summary>
	/// 节点参数
	/// </summary>
	public List<int> NodeParams { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, AIConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, AIConfig> vec = new Dictionary<int, AIConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			AIConfig item = new AIConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			int.TryParse(reader.GetData(1, i), out int paras1);
			item.AIConfigId = paras1;
			int.TryParse(reader.GetData(2, i), out int paras2);
			item.Order = paras2;
			item.Name = reader.GetData(3, i);
			item.Desc = reader.GetData(4, i);
			item.NodeParams= new List<int>();
			if(!string.IsNullOrEmpty(reader.GetData(5, i)))
			{
				string[] NodeParams_Array = reader.GetData(5, i).Split(';');
				for (int j = 0; j < NodeParams_Array.Length; j++)
				{
					int.TryParse(NodeParams_Array[j], out int paras3);
					item.NodeParams.Add(paras3);
				}
			}
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				LDebug.LogError($"{e.Message} 表: AIConfig 行: {i}列: Id", LogColor.red); 
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
