using UnityEngine;
using System;
using System.Collections.Generic;
using ET;
/// <summary>
/// ====该类自动生成请勿手动修改====
/// Author : Derek Liu
/// </summary>
[Config]
public partial class StartSceneConfig : ProtoObject
{
	/// <summary>
	/// Id
	/// </summary>
	public int Id { get; private set; }
	/// <summary>
	/// 所属进程
	/// </summary>
	public int Process { get; private set; }
	/// <summary>
	/// 所属区
	/// </summary>
	public int Zone { get; private set; }
	/// <summary>
	/// 类型
	/// </summary>
	public string SceneType { get; private set; }
	/// <summary>
	/// 名字
	/// </summary>
	public string Name { get; private set; }
	/// <summary>
	/// 外网端口
	/// </summary>
	public int OuterPort { get; private set; }
	
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static Dictionary<int, StartSceneConfig> ReturnDictionary(string csv)
	{
		Dictionary<int, StartSceneConfig> vec = new Dictionary<int, StartSceneConfig>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			StartSceneConfig item = new StartSceneConfig();
			int.TryParse(reader.GetData(0, i), out int paras0);
			item.Id = paras0;
			int.TryParse(reader.GetData(1, i), out int paras1);
			item.Process = paras1;
			int.TryParse(reader.GetData(2, i), out int paras2);
			item.Zone = paras2;
			item.SceneType = reader.GetData(3, i);
			item.Name = reader.GetData(4, i);
			int.TryParse(reader.GetData(5, i), out int paras5);
			item.OuterPort = paras5;
			try
			{
				vec.Add(item.Id, item);
			}
			catch (Exception e)
			{
				Log.Error($"{e.Message} 表: StartSceneConfig 行: {i}列: Id", LogColor.red); 
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
