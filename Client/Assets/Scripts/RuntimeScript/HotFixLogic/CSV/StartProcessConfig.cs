using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class StartProcessConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 所属机器
	/// </summary>
	public int MachineId { get; private set; }
	/// <summary>
	/// 内网端口
	/// </summary>
	public int InnerPort { get; private set; }
	/// <summary>
	/// 程序名
	/// </summary>
	public string AppName { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, StartProcessConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, StartProcessConfig> vec = new Dictionary<int, StartProcessConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			StartProcessConfig item = new StartProcessConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			int.TryParse(reader.GetData(1, i), out int paras1);
			item.MachineId = paras1;
			int.TryParse(reader.GetData(2, i), out int paras2);
			item.InnerPort = paras2;
			item.AppName = reader.GetData(3, i);
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				LDebug.LogError($"{e.Message} 表: StartProcessConfig 行: {i}列: Id", LogColor.red); 
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
