﻿#region << 版 本 注 释 >>
///*----------------------------------------------------------------
// Author : Derek Liu
// 备注：由模板工具自动生成
///----------------------------------------------------------------*/
#endregion

//*******************************************************************
//**                  该类由工具自动生成，请勿手动修改                   **
//*******************************************************************

using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using SQLite;
using LitFramework;
public class SpawnConfig
{
	private const string TName = "SpawnConfig";
	private int _count = -1;
	public  int  Count {
		get {
			if(_count == -1) {
				_count = SQLManager.Instance.ExcurteScalar<int>(string.Format(SQLReader.COUNT,TName));
			}
			return _count;
		}
	}
	private List<int> _keys;
	public  List<int> Keys {
		get {
			if(_keys == null) {
				_keys = new List<int>();
				var tList = SQLManager.Instance.QueryGeneric(string.Format(SQLReader.SELECT_CLOWS_ORDERBY,"ID",TName)).rows;
				for(int i = 0; i < tList.Count; i++){
					_keys.Add((int)tList[i][0]);
				}
			}
			return _keys;
		}
	}
	private Dictionary<int,SpawnConfigRow> _table;
	public SpawnConfigRow this[int index] {
		get {
			if(_table == null){
				_table = new Dictionary<int,SpawnConfigRow>();
			}
			SpawnConfigRow tObj;
			if(!_table.TryGetValue(index,out tObj)){
				tObj = new SpawnConfigRow(index);
				_table[index] = tObj;
			}
			return tObj;
		}
	}
	
	/// <summary>
	/// 自定义查找语句
	/// </summary>
	/// <param name="queryWhere">基础where语句，查找值需要以?代替。如"ID >? and PreloadAmount <?</ param > 
	/// <param name="args">需要将查找的“值”按照查询语句 “？”顺序填充</param>
	///<returns></returns>
	public List<int> QueryBy(string queryWhere, params object[] args)
	{
		var queryCommand = string.Format(SQLReader.SELECT_ID_WHERE, "ID", TName, queryWhere);
		var tList = SQLManager.Instance.QueryGeneric(queryCommand, args).rows;
		
		List<int> result = new List<int>(tList.Count);
		for (int i = 0; i < tList.Count; i++)
		{
			result.Add((int)tList[i][0]);
		}
		return result;
	}
	
	#region Row
	public class SpawnConfigRow
	{
		public SpawnConfigRow( int index )
		{
			ID = index;
			PreloadAmount = SQLManager.Instance.ExcurteScalar<int>(string.Format(SQLReader.SINGLE, "PreloadAmount", TName, ID, "ID"));;
		}
		/// <summary>
		/// 预制件ID
		/// </summary>
		public int ID {get;set;}
		private string sSpawnType;
		/// <summary>
		/// 预制件所属分类
		/// </summary>
		public string SpawnType { 
			get {
				if(string.IsNullOrEmpty(sSpawnType)){
					sSpawnType = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,"SpawnType",TName,ID,"ID"));
				}
				return sSpawnType;
			}
		}
		private string sresPath;
		/// <summary>
		/// Resource资源路径
		/// </summary>
		public string resPath { 
			get {
				if(string.IsNullOrEmpty(sresPath)){
					sresPath = SQLManager.Instance.ExcurteScalar<string>(string.Format(SQLReader.SINGLE,"resPath",TName,ID,"ID"));
				}
				return sresPath;
			}
		}
		/// <summary>
		/// 初始数量
		/// </summary>
		public int PreloadAmount {get;set;}
		
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
		public override string ToString(){
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("ID={0}" ,ID.ToString()));
			sb.AppendLine(string.Format("SpawnType={0}" ,SpawnType.ToString()));
			sb.AppendLine(string.Format("resPath={0}" ,resPath.ToString()));
			sb.AppendLine(string.Format("PreloadAmount={0}" ,PreloadAmount.ToString()));
			return sb.ToString();
		}
		
	}
	#endregion
	
}
