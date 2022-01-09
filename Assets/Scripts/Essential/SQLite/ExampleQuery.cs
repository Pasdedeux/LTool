//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace SQLite.Demos
//{
//	using UnityEngine;
//	using UnityEngine.UI;
//	using System.Collections.Generic;
//    using System.Diagnostics;
//	using Debug = UnityEngine.Debug;
//    public class ExampleQuery : MonoBehaviour
//	{
//		public SQLite.SQLManager dbManager;

//		public Text outputText;
//        private void OnGUI()
//        {
//   //         if (GUILayout.Button("TEST1", GUILayout.Width(100), GUILayout.Height(100)))
//   //         {
//			//	Test();

//			//}

//			//if (GUILayout.Button("TEST2", GUILayout.Width(100), GUILayout.Height(100)))
//			//{
//			//	Test2();

//			//}

//			//if (GUILayout.Button("TEST3", GUILayout.Width(100), GUILayout.Height(100)))
//			//{
//			//	Test3();

//			//}

//			//if (GUILayout.Button("TEST4", GUILayout.Width(100), GUILayout.Height(100)))
//			//{
//			//	Test4();

//			//}

//			if (GUILayout.Button("TEST5", GUILayout.Width(100), GUILayout.Height(100)))
//			{
//				Test5();
//			}
//			if (GUILayout.Button("TEST6", GUILayout.Width(100), GUILayout.Height(100)))
//			{
//				Test6();
//			}
//			if (GUILayout.Button("TEST7", GUILayout.Width(100), GUILayout.Height(100)))
//			{
//				Test7();
//			}
//			if (GUILayout.Button("TEST8", GUILayout.Width(100), GUILayout.Height(100)))
//			{
//				Test8();
//			}
//		}

//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//				Test6();
//            }
//        }
//        void Test()
//		{
//			var attrList = dbManager.Table<ActorAttrConfig>();

//			outputText.text = "ActorAttrConfig\n\n";
//			foreach (var tData in attrList)
//			{
//				outputText.text += "<color=#1abc9c>ID</color>: '" + tData.ID + "' " +
//									"<color=#1abc9c>Name</color>:" + tData.Name.ToString() + " " +
//									"<color=#1abc9c>Des</color>:" + tData.Des.ToString() + " " + "\n";
//			}
//		}

//		void Test2()
//        {

//			var attrList = dbManager.Table<PlayerData>();
//			outputText.text = "PlayerData\n\n";
//			foreach (var t in attrList)
//			{
//				outputText.text += "<color=#1abc9c>ID</color>: '" + t.ID + "' " +
//									"<color=#1abc9c>ATK</color>:" + t.Name.ToString() + " " +
//									"<color=#1abc9c>Des</color>:" + t.SP.ToString() + " " + 
//									"<color=#1abc9c>Des</color>:" + t.ATK + " " +
//									"<color=#1abc9c>Des</color>:" + t.Level + " " +
//									"<color=#1abc9c>Des</color>:" + t.SceneData + " "
//									+ "\n";
//			}

//			outputText.text += " select id == 1004 data : \n";
//			//public int LocationID { get; set; }
//			//public string LocationName { get; set; }
//			var tData = from p in dbManager.Table<PlayerData>()
//						where p.ID == 1004
//						select p;

//            foreach (var kv in tData)
//            {
//				outputText.text += "<color=#1abc9c>LocationID</color>: '" + kv.Name + "' " +
//									"<color=#1abc9c>Damage</color>:" + kv.Level +"\n";
//			}
//        }

//		void Test3()
//        {
//			//List<ActorAttrConfig> attrs = new List<ActorAttrConfig>();
//			//attrs.Add(new ActorAttrConfig(){
//			//	ID = 101,
//			//	Name = "XXX",
//			//	Des = "123123"
//			//});
//			//attrs.Add(new ActorAttrConfig()
//			//{
//			//	ID = 102,
//			//	Name = "X1",
//			//	Des = "121"
//			//});
//			//attrs.Add(new ActorAttrConfig()
//			//{
//			//	ID = 103,
//			//	Name = "X2",
//			//	Des = "124"
//			//});

//			//dbManager.InsertAll(attrs);
//		}

