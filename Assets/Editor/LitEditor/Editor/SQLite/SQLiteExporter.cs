using ExcelDataReader;
using Litframework.ExcelTool;
using LitFramework;
using LitFramework.LitTool;
using LitFrameworkEditor.EditorExtended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Litframework.ExcelTool
{

    internal partial class ExcelExport
    {
        public static void XlsxToSQLite(string extralFileStr = _defaultExtralFileType)
        {
            SQLiteWriter sqliteWriter = null;

            _exportFunc1 = csvpath => {
                sqliteWriter = new SQLiteWriter(csvpath);
                sqliteWriter.Opendb();
            };
            _exportFunc2 = (csvpath, NextFile, csvfile, csOutPath, cnt) =>
            {
                string tTableName = NextFile.Name.Split('.')[0];
                CSVReader reader = new CSVReader(csvfile);
                sqliteWriter.Write(tTableName, reader);
            };
            _exportFunc3 = csvpath => 
            {
                sqliteWriter.Closedb();
                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });
            };
            _exportFunc4 = e =>
            {
                sqliteWriter.Closedb();
            };

            Template_xlsx_2_csv(false, extralFileStr);
        }


        public static void XlsxToSQLiteCs(bool useHotFix, string extralFileStr = _defaultExtralFileType)
        {
            SQLiteWriter sqliteWriter = null;
            _exportFunc1 = csvpath => {
                sqliteWriter = new SQLiteWriter(csvpath);
                sqliteWriter.Opendb();
            };
            _exportFunc2 = (csvpath, NextFile, csvfile, csOutPath, cnt) =>
            {
                SQLParser cp = new SQLParser();
                CreateCSFile(csOutPath, NextFile.Name.Split('.')[0] + ".cs", cp.CreateCS(NextFile.Name.Split('.')[0], csvfile));

                //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                //将excel写入csvconfigs.bytes
                CSVReader reader = new CSVReader(csvfile);
                sqliteWriter.Write(NextFile.Name.Split('.')[0], reader);

                cnt.configsNameList.Add(NextFile.Name.Split('.')[0], reader.GetData(0, 2));
            };
            _exportFunc3 = csvpath =>
            {
                sqliteWriter.Closedb();
                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });
            };
            _exportFunc4 = e =>
            {
                sqliteWriter.Closedb();
            };
            _exportFunc5 = (useHotFix, cnt) =>
            {
                //============更新并保存CS============//
                IConfigsParse rpp = new SQLConfigsParse();

                if (!useHotFix)
                    CreateCSFile(CONFIG_CS_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
                else
                    CreateCSFile(CONFIG_CS_HOTFIX_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
            };

            Template_xlsx_2_csv(useHotFix, extralFileStr);
        }
    }


    /// <summary>
    /// SQL代码模板
    /// </summary>
    class SQLConfigsParse : IConfigsParse
    {
        public List<string> CSString = new List<string>();

        public string CreateCS(ConfigsNamesTemplate rpt)
        {
            AddHead();
            AddBody(rpt);
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        public void AddHead()
        {
            CSString.Add("#region << 版 本 注 释 >>");
            CSString.Add("///*----------------------------------------------------------------");
            CSString.Add("// Author : Derek Liu");
            CSString.Add("// 备注：由模板工具自动生成");
            CSString.Add("///----------------------------------------------------------------*/");
            CSString.Add("#endregion");
            CSString.Add("");
            CSString.Add("//*******************************************************************");
            CSString.Add("//**                  该类由工具自动生成，请勿手动修改                   **");
            CSString.Add("//*******************************************************************");
            CSString.Add("");
            CSString.Add("using LitFramework;");
            CSString.Add("using System.Linq;");
            CSString.Add("using System.Collections.Generic;");
            CSString.Add("public static partial class Configs");
            CSString.Add("{");
        }
        public void AddTail()
        {
            CSString.Add("}");
        }
        public void AddBody(ConfigsNamesTemplate rpt)
        {
            foreach (var item in rpt.configsNameList)
            {
                CSString.Add(string.Format("public static {0} {0}Dict;", item.Key));
            }

            CSString.Add(string.Format("public static void Install()"));
            CSString.Add("{");
            foreach (var item in rpt.configsNameList)
            {
                CSString.Add(string.Format("{0}Dict = new {0}();", item.Key));
            }
            CSString.Add("}");
        }
        public string GetFomatedCS()
        {
            StringBuilder result = new StringBuilder();
            int tablevel = 0;
            for (int i = 0; i < CSString.Count; i++)
            {
                string tab = "";

                for (int j = 0; j < tablevel; ++j)
                    tab += "\t";

                if (CSString[i].Contains("{"))
                    tablevel++;
                if (CSString[i].Contains("}"))
                {
                    tablevel--;
                    tab = "";
                    for (int j = 0; j < tablevel; ++j)
                        tab += "\t";
                }

                result.Append(tab + CSString[i] + "\n");
            }
            return result.ToString();
        }
    }

}