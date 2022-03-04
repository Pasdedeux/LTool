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
            CombinePath();

            string xlsxpath = XLSX_ORI_DIR;
            string streampath = STREAM_OUT_DIR;
            string csvpath = CSV_OUTPUT_DIR;

            _csvListToBeRestored.Clear();
            //文件列表
            DirectoryInfo TheFolder = new DirectoryInfo(xlsxpath);

            if (!Directory.Exists(csvpath))
            {
                Directory.CreateDirectory(csvpath);
            }

            //============================
            SQLiteWriter sqliteWriter = new SQLiteWriter(csvpath);
            sqliteWriter.Opendb();

            try
            {
                //对文件进行遍历
                foreach (var NextFile in TheFolder.GetFiles())
                {
                    if (Path.GetExtension(NextFile.Name) == ".xlsx" && !NextFile.Name.StartsWith("~$"))
                    {
                        string csvfile = XLSXTOCSV(NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));


                        //======================
                        string tTableName = NextFile.Name.Split('.')[0];
                        CSVReader reader = new CSVReader(csvfile);
                        sqliteWriter.Write(tTableName, reader);

                    }
                    else if (Path.GetExtension(NextFile.Name) == ".txt")
                    {
                        FileInfo fi = new FileInfo(csvpath + "/" + NextFile.Name);
                        if (fi.Exists)
                            fi.Delete();
                        NextFile.CopyTo(csvpath + "/" + NextFile.Name);

                        _csvListToBeRestored.Add(new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 });
                    }
                }

                //======================
                sqliteWriter.Closedb();
                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvpath + "/" + "csvconfigs.bytes"), Version = 1 });


                //遍历框架配置的额外后缀文件
                string[] extralFile = extralFileStr.Split('|');
                foreach (var item in extralFile)
                {
                    if (item.Equals("csv")) continue;

                    GetFiles(new DirectoryInfo(streampath), item, _csvListToBeRestored);
                }
            }
            catch (Exception e)
            {
                //======================
                sqliteWriter.Closedb();
                
                throw new Exception(e.Message);
            }
            finally
            {
                //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
                MatchCSVTotalFile(_csvListToBeRestored);
            }
        }


        public static void XlsxToSQLiteCs(bool useHotFix, string extralFileStr = _defaultExtralFileType)
        {
            CombinePath();

            _csvListToBeRestored.Clear();
            string xlsxpath = XLSX_ORI_DIR;
            string streampath = STREAM_OUT_DIR;
            string csvOutPath = CSV_OUTPUT_DIR;
            string csOutPath;

            if (!useHotFix)
                csOutPath = CS_OUTPUT_DIR;
            else
                csOutPath = CS_HOTFIX_OUTPUT_DIR;

            DirectoryInfo theXMLFolder = new DirectoryInfo(xlsxpath);

            if (!Directory.Exists(csvOutPath))
            {
                Directory.CreateDirectory(csvOutPath);
            }
            if (!Directory.Exists(csOutPath))
            {
                Directory.CreateDirectory(csOutPath);
            }

            //============================
            SQLiteWriter sqliteWriter = new SQLiteWriter(csvOutPath);
            sqliteWriter.Opendb();

            try
            {
                ConfigsNamesTemplate cnt = new ConfigsNamesTemplate();
                //对文件进行遍历
                foreach (var NextFile in theXMLFolder.GetFiles())
                {
                    if (Path.GetExtension(NextFile.Name) == ".xlsx" && !NextFile.Name.StartsWith("~$"))
                    {
                        FileStream stream = NextFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        string csvfile = XLSXTOCSV(stream);


                        //========================
                        SQLParser cp = new SQLParser();
                        CreateCSFile(csOutPath, NextFile.Name.Split('.')[0] + ".cs", cp.CreateCS(NextFile.Name.Split('.')[0], csvfile));

                        //这里固定取配置表第三行配置作为类型读取，如果需要修改配置表适配服务器（增加第四行），需要同步修改
                        //将excel写入csvconfigs.bytes
                        CSVReader reader = new CSVReader(csvfile);
                        sqliteWriter.Write(NextFile.Name.Split('.')[0], reader);

                        cnt.configsNameList.Add(NextFile.Name.Split('.')[0], reader.GetData(0, 2));


                    }
                    else if (Path.GetExtension(NextFile.Name) == ".txt")
                    {
                        FileInfo fi = new FileInfo(csvOutPath + "/" + NextFile.Name);
                        if (fi.Exists)
                            fi.Delete();
                        NextFile.CopyTo(csvOutPath + "/" + NextFile.Name);
                        
                        _csvListToBeRestored.Add(new ABVersion { AbName = NextFile.Name, MD5 = string.Empty, Version = 0 });
                    }
                }

                //=========================
                sqliteWriter.Closedb();
                string str = "csv/csvconfigs.bytes";
                _csvListToBeRestored.Add(new ABVersion { AbName = str, MD5 = LitFramework.Crypto.Crypto.md5.GetFileHash(csvOutPath + "/" + "csvconfigs.bytes"), Version = 1 });

                //遍历框架配置的额外后缀文件
                string[] extralFile = extralFileStr.Split('|');
                foreach (var item in extralFile)
                {
                    if (item.Equals("csv")) continue;

                    GetFiles(new DirectoryInfo(streampath), item, _csvListToBeRestored);
                }

                //============更新并保存CS============//
                SQLConfigsParse rpp = new SQLConfigsParse();

                if (!useHotFix)
                    CreateCSFile(CONFIG_CS_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
                else
                    CreateCSFile(CONFIG_CS_HOTFIX_OUTPUT_DIR, CS_CONFIGS, rpp.CreateCS(cnt));
            }
            catch (Exception e)
            {
                //===================
                sqliteWriter.Closedb();
                throw new Exception(e.Message);
            }
            finally
            {
                //加载本地文件，没有就创建完成。有则比对同名文件的MD5，不一样则version+1
                MatchCSVTotalFile(_csvListToBeRestored);
            }
        }
    }


    /// <summary>
    /// 配置表路径注册类
    /// </summary>
    class SQLConfigsParse
    {
        List<string> CSString = new List<string>();

        public string CreateCS(ConfigsNamesTemplate rpt)
        {
            AddHead();
            AddBody(rpt);
            AddTail();
            string result = GetFomatedCS();

            return result;
        }

        private void AddHead()
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
        private void AddTail()
        {
            CSString.Add("}");
        }
        private void AddBody(ConfigsNamesTemplate rpt)
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
        string GetFomatedCS()
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