//		void Test4()
//		{
//			//public int LocationID { get; set; }
//			//public string LocationName { get; set; }
//			List<PlayerData> datas = new List<PlayerData>();
//			datas.Add(new PlayerData()
//			{
//				ID = 1001,
//				ATK = 10,
//				SP = 10,
//				Name = "小明",
//				Level = 1,
//				SceneData = "@1_Sceneconfig_101_true = [1-2]"
//			});

//			datas.Add(new PlayerData()
//			{
//				ID = 1002,
//				ATK = 30,
//				SP = 30,
//				Name = "小Y",
//				Level = 1,
//				SceneData = "@1_Sceneconfig_101_true = [3-2]"
//			});

//			datas.Add(new PlayerData()
//			{
//				ID = 1004,
//				ATK = 12,
//				SP = 14,
//				Name = "小Q",
//				Level = 5,
//				SceneData = "@1_Sceneconfig_101_true = [1-2]"
//			});
//            //         foreach (var item in datas)
//            //         {
//            //	dbManager.UpdateTable(item);
//            //}
//            dbManager.InsertAll(datas);
//        }

//		void Test5()
//		{
//			//var tCmd = dbManager.CreateCommand("select Des from ActorAttrConfig where ID = 101");
//			//var tStr = tCmd.ExecuteScalar<string>();

//			//string tStr2 = dbManager.ExcurteScalar<string>("select Des from ActorAttrConfig where ID = 101");

//			//Debug.Log(tStr);
//			//Debug.Log(tStr2);

//			//var sIDs = new List<int>();
//			//var tData = SQLManager.Ins.QueryGeneric("select g.ID from ActorAttrConfig g order by g.ID").rows;
//			//for (int t = 0; t < tData.Count; t++)
//			//{
//			//	Debug.Log((int)tData[t][0]);
//			//}

//			//bool tOK = false;
//			//var tObj = SQLManager.Ins.QueryFirstRecord<ActorAttrConfig>(out tOK, string.Format(CSVTable.SINGLE_ROW, "ActorAttrConfig", 101));

//			//Debug.Log(tObj.ID + "_" + tObj.Name + "_" + tObj.Des);


//			//CSVCPConfig a = new CSVCPConfig();
//			//var tt = a[101];

//			//var t = string.Format("{0}  + {1}  = {0} + {2}", 4, 3, 3);
//			//Debug.Log(t);


//			string path = Application.streamingAssetsPath + "/csv/csvconfigs.db";
//			_db = new SQLiteConnection(path);
//			string query = "create table TaskConfig2("
//				+ "\"ID\" integer primary key not null,"
//				+ "\"Index\" integer,"
//				+ "\"Name\" integer,"
//				+ "\"Des\" integer,"
//				+ "\"Type\" integer,"
//				+ "\"CountType\" integer,"
//				+ "\"Value\" varchar(140),"
//				+ "\"AllPro\" integer,"
//				+ "\"RewardID\" varchar(140),"
//				+ "\"Panel\" varchar(140)"
//				+ ")";

//			//_db.CreateCommand(query);
//			//_db.Execute(query);
//			long rowID;
//			//_db.Insert(new PlayerData()
//			//{
//			//	ID = 10001,
//			//	ATK = 10002,
//			//	SP = 10003,
//			//	Name = "XX",
//			//	Level = 1,
//			//	SceneData = "level1_50%"
//			//},out rowID);
//			query = "insert  into \"TaskConfig\"(\"ID\",\"Index\",\"Name\",\"Des\",\"Type\",\"CountType\") values (111,112,113,\"114\",2,\"115\")";
//			query = "create table \"TaskConfig3\"(\"ID\" integer primary key not null,\"Index\" integer ,\"Name\" integer ,\"Des\" integer ,\"Type\" integer ,\"CountType\" integer ,\"Value\" varchar(140) ,\"AllPro\" integer ,\"RewardID\" varchar(140) ,\"Panel\" varchar(140))";
//			query = "insert into \"TaskConfig3\"(\"ID\", \"Index\", \"Name\", \"Des\", \"Type\", \"CountType\", \"Value\", \"AllPro\", \"RewardID\", \"Panel\") values(70001, 0, 70001, 75001, 1, 1, \"11001\", 1, \"1000|100;1001|5\", \"FitmentPopUp\")";
//			query = "drop table if exists \"LanguageConfig\"";
//			//_db.Execute(query);
//			query = "create table \"LanguageConfig\"(\"ID\" integer primary key not null,\"CN\" TEXT)";
//			//_db.Execute(query);
//			query = "insert into \"LanguageConfig\"(\"ID\",\"CN\") values (?,?)";
//            //List<object> args = new List<object>();
//            //args.Add(4);
//            //args.Add("钱");
//            //var cmd = _db.Execute(query, args.ToArray());
//            //         //_db.Execute(query);

