using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using Litframework.ExcelTool;

namespace Litframework.ExcelTool
{

    public class SQLiteWriter
    {
        private string sPath;
        public SQLiteWriter(string aPath)
        {
            sPath = aPath + "/" + db;
        }
        //共用一个DB
        public const string db = "csvconfigs.bytes";
        public const string droptable = "drop table if exists \"{0}\"";
        public const string create = "create table \"{0}\"(\n{1}\n)";
        public const string insert = "insert into \"{0}\"({1}) values ({2})";
        private SQLiteConnection _db;

        public void Opendb()
        {
            if (_db == null)
            {
                _db = new SQLiteConnection(sPath);
                _db.BeginTransaction();
#if UNITY_EDITOR
                LDebug.Log($"配置表数据库链接成功 >>{sPath}");
#endif
            }
        }
        public void Write(string aTableName, CSVReader aReader)
        {

            _db.Execute(string.Format(droptable, aTableName));

            //create table;

            List<string> queryParams = new List<string>();

            for (int i = 0; i < aReader.Colume; i++)
            {
                string tt = aReader.GetData(i, 1);
                string th = aReader.GetData(i, 2);

                string decl = "\"" + tt + "\" " + SqlType(th) + " ";
                if (tt.ToLower() == "id")
                {
                    decl += "primary key not null";
                }
                queryParams.Add(decl);
            }
            string headerJoin_fix = string.Join(",\n", queryParams.ToArray());
            string query = string.Format(create, aTableName, headerJoin_fix);

            //Debug.Log(query);

            _db.Execute(query);

            queryParams.Clear();
            for (int x = 0; x < aReader.Colume; x++)
            {
                string tt = aReader.GetData(x, 1);
                string th = aReader.GetData(x, 2);
                string decl = "\"" + tt + "\"";
                queryParams.Add(decl);
            }
            string headerJoin = string.Join(",", queryParams.ToArray());

            queryParams.Clear();
            List<object> paramsObj = new List<object>();

            for (int y = 3; y < aReader.Row; y++)
            {
                for (int x = 0; x < aReader.Colume; x++)
                {
                    string th = aReader.GetData(x, 2);
                    string tvalue = aReader.GetData(x, y);
                    paramsObj.Add(ParamsType(th, tvalue));
                    queryParams.Add("?");
                }
                string valueJoin = string.Join(",", queryParams.ToArray());
                query = string.Format(insert, aTableName, headerJoin, valueJoin);


                //Debug.Log(query);

                _db.Execute(query, paramsObj.ToArray());
                queryParams.Clear();
                paramsObj.Clear();
            }
        }

        public void Closedb()
        {
            if (_db != null)
            {
                _db.Commit();
                _db.Close();
                _db.Dispose();
                _db = null;
            }
        }

        static string SqlType(string aProperTP)
        {
            var tProperTP = aProperTP.ToLower().Split('<')[0];
            switch (tProperTP)
            {
                case "bool":
                case "byte":
                case "int":
                case "uint":
                case "short":
                    {
                        return "integer";
                    }
                case "long":
                case "ulong":
                    {
                        return "bigint";
                    }
                case "float":
                case "double":
                    {
                        return "float";
                    }
                case "list":
                case "dic":
                case "dictionary":
                case "vector3":
                    {
                        return "varchar(140)";
                    }
                case "char":
                case "string":
                    {
                        return "varchar(140)";
                    }
                case "datatime":
                    {
                        return "varchar(140)";
                    }
            }
            return "varchar(140)";
        }

        static object ParamsType(string aProperTP, string aValue)
        {
            var tProperTP = aProperTP.ToLower().Split('<')[0];
            switch (tProperTP)
            {
                case "bool":
                    {
                        return bool.Parse(aValue);
                    }
                case "byte":
                case "int":
                case "uint":
                case "short":
                    {
                        return int.Parse(aValue);
                    }
                case "long":
                case "ulong":
                    {
                        return long.Parse(aValue);
                    }
                case "float":
                case "double":
                case "decimal":
                    {
                        return float.Parse(aValue);
                    }
                case "list":
                case "dic":
                case "dictionary":
                case "vector3":
                case "char":
                case "string":
                case "datatime":
                    {
                        return aValue;
                    }
            }
            return aValue;
        }
    }

}