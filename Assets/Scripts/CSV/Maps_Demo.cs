using UnityEngine;
using System;
using System.Collections.Generic;
using LitFramework;
/// <summary>
/// Author : Derek Liu
/// 创建时间:2021/7/5 13:57:30
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
	/// 地图场景
	/// </summary>
	public string SceneName { get; set; }
	/// <summary>
	/// 刷兵时间点-秒
	/// </summary>
	public Vector3 Time { get; set; }
	/// <summary>
	/// 刷兵ID  RolesID:代表波次  |代表同一时刻刷怪
	/// </summary>
	public string RolesID { get; set; }
	/// <summary>
	/// 刷兵等级
	/// </summary>
	public string RolesLevel { get; set; }
	/// <summary>
	/// 击杀金币
	/// </summary>
	public string Gold { get; set; }
	/// <summary>
	/// 购买技能金币消耗
	/// </summary>
	public int BuySkillCostGold { get; set; }
	/// <summary>
	/// 是否boss
	/// </summary>
	public bool IsBoss { get; set; }
	/// <summary>
	/// 首次通关奖励道具id
	/// </summary>
	public string FirstPassGoods { get; set; }
	/// <summary>
	/// 首次击杀掉落道具数量
	/// </summary>
	public string FirstKillGoodsNum { get; set; }
	
		[Obsolete]
	/// <summary>
	/// 读取配置文件
	/// </summary>
	/// <param name="config">配置文件数据</param>
	/// <returns>数据列表</returns>
	public static List<Maps_Demo> ReturnList(string csv)
	{
		List<Maps_Demo> vec = new List<Maps_Demo>();
		CSVReader reader = new CSVReader(csv);
		for (int i = 3; i < reader.Row; i++)
		{
			Maps_Demo item = new Maps_Demo();
			item.ID = reader.GetData(0, i);
			item.SuperName = reader.GetData(1, i);
			item.SceneName = reader.GetData(2, i);
			item.Time = ParseVector3(reader.GetData(3, i));
			item.RolesID = reader.GetData(4, i);
			item.RolesLevel = reader.GetData(5, i);
			item.Gold = reader.GetData(6, i);
			item.BuySkillCostGold = int.Parse(reader.GetData(7, i));
			item.IsBoss = bool.Parse(reader.GetData(8, i));
			item.FirstPassGoods = reader.GetData(9, i);
			item.FirstKillGoodsNum = reader.GetData(10, i);
			vec.Add(item);
		}
		return vec;
	}
	
		[Obsolete]
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
			item.SceneName = reader.GetData(2, i);
			item.Time = ParseVector3(reader.GetData(3, i));
			item.RolesID = reader.GetData(4, i);
			item.RolesLevel = reader.GetData(5, i);
			item.Gold = reader.GetData(6, i);
			item.BuySkillCostGold = int.Parse(reader.GetData(7, i));
			item.IsBoss = bool.Parse(reader.GetData(8, i));
			item.FirstPassGoods = reader.GetData(9, i);
			item.FirstKillGoodsNum = reader.GetData(10, i);
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