//            //         SQLite3.Result rs = SQLite3.Result.Done;
//            //         string errormsg = "";
//            //         //int rsscmd = cmd.ExecuteNonQuery(out rs, out errormsg);

//            //         Debug.Log(rs + "\n" + errormsg + "\n" + cmd);


//            Configs.Install();
//            //Debug.Log(Configs.sTaskConfig.Count);
//            //var tIDs = Configs.sTaskConfig.IDs;
//            //for (int i = 0; i < tIDs.Count; i++)
//            //{
//            //    var tTmper = Configs.sTaskConfig[tIDs[i]];

//            //    Debug.Log(tTmper.ID
//            //        + "_" + tTmper.Index
//            //        + "_" + tTmper.Name
//            //        + "_" + tTmper.Des
//            //        + "_" + tTmper.Type
//            //        + "_" + tTmper.CountType
//            //        + "_" + tTmper.Value[0]
//            //        + "_" + tTmper.AllPro
//            //        + "_" + tTmper.RewardID.Count
//            //        + "_" + tTmper.Panel
//            //        + "_" + tTmper.Vector3.ToString()
//            //    );
//            //}

//            //Debug.Log(Configs.sActorAttrConfig[Configs.sActorAttrConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sActorConfig[Configs.sActorConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sActorSkinBaseConfig[Configs.sActorSkinBaseConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sActorSkinUpConfig[Configs.sActorSkinUpConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sBaseEffectConfig[Configs.sBaseEffectConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sClassConfig[Configs.sClassConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sCollectionConfig[Configs.sCollectionConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sCPConfig[Configs.sCPConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sCrossTalkConfig[Configs.sCrossTalkConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sCrossTalkElementsConfig[Configs.sCrossTalkElementsConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sCrossTalkGame1[Configs.sCrossTalkGame1.IDs[0]].ToString());
//            //Debug.Log(Configs.sEmployeeConfig[Configs.sEmployeeConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sEmployeeUpConfig[Configs.sEmployeeUpConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sFeatureConfig[Configs.sFeatureConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sFitmentBaseConfig[Configs.sFitmentBaseConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sFitmentConfig[Configs.sFitmentConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sFood[Configs.sFood.IDs[0]].ToString());
//            //Debug.Log(Configs.sFoodUpConfig[Configs.sFoodUpConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sFuncOpenConfig[Configs.sFuncOpenConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sLanguageConfig[Configs.sLanguageConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sLingShiGameConfig[Configs.sLingShiGameConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sLingShiGameItemConfig[Configs.sLingShiGameItemConfig.IDs[0]].ToString());
//            ////Debug.Log(Configs.sLiveShowConfig[Configs.sLiveShowConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sMoneyConfig[Configs.sMoneyConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sNoviceGuideConfig[Configs.sNoviceGuideConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sNPCGuestConfig[Configs.sNPCGuestConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sNPCSolicterConfig[Configs.sNPCSolicterConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sNPCTable[Configs.sNPCTable.IDs[0]].ToString());
//            //Debug.Log(Configs.sOtherDialogue[Configs.sOtherDialogue.IDs[0]].ToString());
//            //Debug.Log(Configs.sOtherDialogueGroups[Configs.sOtherDialogueGroups.IDs[0]].ToString());
//            //Debug.Log(Configs.sProgramConfig[Configs.sProgramConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sPunchlineDialogue[Configs.sPunchlineDialogue.IDs[0]].ToString());
//            //Debug.Log(Configs.sRoomConfig[Configs.sRoomConfig.IDs[0]].ToString());
//            ////Debug.Log(Configs.sSpawnConfig[Configs.sSpawnConfig.IDs[0]].ToString());
//            //Debug.Log(Configs.sStarDialogue[Configs.sStarDialogue.IDs[0]].ToString());
//            //Debug.Log(Configs.sStarReadyDialogue[Configs.sStarReadyDialogue.IDs[0]].ToString());
//            //Debug.Log(Configs.sTaskConfig[Configs.sTaskConfig.IDs[0]].ToString());
//        }

		
//		void Test6()
//        {
//            //Configs.Install();
//            var tList = Configs.sLanguageConfig.IDs;

