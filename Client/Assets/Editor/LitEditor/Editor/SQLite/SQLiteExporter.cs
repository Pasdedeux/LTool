using ExcelDataReader;
using Litframework.ExcelTool;
using LitFramework;
using LitFramework.LitTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Litframework.ExcelTool
{

    internal partial class ExcelExport
    {
        public static void XlsxToSQLite(int platform, string extralFileStr = _defaultExtralFileType)
        {
            SQLiteWriter sqliteWriter = null, sqliteSqlWriter = null;

            _exportFunc1 = csvpath =>
            {
                if(platform<2)
                {
                    sqliteWriter = new SQLiteWriter(csvpath);
                    sqliteWriter.Opendb();
                }
               

                if (platform > 0)
                {
                    sqliteSqlWriter = new SQLiteWriter(SERVER_CSV_OUT_DIR);
                    sqliteSqlWriter.Opendb();
                }
            };
            _exportFunc2 = (csvpath, NextFile, csOutPath, cnt) =>
            {
                string csvfile = XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                CSVReader reader = new CSVReader(csvfile);
                string tTableName = NextFile.Name.Split('.')[0];

                var titleFlag = reader.GetRow(0);
                //首列key标记
                var firstKeyFlag = titleFlag[0].ToLower();
                //如果首列配置为#则不进行后续操作
                if (firstKeyFlag.StartsWith("#-")) return;

                //客户端生成对应文件
                if (platform < 2 && !firstKeyFlag.StartsWith("s-"))
                {
                    reader = new CSVReader(XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), PlatformType.Client));
                    sqliteWriter.Write(tTableName, reader);
                }

                //服务器生成对应文件
                if (platform >0 && !firstKeyFlag.StartsWith("c-"))
                {
                    reader = new CSVReader(XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), PlatformType.Server));
                    sqliteSqlWriter.Write(tTableName, reader);
                }
            };
            _exportFunc3 = csvpath =>
            {
                if (platform < 2) sqliteWriter.Closedb();
                if (platform > 0) sqliteSqlWriter.Closedb();

                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });
            };
            _exportFunc4 = e =>
            {
                if (platform < 2) sqliteWriter.Closedb();
                if (platform > 0) sqliteSqlWriter.Closedb();
            };

            Template_xlsx_2_csv(false, platform, extralFileStr);
        }


        public static void XlsxToSQLiteCs(bool useHotFix, int platform, string extralFileStr = _defaultExtralFileType)
        {
            SQLiteWriter sqliteWriter = null, sqliteSqlWriter = null;

            _exportFunc1 = csvpath =>
            {
                if (platform < 2)
                {
                    sqliteWriter = new SQLiteWriter(csvpath);
                    sqliteWriter.Opendb();
                }

                if (platform > 0)
                {
                    sqliteSqlWriter = new SQLiteWriter(SERVER_CSV_OUT_DIR);
                    sqliteSqlWriter.Opendb();
                }
            };
            _exportFunc2 = (csvpath, NextFile,  csOutPath, cnt) =>
            {
                var tTableName = NextFile.Name.Split('.')[0];
                string csvfile = XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                //将excel写入csvconfigs.bytes
                CSVReader reader = new CSVReader(csvfile);

                var titleFlag = reader.GetRow(0);
                //首列key标记
                var firstKeyFlag = titleFlag[0].ToLower();
                //如果首列配置为#则不进行后续操作
                if (firstKeyFlag.StartsWith("#-")) return;

                string csString = null;
                //客户端生成对应文件
                if (platform < 2 && !firstKeyFlag.StartsWith("s-"))
                {
                    reader = new CSVReader(XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), PlatformType.Client));
                    csString = new SQLParser().CreateCS(tTableName, csvfile, PlatformType.Client);
                    CreateCSFile(csOutPath, tTableName + ".cs", csString);

                    sqliteWriter.Write(tTableName, reader);

                    cnt.configsClientNameList.Add(tTableName, reader.GetData(0, 2));
                }

                //服务器生成对应文件
                if (platform > 0 && !firstKeyFlag.StartsWith("c-"))
                {
                    reader = new CSVReader(XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite), PlatformType.Server));
                    csString = new SQLParser().CreateCS(tTableName, csvfile, PlatformType.Server);
                    CreateCSFile(SERVER_CS_OUT_DIR, tTableName + ".cs", csString);
                    
                    sqliteSqlWriter.Write(tTableName, reader);
                    cnt.configsServerNameList.Add(tTableName, reader.GetData(0, 2));
                }

            };
            _exportFunc3 = csvpath =>
            {
                if (platform < 2) sqliteWriter.Closedb();
                if (platform > 0) sqliteSqlWriter.Closedb();

                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });
            };
            _exportFunc4 = e =>
            {
                if (platform < 2) sqliteWriter.Closedb();
                if (platform > 0) sqliteSqlWriter.Closedb();
            };
            _exportFunc5 = (useHotFix, useServer, cnt) =>
            {
                //============更新并保存CS============//
                string createdCS = null;
                //Client
                if (useServer < 2)
                {
                    createdCS = new SQLConfigsParse().CreateCS(cnt, PlatformType.Client);
                    if (!useHotFix)
                        CreateCSFile(CONFIG_CS_OUTPUT_DIR, CS_CONFIGS, createdCS);
                    else
                        CreateCSFile(CONFIG_CS_HOTFIX_OUTPUT_DIR, CS_CONFIGS, createdCS);
                }

                //Server
                if (useServer > 0)
                {
                    createdCS = new SQLConfigsParse().CreateCS(cnt, PlatformType.Server);
                    CreateCSFile(SERVER_CONFIGS_OUT_DIR, CS_CONFIGS, createdCS);
                }
            };

            Template_xlsx_2_csv(useHotFix, platform, extralFileStr);
        }
    }


    /// <summary>
    /// SQL代码模板
    /// </summary>
    class SQLConfigsParse : IConfigsParse
    {
        public List<string> CSString = new List<string>();

        public string CreateCS(ConfigsNamesTemplate rpt, PlatformType platformType)
        {
            AddHead();
            AddBody(rpt, platformType);
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
        public void AddBody(ConfigsNamesTemplate rpt, PlatformType platformType)
        {
            Dictionary<string, string> configNameList = platformType == PlatformType.Client ? rpt.configsClientNameList : rpt.configsServerNameList;

            foreach (var item in configNameList)
            {
                CSString.Add(string.Format("public static {0} {0}Dict;", item.Key));
            }

            CSString.Add(string.Format("public static void Install()"));
            CSString.Add("{");
            foreach (var item in configNameList)
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