//            var tWatch = Stopwatch.StartNew();

//			//List<string> laung = new List<string>();
//            for (int i = 0; i < 1411; i++)
//            {
//				var tstr = string.Format(SQLReader.SINGLE_ROW, "LanguageConfig", i);
//                //var tList = Configs.sLanguageConfig.IDs;

//                var tStr = Configs.sLanguageConfig[tList[i]].CN;
//                //laung.Add(tStr);
//            }

//			tWatch.Stop();
//			Debug.Log("Read languge " + 1411 + "__use time : " + tWatch.ElapsedMilliseconds);
//		}

//		void Test7()
//        {
//			string path = Application.streamingAssetsPath + "/csv/test.bytes";
//			_db = new SQLiteConnection(path);
//			_db.CreateTable<PlayerDataExample>();
//		}
//		void Test8()
//        {
//            if (_db == null)
//            {
//                string path = Application.streamingAssetsPath + "/csv/csvconfigs.bytes";
//                _db = new SQLiteConnection(path);
//            }
//            //long aRow;
//            ////_db.Insert(new PlayerDataExample() { 
//            ////	ID = 10001,
//            ////	Level = 4,
//            ////	Name = "123",
//            ////	_Items = new List<int>() { 222, 1, 1, 122 }
//            ////},out aRow);;

//            //_db.Update(new PlayerDataExample()
//            //{
//            //	ID = 10001,
//            //	Level = 4,
//            //	Name = "123",
//            //	_Items = new List<int>() { 222, 1, 1, 122 }
//            //}); ;

//            //SQList<QData> list = new SQList<QData>();
//            //list.Add(new QData() { a = 1 });
//            //list.Add(new QData() { a = 2 });
//            //list.Add(new QData() { a = 3 });
//            //list.Add(new QData() { a = 4 });
//            //list.Add(new QData() { a = 5 });
//            //list.Add(new QData() { a = 6 });

//            //         for (int i = 0; i < list.Count; i++)
//            //         {
//            //	Debug.Log(list[i].a);
//            //         }
//            //Debug.Log("set value: !!!:");
//            //list[2].a = 100;

//            //         for (int i = 0; i < list.Count; i++)
//            //         {
//            //             Debug.Log(list[i].a);
//            //         }
//            //QData a = new QData();
//            //a.a = 10;
//            //var tLog = PropertyLogsHelper.GetPropertyLogs<QData>(a);
//            //Debug.Log(a.GetPropertyLogs<QData>());

//        }
//		SQLiteConnection _db;
//		private void OnApplicationQuit()
//        {
//            if(_db != null)
//            {
//				_db.Close();
//				_db.Dispose();
//            }
//        }
//    }
 
//}
//[PropertyChangeTracking]
//public class QData
//{
//	public int a ;
//}
//public class SQList<T> : System.Collections.Generic.List<T>
//{
//	new public  T this[int index] {
//        get
//        {
//			return base[index];
//        }
//        set
//        {
//			base[index] = value;
//			Debug.LogFormat("===> set {0}", value.ToString());
//        }
//	}
//	new public void Add(T item)
//    {
//		base.Add(item);
//    }
//	new public void AddRange(IEnumerable<T> collection)
//    {
//		base.AddRange(collection);
//    }

//	new public void Insert(int index, T item)
//    {
//		base.Insert(index, item);
//    }
//	new public void InsertRange(int index, IEnumerable<T> collection)
//    {
//		base.InsertRange(index, collection);
//    }
	
//	new public bool Remove(T item)
//    {
//		return base.Remove(item);
//    }
//	new public int RemoveAll(Predicate<T> match)
//	{
//		return base.RemoveAll(match);
//	}

//	new public void RemoveAt(int index)
//    {
//		base.RemoveAt(index);
//    }
//	new public void RemoveRange(int index, int count)
//    {
//		base.RemoveRange(index, count);
//    }


//}
